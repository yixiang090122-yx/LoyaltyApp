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
        HttpClient _httpClient;
        public MemberController(IHttpClientFactory httpClient)
        {
            _httpClient = httpClient.CreateClient("LoyaltyAppClient");
        }
        public IActionResult Index()
        {
            var token = HttpContext.Session.GetString("TOKEN");
            if(string.IsNullOrEmpty(token))
            {
                return RedirectToAction("MemberLogin", "Account");
            }
            return View();
        }

        public async Task<IActionResult> Details()
        {
            var token = HttpContext.Session.GetString("TOKEN");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("MemberLogin", "Account");
            }

            var request = new HttpRequestMessage(HttpMethod.Post, "api/Member/FindMemberDetails");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            request.Content = new StringContent("{}", Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(request);
            if(!response.IsSuccessStatusCode)
            {
                if(response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    //UNauthorized Error msg
                    return RedirectToAction("MemberLogin", "Account");
                }
                //Error msg API error
                return RedirectToAction(nameof(Index));
            }

            var memberData = await response.Content.ReadFromJsonAsync<MemberDTO>(new JsonSerializerOptions
            {
                // If JSON return data is {"name":"ABC", "age":2},
                // PropertyNameCaseInsensitive ignores case and automatically maps "name" to "Name".
                PropertyNameCaseInsensitive = true
            });
            //VM


            return View(memberData);
        }
    }
}
