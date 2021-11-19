using Microsoft.AspNetCore.Mvc;
using Models;
using Services.API;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebUi.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private static List<string> _loggedList;
        public static List<string> LoggedList
        {
            get
            {
                if (_loggedList is null)
                    _loggedList = new List<string>();
                return _loggedList;
            }
            set { _loggedList = value; }
        }
        public List<string> GetLoggedIn() => LoggedList;
        public async Task<List<string>> GetLoggedOut()
        {
            var allUserNames = await _userService.GetAllUserNamesAsync();
            List<string> loggedOut = new List<string>();
            foreach (var userName in allUserNames)
            {
                if (!LoggedList.Contains(userName))
                    loggedOut.Add(userName);
            }
            return loggedOut;
        }

        public UserController(IUserService userService)
        {
            _userService = userService;
        }
        public ActionResult Login()
        {
            return View();
        }


        [HttpPost]
        public async Task<ActionResult> Login(User user)
        {
            var res = await _userService.LoginAsync(user);
            if (res)
            {
                if (LoggedList.Contains(user.UserName))
                {
                    TempData.Add("SomeErrorWasMade", true);
                    return RedirectToAction("Index", "Home");
                }
                Response.Cookies.Append("UserName", user.UserName);
                if (!LoggedList.Contains(user.UserName))
                    LoggedList.Add(user.UserName);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                TempData.Add("notFound", true);
                return View(user);
            }

        }
        public ActionResult Logout()
        {
            string username = Request.Cookies["UserName"];
            Response.Cookies.Delete("UserName");
            if (LoggedList.Contains(username))
                LoggedList.Remove(username);
            return Redirect("/");
        }
        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> Register(User user)
        {
            if (user != null)
            {
                if (string.IsNullOrEmpty(user.UserName) || string.IsNullOrEmpty(user.Password))
                {
                    TempData.Add("emptyUser", true);
                    return View(user);
                }
            }
            var res = await _userService.RegisterAsync(user);
            if (res)
            {
                Response.Cookies.Append("UserName", user.UserName);
                if (!LoggedList.Contains(user.UserName))
                    LoggedList.Add(user.UserName);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                TempData.Add("cantRegister", true);
                return View(user);
            }
        }
    }
}
