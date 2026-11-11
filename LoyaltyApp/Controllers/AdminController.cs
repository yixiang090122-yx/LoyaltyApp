using Microsoft.AspNetCore.Mvc;

namespace LoyaltyApp.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
