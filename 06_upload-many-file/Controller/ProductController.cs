using System.Net;
using _06_upload_many_file.DTOs;
using _06_upload_many_file.Helpers;
using _06_upload_many_file.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;

namespace _06_upload_many_file.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly DataContext _dbContext;
        private readonly IFileStorage _fileStorage;
        public ProductController(DataContext dbContext, IFileStorage fileStorage)
        {
            _dbContext = dbContext;
            _fileStorage = fileStorage;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var products = await _dbContext.Products
                    .Include(pro => pro.ProductImages)
                    .Select(pro => new
                    {
                        pro.Id,
                        pro.Name,
                        pro.Price,
                        Images = pro.ProductImages.Select(img => new
                        {
                            img.Id,
                            img.Url
                        }).ToList()
                    })
                    .ToListAsync();
                return Ok(products);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] ProductCreateRequestDto pro, CancellationToken token)
        {
            try
            {
                var product = new Product
                {
                    Name = pro.Name,
                    Price = pro.Price
                };
                if (pro.Images == null || pro.Images?.Count == 0)
                    return BadRequest("Image is required");

                foreach (var file in pro.Images!)
                {
                    var url = await _fileStorage.SaveFileAsync(file, "products", token);

                    // thêm luôn vào ProductImage (thông qua cái quan hệ NAVIGATION PROPERTY)
                    product.ProductImages?.Add(new ProductImage
                    {
                        Url = url
                    });
                }

                await _dbContext.Products.AddAsync(product);
                await _dbContext.SaveChangesAsync(token);

                var imageUrls = product.ProductImages?.Select(img => img.Url).ToList() ?? new List<string>();

                return Ok(new
                {
                    product.Id,
                    product.Name,
                    product.Price,
                    Images = imageUrls,
                    Message = "Product created successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }
    }
}
