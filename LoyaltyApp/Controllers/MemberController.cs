using LoyaltyApp.DTOs;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace LoyaltyApp.Controllers
{
    public class MemberController : Controller
    {
        private readonly HttpClient _httpClient;

        public MemberController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("LoyaltyAppClient");
        }

        private HttpRequestMessage? CreateAuthorizedRequest(HttpMethod method, string requestUri)
        {
            var token = HttpContext.Session.GetString("TOKEN");
            if (string.IsNullOrEmpty(token)) return null;

            var request = new HttpRequestMessage(method, requestUri);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return request;
        }

        // ==========================================
        // C5: HOME / DASHBOARD
        // ==========================================
        public async Task<IActionResult> Index()
        {
            var req = CreateAuthorizedRequest(HttpMethod.Post, "api/Member/FindMemberDetails");
            if (req == null) return RedirectToAction("MemberLogin", "Account");
            req.Content = new StringContent("{}", Encoding.UTF8, "application/json");

            var memberResp = await _httpClient.SendAsync(req);
            if (!memberResp.IsSuccessStatusCode)
            {
                if (memberResp.StatusCode == HttpStatusCode.Unauthorized)
                {
                    HttpContext.Session.Clear();
                    return RedirectToAction("MemberLogin", "Account");
                }
            }

            var memberData = memberResp.IsSuccessStatusCode
                ? await memberResp.Content.ReadFromJsonAsync<MemberDTO>(new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                : new MemberDTO();

            // Fetch Highlights / Promotions
            var highlightsResp = await _httpClient.GetAsync("api/MemberHightlight/GetAllHighlights");
            var highlights = highlightsResp.IsSuccessStatusCode
                ? await highlightsResp.Content.ReadFromJsonAsync<List<HighlightDTO>>() ?? new List<HighlightDTO>()
                : new List<HighlightDTO>();

            ViewBag.Member = memberData;
            ViewBag.Highlights = highlights;

            return View(memberData);
        }

        // ==========================================
        // C4: MY PROFILE & CHANGE PASSWORD
        // ==========================================
        public async Task<IActionResult> Details()
        {
            var req = CreateAuthorizedRequest(HttpMethod.Post, "api/Member/FindMemberDetails");
            if (req == null) return RedirectToAction("MemberLogin", "Account");
            req.Content = new StringContent("{}", Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(req);
            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    HttpContext.Session.Clear();
                    return RedirectToAction("MemberLogin", "Account");
                }
                return RedirectToAction(nameof(Index));
            }

            var memberData = await response.Content.ReadFromJsonAsync<MemberDTO>(new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return View(memberData);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProfile(UpdateProfileVM model)
        {
            var req = CreateAuthorizedRequest(HttpMethod.Post, "api/Member/MemberUpdateProfile");
            if (req == null) return Json(new { success = false, msg = "Unauthorized session. Please login." });

            req.Content = JsonContent.Create(new
            {
                Name = model.Name,
                Email = model.Email
            });

            var response = await _httpClient.SendAsync(req);
            if (response.IsSuccessStatusCode)
            {
                return Json(new { success = true, msg = "Profile updated successfully!" });
            }

            var err = await response.Content.ReadAsStringAsync();
            return Json(new { success = false, msg = err });
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordVM model)
        {
            var req = CreateAuthorizedRequest(HttpMethod.Post, "api/Member/ChangePassword");
            if (req == null) return Json(new { success = false, msg = "Unauthorized session. Please login." });

            req.Content = JsonContent.Create(new
            {
                OldPassword = model.OldPassword,
                NewPassword = model.NewPassword
            });

            var response = await _httpClient.SendAsync(req);
            if (response.IsSuccessStatusCode)
            {
                return Json(new { success = true, msg = "Password changed successfully!" });
            }

            var err = await response.Content.ReadAsStringAsync();
            return Json(new { success = false, msg = err });
        }

        // ==========================================
        // C6: REWARDS CATALOG & REDEEM
        // ==========================================
        public async Task<IActionResult> Rewards()
        {
            var req = CreateAuthorizedRequest(HttpMethod.Post, "api/Member/FindMemberDetails");
            if (req == null) return RedirectToAction("MemberLogin", "Account");
            req.Content = new StringContent("{}", Encoding.UTF8, "application/json");

            var memberResp = await _httpClient.SendAsync(req);
            var memberData = memberResp.IsSuccessStatusCode
                ? await memberResp.Content.ReadFromJsonAsync<MemberDTO>(new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                : new MemberDTO();

            var rewardsResp = await _httpClient.GetAsync("api/MemberReward/GetAllRewards");
            var rewards = rewardsResp.IsSuccessStatusCode
                ? await rewardsResp.Content.ReadFromJsonAsync<List<RewardDTO>>() ?? new List<RewardDTO>()
                : new List<RewardDTO>();

            ViewBag.Member = memberData;
            return View(rewards);
        }

        [HttpPost]
        public async Task<IActionResult> RedeemReward(RedeemRewardVM model)
        {
            var req = CreateAuthorizedRequest(HttpMethod.Post, "api/MemberReward/RedeemReward");
            if (req == null) return Json(new { success = false, msg = "Unauthorized session. Please login." });

            req.Content = JsonContent.Create(model);

            var response = await _httpClient.SendAsync(req);
            if (response.IsSuccessStatusCode)
            {
                return Json(new { success = true, msg = "Reward redeemed successfully!" });
            }

            var err = await response.Content.ReadAsStringAsync();
            return Json(new { success = false, msg = string.IsNullOrWhiteSpace(err) ? "Redemption failed." : err });
        }

        // ==========================================
        // C7: VOUCHERS & IN-STORE REDEEM
        // ==========================================
        public async Task<IActionResult> Vouchers()
        {
            var req = CreateAuthorizedRequest(HttpMethod.Get, "api/MemberVoucher/GetMemberUnclaimedVouchers");
            if (req == null) return RedirectToAction("MemberLogin", "Account");

            var response = await _httpClient.SendAsync(req);
            var vouchers = response.IsSuccessStatusCode
                ? await response.Content.ReadFromJsonAsync<List<VoucherDTO>>() ?? new List<VoucherDTO>()
                : new List<VoucherDTO>();

            return View(vouchers);
        }

        [HttpPost]
        public async Task<IActionResult> RedeemVoucher(RedeemVoucherVM model)
        {
            var req = CreateAuthorizedRequest(HttpMethod.Post, "api/MemberVoucher/RedeemVoucher");
            if (req == null) return Json(new { success = false, msg = "Unauthorized session. Please login." });

            req.Content = JsonContent.Create(model);

            var response = await _httpClient.SendAsync(req);
            if (response.IsSuccessStatusCode)
            {
                return Json(new { success = true, msg = "Voucher claimed and redeemed successfully!" });
            }

            var err = await response.Content.ReadAsStringAsync();
            return Json(new { success = false, msg = string.IsNullOrWhiteSpace(err) ? "Failed to redeem voucher." : err });
        }

        // ==========================================
        // C8: COLLECT POINTS & CASHIER APPROVAL
        // ==========================================
        public async Task<IActionResult> CollectPoints()
        {
            var token = HttpContext.Session.GetString("TOKEN");
            if (string.IsNullOrEmpty(token)) return RedirectToAction("MemberLogin", "Account");

            // Fetch list of outlets for branch selection
            var outletsResp = await _httpClient.GetAsync("api/MemberOutlet/GetAllOutlets");
            var outlets = outletsResp.IsSuccessStatusCode
                ? await outletsResp.Content.ReadFromJsonAsync<List<OutletDTO>>() ?? new List<OutletDTO>()
                : new List<OutletDTO>();

            ViewBag.Outlets = outlets;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RequestPointCode(AssignPointVM model)
        {
            var req = CreateAuthorizedRequest(HttpMethod.Post, "api/MemberPoint/AssignPoint");
            if (req == null) return Json(new { success = false, msg = "Unauthorized session. Please login." });

            req.Content = JsonContent.Create(new { Point = model.Point });

            var response = await _httpClient.SendAsync(req);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<AssignPointVM>();
                return Json(new { success = true, code = result?.Code, point = model.Point });
            }

            var err = await response.Content.ReadAsStringAsync();
            return Json(new { success = false, msg = err });
        }

        [HttpPost]
        public async Task<IActionResult> ApprovePoints(ApprovePointVM model)
        {
            var req = CreateAuthorizedRequest(HttpMethod.Post, "api/MemberPoint/ApprovePoint");
            if (req == null) return Json(new { success = false, msg = "Unauthorized session. Please login." });

            req.Content = JsonContent.Create(new
            {
                Code = model.Code,
                OutletGUID = model.OutletGUID,
                Pin = model.Pin,
                PointRecordStatus = 1 // Approve
            });

            var response = await _httpClient.SendAsync(req);
            if (response.IsSuccessStatusCode)
            {
                return Json(new { success = true, msg = "Points approved and credited to your account!" });
            }

            var err = await response.Content.ReadAsStringAsync();
            return Json(new { success = false, msg = string.IsNullOrWhiteSpace(err) ? "Approval failed. Please check the staff PIN." : err });
        }

        // ==========================================
        // C9: TRANSACTION HISTORY
        // ==========================================
        public async Task<IActionResult> History()
        {
            var req = CreateAuthorizedRequest(HttpMethod.Get, "api/MemberHistory/GetHistoryRecord");
            if (req == null) return RedirectToAction("MemberLogin", "Account");

            var response = await _httpClient.SendAsync(req);
            var historyRecords = response.IsSuccessStatusCode
                ? await response.Content.ReadFromJsonAsync<List<HistoryRecordDTO>>() ?? new List<HistoryRecordDTO>()
                : new List<HistoryRecordDTO>();

            return View(historyRecords);
        }

        // ==========================================
        // C11: CUSTOMER FEEDBACK
        // ==========================================
        public async Task<IActionResult> Feedback()
        {
            var token = HttpContext.Session.GetString("TOKEN");
            if (string.IsNullOrEmpty(token)) return RedirectToAction("MemberLogin", "Account");

            var outletsResp = await _httpClient.GetAsync("api/MemberOutlet/GetAllOutlets");
            var outlets = outletsResp.IsSuccessStatusCode
                ? await outletsResp.Content.ReadFromJsonAsync<List<OutletDTO>>() ?? new List<OutletDTO>()
                : new List<OutletDTO>();

            ViewBag.Outlets = outlets;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SubmitFeedback(CreateFeedbackVM model)
        {
            var req = CreateAuthorizedRequest(HttpMethod.Post, "api/MemberFeedback/CreateFeedback");
            if (req == null) return Json(new { success = false, msg = "Unauthorized session. Please login." });

            req.Content = JsonContent.Create(model);

            var response = await _httpClient.SendAsync(req);
            if (response.IsSuccessStatusCode)
            {
                return Json(new { success = true, msg = "Thank you! Your feedback has been submitted to management." });
            }

            var err = await response.Content.ReadAsStringAsync();
            return Json(new { success = false, msg = string.IsNullOrWhiteSpace(err) ? "Failed to submit feedback." : err });
        }
    }
}
