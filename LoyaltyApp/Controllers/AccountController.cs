using LoyaltyApp.ViewModels.Account;
using LoyaltyApp.ViewModels.Admin;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Nodes;

namespace LoyaltyApp.Controllers
{
    public class AccountController : Controller
    {
        HttpClient _httpClient;
        public AccountController(IHttpClientFactory httpClient)
        {
            _httpClient = httpClient.CreateClient("LoyaltyAppClient");
        }

        // Member Login
        public IActionResult MemberLogin()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> MemberLogin(MemberLoginVM memberLoginVM)
        {
            var response = await _httpClient.PostAsJsonAsync("api/Member/MemberLogin", memberLoginVM);

            if (!response.IsSuccessStatusCode)
            {
                return Json(new
                {
                    success = false,
                    msg = "Login Fail"
                });
            }

            // Login Success
            var token = await response.Content.ReadFromJsonAsync<JsonNode>();
            var getToken = token["successToken"].ToString();
            HttpContext.Session.SetString("TOKEN", getToken);
            HttpContext.Session.SetString("MemberPhone", memberLoginVM.PhoneNumber);
            HttpContext.Session.SetString("OTP", token["otp"].ToString());
            HttpContext.Session.SetString("EMAIL", token["email"].ToString());

            return Json(new
            {
                success = true,
                msg = "Login Success",
                redirectUrl = "/Member/Index"
            });
        }

        // Admin Login
        public IActionResult AdminLogin()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AdminLogin(AdminLoginVM adminLoginVM)
        {
            var response = await _httpClient.PostAsJsonAsync("api/Admin/AdminLogin", adminLoginVM);

            if (!response.IsSuccessStatusCode)
            {
                return Json(new
                {
                    success = false,
                    msg = "Login Fail"
                });
            }

            // Login Success
            var token = await response.Content.ReadFromJsonAsync<JsonNode>();
            if (token != null && token["successToken"] != null)
            {
                HttpContext.Session.SetString("ADMIN_TOKEN", token["successToken"].ToString());
            }
            HttpContext.Session.SetString("AdminUser", adminLoginVM.Username);

            return Json(new
            {
                success = true,
                msg = "Login Success",
                redirectUrl = "/Admin/Index"
            });
        }

        // Member Logout Action
        [HttpPost]
        public IActionResult Logout()
        {
            // Clear all session tokens/data
            HttpContext.Session.Clear();

            // Redirect back to Member Login page
            return RedirectToAction("MemberLogin", "Account");
        }

        // Admin Logout Action
        [HttpPost]
        public IActionResult AdminLogout()
        {
            // Clear all session tokens/data
            HttpContext.Session.Clear();

            // Redirect back to Admin Login page specifically
            return RedirectToAction("AdminLogin", "Account");
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}