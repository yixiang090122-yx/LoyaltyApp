using LoyaltyApp.DTOs;
using LoyaltyApp.ViewModels.Account;
using LoyaltyApp.ViewModels.Admin;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Nodes;

namespace LoyaltyApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly HttpClient _httpClient;

        public AccountController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("LoyaltyAppClient");
        }

        // ==========================================
        // MEMBER AUTHENTICATION
        // ==========================================

        // GET: /Account/MemberLogin
        public IActionResult MemberLogin()
        {
            return View();
        }

        // POST: /Account/MemberLogin
        [HttpPost]
        public async Task<IActionResult> MemberLogin(MemberLoginVM memberLoginVM)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, msg = "Invalid input data." });
            }

            var response = await _httpClient.PostAsJsonAsync("api/Member/MemberLogin", memberLoginVM);

            if (!response.IsSuccessStatusCode)
            {
                var errorMsg = await response.Content.ReadAsStringAsync();
                return Json(new
                {
                    success = false,
                    msg = string.IsNullOrWhiteSpace(errorMsg) ? "Login Failed. Please check your credentials." : errorMsg
                });
            }

            // Login Success
            var token = await response.Content.ReadFromJsonAsync<JsonNode>();
            var getToken = token?["successToken"]?.ToString();
            if (string.IsNullOrEmpty(getToken))
            {
                return Json(new { success = false, msg = "Failed to retrieve access token." });
            }

            HttpContext.Session.SetString("TOKEN", getToken);
            HttpContext.Session.SetString("MemberPhone", memberLoginVM.PhoneNumber);
            if (token?["otp"] != null) HttpContext.Session.SetString("OTP", token["otp"]!.ToString());
            if (token?["email"] != null) HttpContext.Session.SetString("EMAIL", token["email"]!.ToString());

            return Json(new
            {
                success = true,
                msg = "Login Success",
                redirectUrl = "/Member/Index"
            });
        }

        // GET: /Account/Register
        public IActionResult Register()
        {
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        public async Task<IActionResult> Register(RegisterMemberVM model)
        {
            if (!ModelState.IsValid)
            {
                var errors = string.Join(" ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return Json(new { success = false, msg = errors });
            }

            using var form = new MultipartFormDataContent();
            form.Add(new StringContent(model.Name), "Name");
            form.Add(new StringContent(model.PhoneNumber), "PhoneNumber");
            form.Add(new StringContent(model.Email), "Email");
            form.Add(new StringContent(model.Password), "Password");
            form.Add(new StringContent(model.BirthDate), "Birthday");

            var response = await _httpClient.PostAsync("api/Member/RegisterMember", form);
            if (response.IsSuccessStatusCode)
            {
                return Json(new
                {
                    success = true,
                    msg = "Registration successful! You may now log in.",
                    redirectUrl = "/Account/MemberLogin"
                });
            }

            var error = await response.Content.ReadAsStringAsync();
            return Json(new { success = false, msg = error });
        }

        // GET: /Account/ForgotPassword
        public IActionResult ForgotPassword()
        {
            return View();
        }

        // POST: /Account/ForgotPassword
        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordVM model)
        {
            if (string.IsNullOrWhiteSpace(model.PhoneNumber))
            {
                return Json(new { success = false, msg = "Please enter your registered phone number." });
            }

            var response = await _httpClient.PostAsJsonAsync("api/Member/GenerateOTP", new { PhoneNumber = model.PhoneNumber });
            if (response.IsSuccessStatusCode)
            {
                return Json(new
                {
                    success = true,
                    msg = "OTP has been sent to your registered email.",
                    redirectUrl = $"/Account/ResetPassword?phone={model.PhoneNumber}"
                });
            }

            var error = await response.Content.ReadAsStringAsync();
            return Json(new { success = false, msg = error });
        }

        // GET: /Account/ResetPassword
        public IActionResult ResetPassword(string? phone)
        {
            ViewBag.Phone = phone ?? string.Empty;
            return View();
        }

        // POST: /Account/ResetPassword
        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordVM model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, msg = "Please provide all required fields." });
            }

            var response = await _httpClient.PostAsJsonAsync("api/Member/MemberResetPassword", new
            {
                phoneNumber = model.PhoneNumber,
                otp = model.OTP,
                newPassword = model.NewPassword
            });

            if (response.IsSuccessStatusCode)
            {
                return Json(new
                {
                    success = true,
                    msg = "Password reset successfully! Please login with your new password.",
                    redirectUrl = "/Account/MemberLogin"
                });
            }

            var error = await response.Content.ReadAsStringAsync();
            return Json(new { success = false, msg = error });
        }

        // POST: /Account/Logout
        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("MemberLogin", "Account");
        }

        // ==========================================
        // ADMIN AUTHENTICATION
        // ==========================================

        // GET: /Account/AdminLogin
        public IActionResult AdminLogin()
        {
            return View();
        }

        // POST: /Account/AdminLogin
        [HttpPost]
        public async Task<IActionResult> AdminLogin(AdminLoginVM adminLoginVM)
        {
            var response = await _httpClient.PostAsJsonAsync("api/Admin/AdminLogin", adminLoginVM);

            if (!response.IsSuccessStatusCode)
            {
                return Json(new
                {
                    success = false,
                    msg = "Invalid admin username or password."
                });
            }

            var token = await response.Content.ReadFromJsonAsync<JsonNode>();
            if (token != null && token["successToken"] != null)
            {
                HttpContext.Session.SetString("ADMIN_TOKEN", token["successToken"]!.ToString());
            }
            HttpContext.Session.SetString("AdminUser", adminLoginVM.Username);

            return Json(new
            {
                success = true,
                msg = "Login Success",
                redirectUrl = "/Admin/Index"
            });
        }

        // POST: /Account/AdminLogout
        [HttpPost]
        public IActionResult AdminLogout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("AdminLogin", "Account");
        }
    }
}
