using backend.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace frontend.Controllers
{
    public class ProductController : Controller
    {
        private readonly HttpClient _httpClient;
        private string ProductApiBaseURL = "http://localhost:5008/api/Product";
        private string CategoryApiBaseURL = "http://localhost:5008/api/Category";

        public ProductController(HttpClient httpClient) => _httpClient = httpClient;


        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var data = await _httpClient.GetFromJsonAsync<List<ProductDto>>(ProductApiBaseURL);
            return View(data);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await LoadCategories();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateProductDto product)
        {
            if (!ModelState.IsValid)
            {
                await LoadCategories();
                return View(product);
            }

            var response = await _httpClient.PostAsJsonAsync(ProductApiBaseURL, product);
            if (response.IsSuccessStatusCode)
                return RedirectToAction(nameof(Index));

            await LoadCategories();
            return View(product);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var data = await _httpClient.GetFromJsonAsync<ProductDto>($"{ProductApiBaseURL}/{id}");
            if (data == null) return NotFound();

            var model = new UpdateProductDto(
                data.Name,
                data.Price,
                data.Stock,
                data.CategoryId
            );

            ViewBag.ProductId = id;

            await LoadCategories();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, UpdateProductDto product)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ProductId = id;

                await LoadCategories();
                return View(product);
            }

            var response = await _httpClient.PutAsJsonAsync($"{ProductApiBaseURL}/{id}", product);
            if (response.IsSuccessStatusCode)
                return RedirectToAction(nameof(Index));

            ViewBag.ProductId = id;

            await LoadCategories();
            return View(product);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var data = await _httpClient.GetFromJsonAsync<ProductDto>($"{ProductApiBaseURL}/{id}");
            if (data == null) return NotFound();

            return View(data);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var response = await _httpClient.DeleteAsync($"{ProductApiBaseURL}/{id}");
            if (response.IsSuccessStatusCode)
                return RedirectToAction(nameof(Index));

            return RedirectToAction(nameof(Delete), new { id });
        }

        private async Task LoadCategories()
        {
            var categories = await _httpClient
                .GetFromJsonAsync<List<CategoryDto>>(CategoryApiBaseURL);
            ViewBag.Categories = new SelectList(categories, "Id", "Name");
        }
    }
}
