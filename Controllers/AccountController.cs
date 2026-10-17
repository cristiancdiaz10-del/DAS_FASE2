using Microsoft.AspNetCore.Mvc;

namespace DAS_Proyecto.Controllers
{
    public class AccountController : Controller
    {
      
        [HttpGet]
        public IActionResult Login()
        {
            return View("~/Views/Home/Login.cshtml");
        }

     
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(string usuario, string contrasena)
        {
            
            return RedirectToAction("Cotizaciones", "Home");
        }
    }
}