
using System.Security.Claims;
using DAS_Proyecto.Models;
using DAS_Proyecto.Repositories.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DAS_Proyecto.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IPasswordHasher<Usuario> _passwordHasher;

        public AccountController(
            IUsuarioRepository usuarioRepository,
            IPasswordHasher<Usuario> passwordHasher)
        {
            _usuarioRepository = usuarioRepository;
            _passwordHasher = passwordHasher;
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            if (User.Identity?.IsAuthenticated == true)
                return RedirectToAction("Index", "Home");

            ViewData["ReturnUrl"] = returnUrl;
            return View("~/Views/Home/Login.cshtml");
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(
            string usuario,
            string contrasena,
            string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (string.IsNullOrWhiteSpace(usuario) ||
                string.IsNullOrWhiteSpace(contrasena))
            {
                ViewBag.MensajeError = "Ingresa tu correo y contraseña.";
                return View("~/Views/Home/Login.cshtml");
            }

            var cuenta = await _usuarioRepository.ObtenerPorCorreoAsync(
                usuario.Trim());

            if (cuenta == null)
            {
                ViewBag.MensajeError = "Correo o contraseña incorrectos.";
                return View("~/Views/Home/Login.cshtml");
            }

            var resultado = _passwordHasher.VerifyHashedPassword(
                cuenta,
                cuenta.PasswordHash,
                contrasena);

            if (resultado == PasswordVerificationResult.Failed)
            {
                ViewBag.MensajeError = "Correo o contraseña incorrectos.";
                return View("~/Views/Home/Login.cshtml");
            }

            if (resultado == PasswordVerificationResult.SuccessRehashNeeded)
            {
                cuenta.PasswordHash =
                    _passwordHasher.HashPassword(cuenta, contrasena);

                await _usuarioRepository.ActualizarAsync(cuenta);
            }

            var reclamos = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier,
                    cuenta.IdUsuario.ToString()),

                new Claim(ClaimTypes.Name, cuenta.Nombre),

                new Claim(ClaimTypes.Email, cuenta.Correo),

                new Claim(ClaimTypes.Role, cuenta.Rol.ToString())
            };

            var identidad = new ClaimsIdentity(
                reclamos,
                CookieAuthenticationDefaults.AuthenticationScheme);

            var principal = new ClaimsPrincipal(identidad);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                new AuthenticationProperties
                {
                    IsPersistent = false,
                    AllowRefresh = true
                });

            if (!string.IsNullOrWhiteSpace(returnUrl) &&
                Url.IsLocalUrl(returnUrl))
            {
                return LocalRedirect(returnUrl);
            }

            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction(nameof(Login));
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return Content(
                "No tienes permisos para acceder a esta sección.");
        }
    }
}
