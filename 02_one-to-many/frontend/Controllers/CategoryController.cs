using backend.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace frontend.Controllers
{
    public class CategoryController : Controller
    {
        private readonly HttpClient _httpClient;
        private string CategoryApiBaseURL = "http://localhost:5008/api/Category";

        public CategoryController(HttpClient httpClient) => _httpClient = httpClient;


        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var data = await _httpClient.GetFromJsonAsync<List<CategoryDto>>(CategoryApiBaseURL);
            return View(data);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateCategoryDto category)
        {
            if (!ModelState.IsValid) return View(category);

            var response = await _httpClient.PostAsJsonAsync(CategoryApiBaseURL, category);
            if (response.IsSuccessStatusCode)
                return RedirectToAction(nameof(Index));

            return View(category);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var data = await _httpClient.GetFromJsonAsync<CategoryDto>($"{CategoryApiBaseURL}/{id}");
            if (data == null) return NotFound();

            var model = new UpdateCategoryDto(data.Name);
            ViewBag.CategoryId = id;
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, UpdateCategoryDto category)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.CategoryId = id;
                return View(category);
            }

            var response = await _httpClient.PutAsJsonAsync($"{CategoryApiBaseURL}/{id}", category);
            if (response.IsSuccessStatusCode)
                return RedirectToAction(nameof(Index));

            ViewBag.CategoryId = id;
            return View(category);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var data = await _httpClient.GetFromJsonAsync<CategoryDto>($"{CategoryApiBaseURL}/{id}");
            if (data == null)
                return NotFound();

            return View(data);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var response = await _httpClient.DeleteAsync($"{CategoryApiBaseURL}/{id}");
            if (response.IsSuccessStatusCode)
                return RedirectToAction(nameof(Index));

            return RedirectToAction(nameof(Delete), new { id });
        }
    }
}