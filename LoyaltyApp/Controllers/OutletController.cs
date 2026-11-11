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

        private HttpRequestMessage? CreateAdminRequest(HttpMethod method, string requestUri)
        {
            var adminToken = HttpContext.Session.GetString("ADMIN_TOKEN");
            if (string.IsNullOrEmpty(adminToken)) return null;

            var request = new HttpRequestMessage(method, requestUri);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);
            return request;
        }

        // ==========================================
        // ADMIN OUTLET MANAGEMENT
        // ==========================================

        // GET: /Outlet/Index
        public async Task<IActionResult> Index()
        {
            var request = CreateAdminRequest(HttpMethod.Get, "api/Outlet/GetAllOutlets");
            if (request == null) return RedirectToAction("AdminLogin", "Account");

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
            var request = CreateAdminRequest(HttpMethod.Post, "api/Outlet/PostOutlet");
            if (request == null) return Json(new { success = false, msg = "Unauthorized session. Please login as admin." });

            if (string.IsNullOrWhiteSpace(model.OutletName))
            {
                return Json(new { success = false, msg = "Outlet name cannot be empty." });
            }

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
            var request = CreateAdminRequest(HttpMethod.Get, $"api/Outlet/ActiveOrDeactiveOutlet/{id}");
            if (request == null) return Json(new { success = false, msg = "Unauthorized session. Please login as admin." });

            var response = await _httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                return Json(new { success = true, msg = "Outlet status updated successfully!" });
            }

            return Json(new { success = false, msg = "Failed to update outlet status." });
        }

        // ==========================================
        // OUTLET STAFF MANAGEMENT (PIN MATRIX)
        // ==========================================

        // GET: /Outlet/GetStaffByOutlet?outletId=1
        [HttpGet]
        public async Task<IActionResult> GetStaffByOutlet(int outletId)
        {
            var request = CreateAdminRequest(HttpMethod.Post, "api/OutletStaff/GetAllOutletStaff");
            if (request == null) return Json(new { success = false, msg = "Unauthorized session." });

            request.Content = JsonContent.Create(new { OutletID = outletId });

            var response = await _httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var staffList = await response.Content.ReadFromJsonAsync<List<OutletStaffDTO>>() ?? new List<OutletStaffDTO>();
                return Json(new { success = true, data = staffList });
            }

            return Json(new { success = false, msg = "Unable to fetch staff members." });
        }

        // POST: /Outlet/CreateStaff
        [HttpPost]
        public async Task<IActionResult> CreateStaff(CreateOutletStaffDTO model)
        {
            var request = CreateAdminRequest(HttpMethod.Post, "api/OutletStaff/CreateOutletStaff");
            if (request == null) return Json(new { success = false, msg = "Unauthorized session." });

            if (string.IsNullOrWhiteSpace(model.Pin) || model.Pin.Length != 6)
            {
                return Json(new { success = false, msg = "PIN must be exactly 6 digits." });
            }

            request.Content = JsonContent.Create(new
            {
                OutletStaffName = model.OutletStaffName,
                Pin = model.Pin,
                Role = string.IsNullOrWhiteSpace(model.Role) ? "Cashier" : model.Role,
                OutletID = model.OutletID
            });

            var response = await _httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                return Json(new { success = true, msg = "Staff member & PIN added successfully!" });
            }

            var err = await response.Content.ReadAsStringAsync();
            return Json(new { success = false, msg = err });
        }

        // POST: /Outlet/DeleteStaff/5
        [HttpPost]
        public async Task<IActionResult> DeleteStaff(int id)
        {
            var request = CreateAdminRequest(HttpMethod.Delete, $"api/OutletStaff/DeleteOutletStaff/{id}");
            if (request == null) return Json(new { success = false, msg = "Unauthorized session." });

            var response = await _httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                return Json(new { success = true, msg = "Staff removed successfully." });
            }

            return Json(new { success = false, msg = "Failed to remove staff." });
        }

        // ==========================================
        // MEMBER / PUBLIC STORE LOCATIONS
        // ==========================================

        // GET: /Outlet/Locations
        public async Task<IActionResult> Locations()
        {
            var response = await _httpClient.GetAsync("api/MemberOutlet/GetAllOutlets");
            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Error = "Unable to load store locations.";
                return View(new List<OutletDTO>());
            }

            var outlets = await response.Content.ReadFromJsonAsync<List<OutletDTO>>() ?? new List<OutletDTO>();
            return View(outlets);
        }

        // GET: /Outlet/GetMemberOutletStaff?guid=...
        [HttpGet]
        public async Task<IActionResult> GetMemberOutletStaff(string guid)
        {
            var response = await _httpClient.GetAsync($"api/MemberOutletStaff/GetAllOutletStaff/{guid}");
            if (response.IsSuccessStatusCode)
            {
                var staff = await response.Content.ReadFromJsonAsync<List<OutletStaffDTO>>() ?? new List<OutletStaffDTO>();
                return Json(new { success = true, data = staff });
            }

            return Json(new { success = false, msg = "No staff data available." });
        }
    }
}
