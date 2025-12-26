Placez ici l'exécutable wkhtmltopdf et les DLL associées

Instructions rapides :

1. Téléchargez wkhtmltopdf pour Windows depuis :
   https://wkhtmltopdf.org/downloads.html

2. Ouvrez l'archive téléchargée (généralement un zip ou un installeur). Si c'est un installeur `.msi`/`.exe`, installez-le puis copiez `wkhtmltopdf.exe` et les DLL nécessaires depuis l'installation (par exemple `C:\Program Files\wkhtmltopdf\bin`) vers ce dossier.

3. Le dossier attendu dans ce projet est : `wwwroot/Rotativa` (chemin relatif au projet). Exemples de fichiers nécessaires : `wkhtmltopdf.exe`, `libwkhtmltox.dll`, etc.

4. Redémarrez l'application :

```powershell
dotnet build "F:/GITHUB/SystemeNote/SystemeNote.csproj"
dotnet run --project "F:/GITHUB/SystemeNote/SystemeNote.csproj"
```

Si vous souhaitez que je tente de télécharger automatiquement les binaires dans ce dossier, autorisez-moi explicitement — je peux exécuter une commande PowerShell pour récupérer une release publique, mais cela nécessite Internet et peut échouer selon la disponibilité des builds.
