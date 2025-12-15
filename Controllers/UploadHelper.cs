using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace SystemeNote.Utils
{
    public static class UploadHelper
    {
        public static async Task<string> ProcessUpload(IFormFile file, DbContext context, Func<string[], Task> processRow)
        {
            if (file == null || file.Length == 0)
            {
                return "Veuillez sélectionner un fichier.";
            }

            var importedCount = 0;
            var errors = new StringBuilder();
            var lineNumber = 1;

            try
            {
                using var reader = new StreamReader(file.OpenReadStream());
                await reader.ReadLineAsync(); // Ignorer l'en-tête

                while (!reader.EndOfStream)
                {
                    lineNumber++;
                    var line = await reader.ReadLineAsync();
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    var cols = line.Split(',').Select(c => c.Trim(' ', '"')).ToArray();

                    try { await processRow(cols); importedCount++; }
                    catch (Exception ex) { errors.AppendLine($"Ligne {lineNumber}: {ex.Message}"); }
                }

                if (errors.Length == 0) { await context.SaveChangesAsync(); return $"Importation terminée. {importedCount} ligne(s) traitée(s) avec succès."; }

                return $"Importation terminée avec des erreurs. {importedCount} ligne(s) traitée(s) mais non sauvegardée(s). Erreurs : <br/>{errors.ToString().Replace("\n", "<br/>")}";
            }
            catch (Exception ex) { return "L'importation a échoué : " + ex.Message; }
        }
    }
}