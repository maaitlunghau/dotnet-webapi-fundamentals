using client.Models;
using client.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace client.Controllers
{
    public class ProductController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private const string ProductApiBaseURL = "http://localhost:5111/api/product";

        public ProductController(
            HttpClient httpClient,
            IWebHostEnvironment webHostEnvironment
        )
        {
            _httpClient = httpClient;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        public async Task<IActionResult> Index(decimal? minPrice, decimal? maxPrice)
        {
            var url = $"{ProductApiBaseURL}?minPrice={minPrice}&maxPrice={maxPrice}";
            var products = await _httpClient.GetFromJsonAsync<List<Product>>(url);
            
            ViewBag.MinPrice = minPrice;
            ViewBag.MaxPrice = maxPrice;
            
            return View(products);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Product pro, IFormFile formFile)
        {
            ModelState.Remove("Image");

            if (formFile is null || formFile.Length == 0)
                ModelState.AddModelError("Image", "Vui lòng upload hình.");
            
            if (!ModelState.IsValid) return View(pro);

            try 
            {
                var imagePath = await FileUpload.SaveImage(
                    _webHostEnvironment,
                    "ProductImages",
                    formFile!
                );
                pro.Image = imagePath;
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(pro);
            }

            var res = await _httpClient.PostAsJsonAsync(ProductApiBaseURL, pro);
            if (res.IsSuccessStatusCode)
                return RedirectToAction(nameof(Index));

            return View(pro);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int id, bool status)
        {
            var response = await _httpClient.PutAsync($"{ProductApiBaseURL}/{id}?status={status}", null);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var products = await _httpClient.GetFromJsonAsync<List<Product>>(ProductApiBaseURL);
            var pro = products?.FirstOrDefault(p => p.Id == id);
            
            if (pro is null) return NotFound();

            var response = await _httpClient.DeleteAsync($"{ProductApiBaseURL}/{id}");
            if (response.IsSuccessStatusCode)
            {
                FileUpload.DeleteImage(_webHostEnvironment, pro.Image!);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
