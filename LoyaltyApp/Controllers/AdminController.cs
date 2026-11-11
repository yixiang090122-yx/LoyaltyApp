using LoyaltyApp.DTOs;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text.Json;

namespace LoyaltyApp.Controllers
{
    public class AdminController : Controller
    {
        private readonly HttpClient _httpClient;

        public AdminController(IHttpClientFactory httpClientFactory)
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
        // A2: ADMIN DASHBOARD
        // ==========================================
        public async Task<IActionResult> Index()
        {
            var req = CreateAdminRequest(HttpMethod.Get, "api/ManageMember/GetAllMember");
            if (req == null) return RedirectToAction("AdminLogin", "Account");

            // 1. Get Members
            var membersResp = await _httpClient.SendAsync(req);
            var members = membersResp.IsSuccessStatusCode
                ? await membersResp.Content.ReadFromJsonAsync<List<MemberDTO>>() ?? new List<MemberDTO>()
                : new List<MemberDTO>();

            // 2. Get Rewards
            var rewardsResp = await _httpClient.SendAsync(CreateAdminRequest(HttpMethod.Get, "api/ManageRewards/GetAllRewards")!);
            var rewards = rewardsResp.IsSuccessStatusCode
                ? await rewardsResp.Content.ReadFromJsonAsync<List<RewardDTO>>() ?? new List<RewardDTO>()
                : new List<RewardDTO>();

            // 3. Get Vouchers
            var vouchersResp = await _httpClient.SendAsync(CreateAdminRequest(HttpMethod.Get, "api/ManageRewards/GetAllVouchers")!);
            var vouchers = vouchersResp.IsSuccessStatusCode
                ? await vouchersResp.Content.ReadFromJsonAsync<List<VoucherDTO>>() ?? new List<VoucherDTO>()
                : new List<VoucherDTO>();

            // 4. Get Points
            var pointsResp = await _httpClient.SendAsync(CreateAdminRequest(HttpMethod.Get, "api/ManagePoint/GetAllPointAssignRecord")!);
            var pointRecords = pointsResp.IsSuccessStatusCode
                ? await pointsResp.Content.ReadFromJsonAsync<List<PointRecordDTO>>() ?? new List<PointRecordDTO>()
                : new List<PointRecordDTO>();

            ViewBag.TotalMembers = members.Count;
            ViewBag.ActiveRewards = rewards.Count(r => r.Status == "Active");
            ViewBag.ActiveVouchers = vouchers.Count;
            ViewBag.TotalPointRecords = pointRecords.Count;
            ViewBag.RecentMembers = members.Take(5).ToList();
            ViewBag.RecentPoints = pointRecords.Take(5).ToList();

            return View();
        }

        // ==========================================
        // A3: MANAGE MEMBERS
        // ==========================================
        public async Task<IActionResult> Members()
        {
            var req = CreateAdminRequest(HttpMethod.Get, "api/ManageMember/GetAllMember");
            if (req == null) return RedirectToAction("AdminLogin", "Account");

            var response = await _httpClient.SendAsync(req);
            var members = response.IsSuccessStatusCode
                ? await response.Content.ReadFromJsonAsync<List<MemberDTO>>() ?? new List<MemberDTO>()
                : new List<MemberDTO>();

            return View(members);
        }

        [HttpPost]
        public async Task<IActionResult> CreateMember(MemberDTO model, string password)
        {
            var req = CreateAdminRequest(HttpMethod.Post, "api/ManageMember/CreateMember");
            if (req == null) return Json(new { success = false, msg = "Unauthorized session." });

            req.Content = JsonContent.Create(new
            {
                Name = model.Name,
                PhoneNumber = model.PhoneNumber,
                Email = model.Email,
                BirthDate = string.IsNullOrWhiteSpace(model.BirthDate) ? "2000-01-01" : model.BirthDate,
                Password = string.IsNullOrWhiteSpace(password) ? "123456" : password,
                Tier = string.IsNullOrWhiteSpace(model.Tier) ? "Gold" : model.Tier
            });

            var response = await _httpClient.SendAsync(req);
            if (response.IsSuccessStatusCode)
            {
                return Json(new { success = true, msg = "Member created successfully!" });
            }

            var err = await response.Content.ReadAsStringAsync();
            return Json(new { success = false, msg = err });
        }

        [HttpPost]
        public async Task<IActionResult> EditMember(MemberDTO model)
        {
            var req = CreateAdminRequest(HttpMethod.Post, "api/ManageMember/EditMember");
            if (req == null) return Json(new { success = false, msg = "Unauthorized session." });

            req.Content = JsonContent.Create(model);

            var response = await _httpClient.SendAsync(req);
            if (response.IsSuccessStatusCode)
            {
                return Json(new { success = true, msg = "Member updated successfully!" });
            }

            var err = await response.Content.ReadAsStringAsync();
            return Json(new { success = false, msg = err });
        }

        [HttpPost]
        public async Task<IActionResult> ToggleMemberStatus(int memberId)
        {
            var req = CreateAdminRequest(HttpMethod.Post, "api/ManageMember/ActiveOrDeactiveMember");
            if (req == null) return Json(new { success = false, msg = "Unauthorized session." });

            req.Content = JsonContent.Create(new { MemberID = memberId });

            var response = await _httpClient.SendAsync(req);
            if (response.IsSuccessStatusCode)
            {
                return Json(new { success = true, msg = "Member status updated successfully!" });
            }

            return Json(new { success = false, msg = "Failed to update member status." });
        }

        // ==========================================
        // A4: MANAGE REWARDS
        // ==========================================
        public async Task<IActionResult> Rewards()
        {
            var req = CreateAdminRequest(HttpMethod.Get, "api/ManageRewards/GetAllRewards");
            if (req == null) return RedirectToAction("AdminLogin", "Account");

            var response = await _httpClient.SendAsync(req);
            var rewards = response.IsSuccessStatusCode
                ? await response.Content.ReadFromJsonAsync<List<RewardDTO>>() ?? new List<RewardDTO>()
                : new List<RewardDTO>();

            return View(rewards);
        }

        [HttpPost]
        public async Task<IActionResult> CreateReward(CreateRewardVM model)
        {
            var req = CreateAdminRequest(HttpMethod.Post, "api/ManageRewards/CreateReward");
            if (req == null) return Json(new { success = false, msg = "Unauthorized session." });

            using var form = new MultipartFormDataContent();
            form.Add(new StringContent(model.FullName), "FullName");
            form.Add(new StringContent(model.Title), "Title");
            form.Add(new StringContent(model.SubTitle ?? ""), "SubTitle");
            form.Add(new StringContent(model.ShortTitle ?? ""), "ShortTitle");
            form.Add(new StringContent(model.Description ?? ""), "Description");
            form.Add(new StringContent(model.TnC ?? ""), "TnC");
            form.Add(new StringContent(model.Point.ToString()), "Point");
            form.Add(new StringContent(model.CreateDate.ToString("yyyy-MM-ddTHH:mm:ss")), "CreateDate");
            form.Add(new StringContent(model.ExpireDate.ToString("yyyy-MM-ddTHH:mm:ss")), "ExpireDate");
            form.Add(new StringContent(model.Status), "Status");

            var response = await _httpClient.PostAsync("api/ManageRewards/CreateReward", form);
            if (response.IsSuccessStatusCode)
            {
                return Json(new { success = true, msg = "Reward created successfully!" });
            }

            var err = await response.Content.ReadAsStringAsync();
            return Json(new { success = false, msg = err });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteReward(int id)
        {
            var req = CreateAdminRequest(HttpMethod.Get, $"api/ManageRewards/DeleteReward/{id}");
            if (req == null) return Json(new { success = false, msg = "Unauthorized session." });

            var response = await _httpClient.SendAsync(req);
            if (response.IsSuccessStatusCode)
            {
                return Json(new { success = true, msg = "Reward deleted successfully." });
            }

            return Json(new { success = false, msg = "Failed to delete reward." });
        }

        // ==========================================
        // A5: MANAGE VOUCHERS
        // ==========================================
        public async Task<IActionResult> Vouchers()
        {
            var req = CreateAdminRequest(HttpMethod.Get, "api/ManageRewards/GetAllVouchers");
            if (req == null) return RedirectToAction("AdminLogin", "Account");

            var response = await _httpClient.SendAsync(req);
            var vouchers = response.IsSuccessStatusCode
                ? await response.Content.ReadFromJsonAsync<List<VoucherDTO>>() ?? new List<VoucherDTO>()
                : new List<VoucherDTO>();

            return View(vouchers);
        }

        [HttpPost]
        public async Task<IActionResult> CreateVoucher(CreateVoucherVM model)
        {
            var req = CreateAdminRequest(HttpMethod.Post, "api/ManageRewards/CreateVoucher");
            if (req == null) return Json(new { success = false, msg = "Unauthorized session." });

            using var form = new MultipartFormDataContent();
            form.Add(new StringContent(model.VoucherName), "VoucherName");
            form.Add(new StringContent(model.Title), "Title");
            form.Add(new StringContent(model.Subtitle ?? ""), "Subtitle");
            form.Add(new StringContent(model.ShortTitle ?? ""), "ShortTitle");
            form.Add(new StringContent(model.Description ?? ""), "Description");
            form.Add(new StringContent(model.TNC ?? ""), "TNC");
            form.Add(new StringContent(model.CreatedDate.ToString("yyyy-MM-ddTHH:mm:ss")), "CreatedDate");
            form.Add(new StringContent(model.ExpiredDate.ToString("yyyy-MM-ddTHH:mm:ss")), "ExpiredDate");
            form.Add(new StringContent(model.VoucherRules ?? "[]"), "VoucherRules");

            var response = await _httpClient.PostAsync("api/ManageRewards/CreateVoucher", form);
            if (response.IsSuccessStatusCode)
            {
                return Json(new { success = true, msg = "Voucher created successfully!" });
            }

            var err = await response.Content.ReadAsStringAsync();
            return Json(new { success = false, msg = err });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteVoucher(int id)
        {
            var req = CreateAdminRequest(HttpMethod.Get, $"api/ManageRewards/DeleteVoucher/{id}");
            if (req == null) return Json(new { success = false, msg = "Unauthorized session." });

            var response = await _httpClient.SendAsync(req);
            if (response.IsSuccessStatusCode)
            {
                return Json(new { success = true, msg = "Voucher deleted successfully." });
            }

            return Json(new { success = false, msg = "Failed to delete voucher." });
        }

        // ==========================================
        // A6: MANAGE HIGHLIGHTS
        // ==========================================
        public async Task<IActionResult> Highlights()
        {
            var req = CreateAdminRequest(HttpMethod.Get, "api/ManageHighlight/GetAllHighlight");
            if (req == null) return RedirectToAction("AdminLogin", "Account");

            var response = await _httpClient.SendAsync(req);
            var highlights = response.IsSuccessStatusCode
                ? await response.Content.ReadFromJsonAsync<List<HighlightDTO>>() ?? new List<HighlightDTO>()
                : new List<HighlightDTO>();

            return View(highlights);
        }

        [HttpPost]
        public async Task<IActionResult> CreateHighlight(CreateHighlightVM model)
        {
            var req = CreateAdminRequest(HttpMethod.Post, "api/ManageHighlight/CreateHighlight");
            if (req == null) return Json(new { success = false, msg = "Unauthorized session." });

            using var form = new MultipartFormDataContent();
            form.Add(new StringContent(model.Title), "Title");
            form.Add(new StringContent(model.SubTitle ?? ""), "SubTitle");
            form.Add(new StringContent(model.Description ?? ""), "Description");
            form.Add(new StringContent(model.TnC ?? ""), "TnC");
            form.Add(new StringContent(model.CreateDate.ToString("yyyy-MM-ddTHH:mm:ss")), "CreateDate");
            form.Add(new StringContent(model.ExpireDate.ToString("yyyy-MM-ddTHH:mm:ss")), "ExpireDate");

            var response = await _httpClient.PostAsync("api/ManageHighlight/CreateHighlight", form);
            if (response.IsSuccessStatusCode)
            {
                return Json(new { success = true, msg = "Highlight banner created successfully!" });
            }

            var err = await response.Content.ReadAsStringAsync();
            return Json(new { success = false, msg = err });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteHighlight(int id)
        {
            var req = CreateAdminRequest(HttpMethod.Get, $"api/ManageHighlight/DeleteHighlight/{id}");
            if (req == null) return Json(new { success = false, msg = "Unauthorized session." });

            var response = await _httpClient.SendAsync(req);
            if (response.IsSuccessStatusCode)
            {
                return Json(new { success = true, msg = "Highlight deleted successfully." });
            }

            return Json(new { success = false, msg = "Failed to delete highlight." });
        }

        // ==========================================
        // A8: POINT RECORDS & OUTLET ASSIGN
        // ==========================================
        public async Task<IActionResult> PointRecords(int? outletId)
        {
            var endpoint = outletId.HasValue && outletId.Value > 0
                ? $"api/ManagePoint/GetAllPointAssignRecordByOutletId/{outletId.Value}"
                : "api/ManagePoint/GetAllPointAssignRecord";

            var req = CreateAdminRequest(HttpMethod.Get, endpoint);
            if (req == null) return RedirectToAction("AdminLogin", "Account");

            var response = await _httpClient.SendAsync(req);
            var records = response.IsSuccessStatusCode
                ? await response.Content.ReadFromJsonAsync<List<PointRecordDTO>>() ?? new List<PointRecordDTO>()
                : new List<PointRecordDTO>();

            // Outlets for filter
            var outletsResp = await _httpClient.SendAsync(CreateAdminRequest(HttpMethod.Get, "api/Outlet/GetAllOutlets")!);
            var outlets = outletsResp.IsSuccessStatusCode
                ? await outletsResp.Content.ReadFromJsonAsync<List<OutletDTO>>() ?? new List<OutletDTO>()
                : new List<OutletDTO>();

            ViewBag.Outlets = outlets;
            ViewBag.SelectedOutlet = outletId;

            return View(records);
        }

        [HttpPost]
        public async Task<IActionResult> OutletAssignPoint(OutletAssignRecordVM model)
        {
            var req = CreateAdminRequest(HttpMethod.Post, "api/ManagePoint/OutletAssignPoint");
            if (req == null) return Json(new { success = false, msg = "Unauthorized session." });

            req.Content = JsonContent.Create(model);

            var response = await _httpClient.SendAsync(req);
            if (response.IsSuccessStatusCode)
            {
                return Json(new { success = true, msg = "Points directly credited to member account!" });
            }

            var err = await response.Content.ReadAsStringAsync();
            return Json(new { success = false, msg = err });
        }

        // ==========================================
        // A9: REDEMPTION RECORDS (REWARDS & VOUCHERS)
        // ==========================================
        public async Task<IActionResult> RedemptionRecords()
        {
            var reqVoucher = CreateAdminRequest(HttpMethod.Get, "api/ManageRecords/GetAllVoucherAssignRecord");
            if (reqVoucher == null) return RedirectToAction("AdminLogin", "Account");

            var voucherResp = await _httpClient.SendAsync(reqVoucher);
            var vouchers = voucherResp.IsSuccessStatusCode
                ? await voucherResp.Content.ReadFromJsonAsync<List<VoucherAssignDTO>>() ?? new List<VoucherAssignDTO>()
                : new List<VoucherAssignDTO>();

            var reqReward = CreateAdminRequest(HttpMethod.Get, "api/ManageRecords/GetAllRewardAssignRecord");
            var rewardResp = await _httpClient.SendAsync(reqReward!);
            var rewards = rewardResp.IsSuccessStatusCode
                ? await rewardResp.Content.ReadFromJsonAsync<List<RewardAssignDTO>>() ?? new List<RewardAssignDTO>()
                : new List<RewardAssignDTO>();

            ViewBag.VoucherRedemptions = vouchers;
            ViewBag.RewardRedemptions = rewards;

            return View();
        }

        // ==========================================
        // A10: CUSTOMER FEEDBACK OVERSIGHT
        // ==========================================
        public async Task<IActionResult> Feedback()
        {
            var req = CreateAdminRequest(HttpMethod.Get, "api/Feedback/GetAllFeedbackList");
            if (req == null) return RedirectToAction("AdminLogin", "Account");

            var response = await _httpClient.SendAsync(req);
            var feedbacks = response.IsSuccessStatusCode
                ? await response.Content.ReadFromJsonAsync<List<FeedbackDTO>>() ?? new List<FeedbackDTO>()
                : new List<FeedbackDTO>();

            return View(feedbacks);
        }
    }
}
