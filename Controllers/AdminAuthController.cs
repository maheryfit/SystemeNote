using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SystemeNote.Data;

namespace SystemeNote.Controllers
{
    public class AdminAuthController : Controller
    {
        private readonly AppDbContext _context;

        public AdminAuthController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? returnUrl = null)
        {
            var admins = await _context.Administrateurs
                .AsNoTracking()
                .OrderBy(a => a.PrenomAdmin)
                .ThenBy(a => a.NomAdmin)
                .Select(a => new SelectListItem
                {
                    Value = a.Id.ToString(),
                    Text = a.PrenomAdmin + " " + a.NomAdmin
                })
                .ToListAsync();

            ViewBag.Administrateurs = admins;
            ViewBag.ReturnUrl = returnUrl;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(int? administrateurId, bool rememberMe, string? returnUrl = null)
        {
            if (administrateurId is null)
            {
                ModelState.AddModelError(string.Empty, "Veuillez sélectionner un administrateur.");
                return await Index(returnUrl);
            }

            var admin = await _context.Administrateurs
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == administrateurId.Value);

            if (admin is null)
            {
                ModelState.AddModelError(string.Empty, "Administrateur invalide.");
                return await Index(returnUrl);
            }

            var displayName = $"{admin.PrenomAdmin} {admin.NomAdmin}".Trim();

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, admin.Id.ToString()),
                new(ClaimTypes.Name, displayName),
                new(ClaimTypes.Role, "Administrateur"),
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            var authProps = new AuthenticationProperties
            {
                IsPersistent = rememberMe,
                RedirectUri = returnUrl
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, authProps);

            if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Index", "Professeur");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(nameof(Index));
        }
    }
}