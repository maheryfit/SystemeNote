using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

namespace SystemeNote.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly string _connectionString;

        public LoginModel(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string not found.");
        }

        public sealed class InputModel
        {
            [Required]
            public string Username { get; set; } = string.Empty; // Matricule

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; } = string.Empty; // MotDePasse

            public bool RememberMe { get; set; }
        }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public string? ReturnUrl { get; set; }

        public string? ErrorMessage { get; set; }

        public void OnGet()
        {
            ReturnUrl ??= Url.Content("~/DashboardEtudiants");
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            (bool valid, int? etudiantId, string? matricule) = await ValidateEtudiantAsync(Input.Username, Input.Password);

            if (!valid || etudiantId is null || string.IsNullOrWhiteSpace(matricule))
            {
                ErrorMessage = "Identifiants invalides.";
                return Page();
            }

            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, matricule),
                new(ClaimTypes.NameIdentifier, etudiantId.Value.ToString()),
                new(ClaimTypes.Role, "Etudiant")
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            var authProps = new AuthenticationProperties
            {
                IsPersistent = Input.RememberMe,
                RedirectUri = ReturnUrl
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, authProps);

            return LocalRedirect(ReturnUrl ?? Url.Content("~/DashboardEtudiants"));
        }

        private async Task<(bool valid, int? etudiantId, string? matricule)> ValidateEtudiantAsync(string matricule, string motDePasse)
        {
            // NOTE: comparaison directe (mot de passe en clair) car ta DB stocke mot_de_passe en nvarchar(255).
            // Si tu hashes ensuite, on changera cette méthode.

            const string sql = @"
                SELECT TOP (1) Id, matricule
                FROM etudiant
                WHERE matricule = @matricule
                  AND mot_de_passe = @motDePasse
                  AND is_actif = 1;";
            int id;
            string mat;
            await using(var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                await using var command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@matricule", matricule);
                command.Parameters.AddWithValue("@motDePasse", motDePasse);

                await using var reader = await command.ExecuteReaderAsync();

                if (!await reader.ReadAsync())
                {
                    return (false, null, null);
                }

                id = reader.GetInt32(0);
                mat = reader.GetString(1);
            }
            return (true, id, mat);
        }
    }
}