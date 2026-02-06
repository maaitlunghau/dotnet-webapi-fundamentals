using frontend.DTOs;
using frontend.Helpers;
using Microsoft.AspNetCore.Mvc;
using Shared.Domain;

namespace frontend.Controllers
{
    public class UserController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly string _userApiBaseUrl = "http://localhost:5013/api/User";
        public UserController(HttpClient httpClient) => _httpClient = httpClient;


        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var currentUser = HttpContext.Session.GetObject<UserSessionDto>("CurrentUser");
            if (currentUser != null && currentUser?.Role != "admin")
            {
                TempData["ErrorMessage"] = "Access denied. Admins only.";
                return RedirectToAction("Index", "Home");
            }
            else if (currentUser == null)
            {
                TempData["ErrorMessage"] = "Please login to access the user list.";
                return RedirectToAction("Login", "Auth");
            }

            var users = await _httpClient.GetFromJsonAsync<List<User>>(_userApiBaseUrl);
            return View(users);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(User user)
        {
            if (!ModelState.IsValid) return View(user);

            var response = await _httpClient.PostAsJsonAsync(_userApiBaseUrl, user);
            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = $"User '{user.Email}' has been created successfully.";
                return RedirectToAction(nameof(Index));
            }

            TempData["ErrorMessage"] = "Failed to create user. Please try again.";
            return View(user);
        }

        public async Task<IActionResult> Update(Guid id)
        {
            var existingUser = await _httpClient.GetFromJsonAsync<User>($"{_userApiBaseUrl}/{id}");
            if (existingUser == null) return NotFound();

            ViewBag.CurrentRole = existingUser.Role;

            return View(existingUser);
        }

        [HttpPost]
        public async Task<IActionResult> Update(Guid id, User user)
        {
            if (!ModelState.IsValid) return View(user);

            var response = await _httpClient.PutAsJsonAsync($"{_userApiBaseUrl}/{id}", user);
            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = $"User '{user.Email}' has been updated successfully.";
                return RedirectToAction(nameof(Index));
            }

            TempData["ErrorMessage"] = "Failed to update user. Please try again.";
            return View(user);
        }

        public async Task<IActionResult> Delete(Guid id)
        {
            var response = await _httpClient.DeleteAsync($"{_userApiBaseUrl}/{id}");
            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "User has been deleted successfully.";
                return RedirectToAction(nameof(Index));
            }

            TempData["ErrorMessage"] = "Failed to delete user. Please try again.";
            return RedirectToAction(nameof(Index));
        }
    }
}
