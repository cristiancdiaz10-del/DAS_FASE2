using Microsoft.AspNetCore.Mvc;

namespace RotulosRanibal.Controllers
{
    public class VentasController : Controller
    {
      
        public IActionResult Index()
        {
            return View();
        }
    }
}