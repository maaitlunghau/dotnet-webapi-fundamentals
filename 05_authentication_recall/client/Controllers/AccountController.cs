using client.DTOs;
using client.Models;
using Microsoft.AspNetCore.Mvc;

namespace client.Controllers
{
    public class AccountController : Controller
    {
        private readonly HttpClient _httpClient;
        private string BaseUrlAccount = "http://localhost:5023/api/Account";

        public AccountController(HttpClient httpClient)
            => _httpClient = httpClient;

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var acc = _httpClient.GetFromJsonAsync<List<Account>>(BaseUrlAccount).Result;
            return View(acc);
        }

        [HttpGet]
        public async Task<IActionResult> Login()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> CheckLogin(LoginDto dto)
        {
            var res = await _httpClient.PostAsJsonAsync($"{BaseUrlAccount}/login", dto);
            if (res.IsSuccessStatusCode)
            {
                var user = await res.Content.ReadFromJsonAsync<Account>();
                if (user is not null && user.Role == "ADMIN")
                {
                    return RedirectToAction(nameof(Index));
                }
                else if (user is not null)
                {
                    return RedirectToAction("Detail");
                }
            }

            ViewBag.ErrorMessage = "Login failed!";

            return View();
        }

        [HttpGet]
        public IActionResult Detail(int id)
        {
            var acc = _httpClient.GetFromJsonAsync<Account>($"{BaseUrlAccount}/{id}");

            return View(acc);
        }
    }
}
