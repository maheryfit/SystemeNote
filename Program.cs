using Microsoft.EntityFrameworkCore;
using SystemeNote.Data;
using Rotativa.AspNetCore;
using System.IO;

var builder = WebApplication.CreateBuilder(args);

var conn = builder.Configuration.GetConnectionString("DefaultConnection")
           ?? builder.Configuration["ConnectionStrings:DefaultConnection"];

builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlServer(conn, sqlOptions =>
        sqlOptions.EnableRetryOnFailure()
    )
);

// Add services to the container.
builder.Services.AddRazorPages();

var app = builder.Build();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);
app.MapRazorPages();

// Configure Rotativa (wkhtmltopdf) path: expect binaries under wwwroot/Rotativa
var webRoot = app.Environment.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
var rotativaFolder = Path.Combine(webRoot, "Rotativa");
if (!Directory.Exists(rotativaFolder)) Directory.CreateDirectory(rotativaFolder);

// Expected executable name for Windows
var wkexe = Path.Combine(rotativaFolder, "wkhtmltopdf.exe");
if (!System.IO.File.Exists(wkexe))
{
    Console.WriteLine("Rotativa: wkhtmltopdf.exe not found in: " + rotativaFolder);
    Console.WriteLine("Please download wkhtmltopdf for Windows and place wkhtmltopdf.exe and required DLLs in the folder above.");
    Console.WriteLine("See https://wkhtmltopdf.org/downloads.html");
}

try
{
    // second parameter is the relative folder name under webroot where wkhtmltopdf binaries live
    RotativaConfiguration.Setup(webRoot, "Rotativa");
    Console.WriteLine("Rotativa configured with folder: " + rotativaFolder);
}
catch (Exception ex)
{
    // If Rotativa fails to initialize, do not crash the app at startup — log to console for diagnosis
    Console.WriteLine("Rotativa initialization warning: " + ex.Message);
}

app.Run();
