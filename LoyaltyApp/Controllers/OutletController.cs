using LoyaltyApp.DTOs;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;

namespace LoyaltyApp.Controllers
{
    public class OutletController : Controller
    {
        private readonly HttpClient _httpClient;

        public OutletController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("LoyaltyAppClient");
        }

        // ==========================================
        // ADMIN OUTLET MANAGEMENT
        // ==========================================

        // GET: /Outlet or /Outlet/Index
        public async Task<IActionResult> Index()
        {
            var adminToken = HttpContext.Session.GetString("ADMIN_TOKEN");
            if (string.IsNullOrEmpty(adminToken))
            {
                return RedirectToAction("AdminLogin", "Account");
            }

            var request = new HttpRequestMessage(HttpMethod.Get, "api/Outlet/GetAllOutlets");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);

            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Error = "Unable to fetch outlets from backend.";
                return View(new List<OutletDTO>());
            }

            var outlets = await response.Content.ReadFromJsonAsync<List<OutletDTO>>() ?? new List<OutletDTO>();
            return View(outlets);
        }

        // POST: /Outlet/Create
        [HttpPost]
        public async Task<IActionResult> Create(CreateOutletDTO model)
        {
            var adminToken = HttpContext.Session.GetString("ADMIN_TOKEN");
            if (string.IsNullOrEmpty(adminToken))
            {
                return Json(new { success = false, msg = "Unauthorized session. Please login as admin." });
            }

            if (string.IsNullOrWhiteSpace(model.OutletName))
            {
                return Json(new { success = false, msg = "Outlet name cannot be empty." });
            }

            var request = new HttpRequestMessage(HttpMethod.Post, "api/Outlet/PostOutlet");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);
            request.Content = JsonContent.Create(new { OutletName = model.OutletName });

            var response = await _httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                return Json(new { success = true, msg = "Outlet created successfully!" });
            }

            var error = await response.Content.ReadAsStringAsync();
            return Json(new { success = false, msg = string.IsNullOrWhiteSpace(error) ? "Failed to create outlet." : error });
        }

        // POST: /Outlet/ToggleStatus/5
        [HttpPost]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            var adminToken = HttpContext.Session.GetString("ADMIN_TOKEN");
            if (string.IsNullOrEmpty(adminToken))
            {
                return Json(new { success = false, msg = "Unauthorized session. Please login as admin." });
            }

            var request = new HttpRequestMessage(HttpMethod.Get, $"api/Outlet/ActiveOrDeactiveOutlet/{id}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);

            var response = await _httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                return Json(new { success = true, msg = "Outlet status updated successfully!" });
            }

            return Json(new { success = false, msg = "Failed to update outlet status." });
        }

        // ==========================================
        // MEMBER / PUBLIC STORE LOCATIONS
        // ==========================================

        // GET: /Outlet/Locations
        public async Task<IActionResult> Locations()
        {
            // Anonymous or Member can browse outlets
            var response = await _httpClient.GetAsync("api/MemberOutlet/GetAllOutlets");
            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Error = "Unable to load store locations.";
                return View(new List<OutletDTO>());
            }

            var outlets = await response.Content.ReadFromJsonAsync<List<OutletDTO>>() ?? new List<OutletDTO>();
            return View(outlets);
        }
    }
}
