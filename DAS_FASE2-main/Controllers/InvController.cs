using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace RotulosRanibal.Controllers
{
    public class InventarioController : Controller
    {
     
        public IActionResult Index()
        {
            return View();
        }

        
        [HttpGet]
        public IActionResult Nuevo ()
        {
            return View();
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddInven(string codigo, string material, int existencias, string unidad, decimal? costo, string descripcion)
        {
   

            TempData["MensajeExito"] = "¡Material agregado correctamente al inventario!";
            return RedirectToAction(nameof(Index));
        }

        
        [HttpGet]
        public IActionResult EditarMaterial(string id)
        {
            var listaInventario = new List<Dictionary<string, string>>
            {
                new Dictionary<string, string> { { "Codigo", "MAT-001" }, { "Nombre", "Lámina acrílica" }, { "Existencias", "25" }, { "Unidad", "Unidad" }, { "Costo", "12.50" }, { "Descripcion", "Lámina acrílica transparente 3mm" } },
                new Dictionary<string, string> { { "Codigo", "MAT-004" }, { "Nombre", "Lámina acrílica" }, { "Existencias", "25" }, { "Unidad", "Unidad" }, { "Costo", "0.00" }, { "Descripcion", "Descripción breve del material" } }
            };

            var materialSeleccionado = listaInventario.FirstOrDefault(m => m["Codigo"] == id);

            if (materialSeleccionado == null)
            {
                return NotFound();
            }

            ViewBag.Material = materialSeleccionado;
            return View();
        }

      
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ActualizarMaterial(string id, string codigo, string material, int existencias, string unidad, decimal? costo, string descripcion)
        {
            TempData["MensajeExito"] = "¡Material actualizado correctamente!";

            return RedirectToAction(nameof(Index));
        }
    }
}