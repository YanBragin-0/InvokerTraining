using InvokerTraining.Application.Abstractions;
using InvokerTraining.Application.DataTransfers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InvokerTraining.Controllers
{
    public class AccountController : Controller
    {
        [AllowAnonymous] 
        public IActionResult Login()
        {
            return View(); // Views/Account/Login.cshtml
        }

        [AllowAnonymous]
        public IActionResult Register()
        {
            return View(); // Views/Account/Register.cshtml
        }
    }
}
