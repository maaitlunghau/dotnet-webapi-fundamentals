using frontend.DTOs;
using frontend.Helpers;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace frontend.Controllers
{
    public class AuthController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly string _authApiBaseUrl = "http://localhost:5013/api/Auth";
        public AuthController(HttpClient httpClient) => _httpClient = httpClient;

        // GET: /Auth/Login
        [HttpGet]
        public IActionResult Login()
        {
            // Nếu đã login, redirect về Home
            var currentUser = HttpContext.Session.GetObject<UserSessionDto>("CurrentUser");
            if (currentUser != null)
                return RedirectToAction("Index", "Home");

            return View();
        }

        // POST: /Auth/Login
        [HttpPost]
        public async Task<IActionResult> Login(LoginRequestDto loginRequest)
        {
            if (!ModelState.IsValid)
                return View(loginRequest);

            try
            {
                // Call Backend API
                var response = await _httpClient.PostAsJsonAsync(
                    $"{_authApiBaseUrl}/Login",
                    loginRequest
                );
                if (!response.IsSuccessStatusCode)
                {
                    TempData["ErrorMessage"] = "Invalid email or password.";
                    return View(loginRequest);
                }

                // Parse response
                var authResponse = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
                if (authResponse == null)
                {
                    TempData["ErrorMessage"] = "Login failed. Please try again.";
                    return View(loginRequest);
                }

                // Lưu tokens vào Cookie (HttpOnly, Secure)
                Response.Cookies.Append("AccessToken", authResponse.AccessToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTimeOffset.UtcNow.AddMinutes(5)
                });

                Response.Cookies.Append("RefreshToken", authResponse.RefreshToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTimeOffset.UtcNow.AddDays(7)
                });

                // Parse JWT để lấy thông tin user
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(authResponse.AccessToken);

                var userId = jwtToken.Claims
                    .FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;
                var email = jwtToken.Claims
                    .FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Email)?.Value;
                var role = jwtToken.Claims
                    .FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

                // Tạo UserSessionDto
                var userSession = new UserSessionDto
                {
                    Id = Guid.Parse(userId ?? Guid.Empty.ToString()),
                    Email = email ?? loginRequest.Email,
                    Role = role ?? "user"
                };

                // Lưu User object vào Session
                HttpContext.Session.SetObject("CurrentUser", userSession);

                TempData["SuccessMessage"] = $"Welcome back, {userSession.Email}!";
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An error occurred: {ex.Message}";
                return View(loginRequest);
            }
        }

        // POST: /Auth/Logout
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            try
            {
                // Lấy refresh token từ cookie
                var refreshToken = Request.Cookies["RefreshToken"];

                if (!string.IsNullOrEmpty(refreshToken))
                {
                    // Call Backend API để revoke token
                    await _httpClient.PostAsJsonAsync(
                        $"{_authApiBaseUrl}/Logout",
                        new { refreshToken }
                    );
                }
            }
            catch
            {
                // Ignore errors, vẫn logout ở frontend
            }

            // Xóa cookies
            Response.Cookies.Delete("AccessToken");
            Response.Cookies.Delete("RefreshToken");

            // Xóa session
            HttpContext.Session.Clear();

            TempData["SuccessMessage"] = "You have been logged out successfully.";
            return RedirectToAction(nameof(Login));
        }
    }
}
