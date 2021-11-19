using Microsoft.AspNetCore.Mvc;
using Services.API;
using System.Diagnostics;
using System.Linq;
using WebUi.Hubs;
using WebUi.Models;

namespace WebUi.Controllers
{
    public class HomeController : Controller
    {
        private readonly IUserService _userService;

        public HomeController(IUserService userService)
        {
            this._userService = userService;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Game()
        {
            return View();
        }
        public IActionResult HowToPlay()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Chat(string chatTarget)
        {
            if (chatTarget == Request.Cookies["UserName"])
            {
                TempData.Add("TargetCantBeSelf", true);
                return RedirectToAction("Index");
            }
            if (chatTarget is null)
                return View("Chat", "Public"); // public chat

            if (ChatHub.Connections.TryGetValue(chatTarget, out _))
                return View("Chat", chatTarget); // private chat
            else // maybe logged out user
            {
                if (_userService.GetAllUserNamesAsync().Result.Contains(chatTarget))
                {
                    //TempData.Add("loggedOut", true);
                    return View("Chat", chatTarget); // private chat
                }
            }

            TempData.Add("notFound", true);
            return RedirectToAction("Index");  // no chat found



        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
