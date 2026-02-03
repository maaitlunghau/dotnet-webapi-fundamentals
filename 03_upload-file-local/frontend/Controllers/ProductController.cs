using frontend.Helpers;
using LModels.Data;
using Microsoft.AspNetCore.Mvc;

namespace frontend.Controllers
{
    public class ProductController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private string ProductApiBaseURL = "http://localhost:5041/api/product";
        public ProductController(
            HttpClient httpClient,
            IWebHostEnvironment webHostEnvironment
        )
        {
            _httpClient = httpClient;
            _webHostEnvironment = webHostEnvironment;
        }


        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var products = await _httpClient.GetFromJsonAsync<List<Product>>(ProductApiBaseURL);
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
            if (formFile is null || formFile.Length == 0)
                ModelState.AddModelError("Image", "Vui lòng upload hình.");
            if (!ModelState.IsValid) return View(pro);

            var imagePath = await FileUpload.SaveImage(
                _webHostEnvironment,
                "ProductImages",
                formFile!
            );
            pro.ImageUrl = imagePath;

            var res = await _httpClient.PostAsJsonAsync(ProductApiBaseURL, pro);
            if (res.IsSuccessStatusCode)
                return RedirectToAction(nameof(Index));

            return View(pro);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var pro = await _httpClient.GetFromJsonAsync<Product>($"{ProductApiBaseURL}/{id}");
            if (pro is null) return NotFound();

            var response = await _httpClient.DeleteAsync($"{ProductApiBaseURL}/{id}");
            if (response.IsSuccessStatusCode)
            {
                FileUpload.DeleteImage(_webHostEnvironment, pro.ImageUrl!);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}