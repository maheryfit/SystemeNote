# University Grading System (.NET Core)

A university **grading management** web application built as a two-person university project (binôme).  
It covers the full flow from academic structure setup (diplomas, semesters, UEs, subjects) to student grades, rankings, dashboards, and exports (Excel/PDF).

> **Tech stack:** .NET Core (MVC + Razor Pages) + SQL Server + Entity Framework Core  
> **Deployment:** CI/CD with GitHub Actions to a DigitalOcean Droplet (GitHub Education)

---

## 🎯 Features

### 🏢 Admin / Professor Side (MVC Controllers)
- **Manage Academic Structure:**
  - **Diplomas:** Licence, Master, etc.
  - **Promotions:** Cohorts attached to specific diplomas.
  - **Semesters & Options:** Semesters (S1–S4/S6) and options (Dev / DB / Web & Design).
  - **Semester Planning (`PlanifSemestre`):** Define the start and end dates of a semester for a specific promotion and option.
  - **Curriculum (`ParcoursEtude`):** Define Teaching Units (UEs) and Subjects (Matières), assign them to a semester plan, and allocate credits.
- **Student Management:**
  - Create students (matricule, identity, DOB).
  - Enroll students in specific semester planning (`HistoriqueSemestreEtudiant`).
  - View full history of student semesters, grades, UE averages, and academic ranking.
- **Grades Management:**
  - Manual entry of notes (matricule, planning semester, subject, grade).
  - **Import via files:** Batch grade uploads using Excel/CSV.
- **Analytics / Dashboards:**
  - Pass/Fail stats (Admis vs. Ajourné) per semester planning and promotion.
  - Variance and standard deviation of class grades.
- **Exports:**
  - **PDF Export:** Generate student rankings and scoreboards as PDF documents (powered by Rotativa / `wkhtmltopdf`).
  - Excel/CSV lists of student rankings and averages using *EPPlus*.

### 🎓 Student Side (Razor Pages)
- **Secure Authentication:** Identity verification via student Matricule and credentials.
- **Personal Dashboard (`DashboardEtudiants`):**
  - View enrolled semesters, current UE status (Admis/Ajourné).
  - Track individual subject (`Matière`) grades and overall module (`UE`) averages.
  - **Progress Analytics:** Compare Semester vs. Prior Semester averages with variance metrics.
- **Self-Service Export:**
  - Download official personal transcript (Relevé de notes) as a dynamically generated **PDF**.

---

## 🛠 Tech Stack

- **Backend / Web Framework:** ASP.NET Core 8 (Mix of MVC for Admins and Razor Pages for Students)
- **ORM:** Entity Framework Core (SQL Server)
- **Authentication:** Cookie Authentication (`Microsoft.AspNetCore.Authentication.Cookies`)
- **PDF Generation:** Rotativa.AspNetCore
- **Excel/Data Processing:** EPPlus
- **Database:** Microsoft SQL Server (Containerized via Docker)
- **CI/CD:** GitHub Actions
- **Hosting / Deploy:** DigitalOcean Droplet (via GitHub Education)

---

## 🚀 Local Development Setup

### Prerequisites
1. **Docker / Docker Desktop** (for the database)
2. **.NET 8 SDK**
3. **wkhtmltopdf** (for PDF generation)

### 1. Database Setup (Docker)
The Microsoft SQL Server runs in a Docker container.
```bash
# Start the SQL Server container
docker-compose up -d
```
The database connection string is already configured in `appsettings.json` to target `localhost,1433` with user `sa` and password `SYSTEMENOTEbdd??2025`.

### 2. Apply EF Core Migrations
Update the database schema to the latest state:
```bash
dotnet ef database update
```

### 3. Configure Rotativa (PDF Engine)
PDF generation relies on `wkhtmltopdf`.
1. Download the executable from [wkhtmltopdf.org](https://wkhtmltopdf.org/downloads.html).
2. Create the folder `wwwroot/Rotativa` at the root of the project.
3. Place `wkhtmltopdf.exe` and its required `.dll` files inside `wwwroot/Rotativa`.
> *Note: If not configured, the app will log a warning at startup but will not crash, though PDF downloads will fail.*

### 4. Run the Project
```bash
dotnet run
```
The application runs typically at `http://localhost:5026`.

---

## 📂 Project Architecture

- **`/Controllers` & `/Views`:** Core MVC architecture tailored to Administrative and Professor actions. Contains CRUD for all structural entities (`PlanifSemestresController`, `EtudiantsController`, etc.)
- **`/Pages`:** Razor Pages architecture strictly used for Student-facing interfaces (`/DashboardEtudiants`). Designed for a focused user flow.
- **`/Models`:** Entity Framework Core Code-First models representing database tables (`Etudiant`, `Diplome`, `UniteEnseignement`, `ParcoursEtude`, `NoteEtudiant`, etc.).
- **`/ViewModels`:** Specific view data structures utilized for mapping analytics and PDFs (`PdfReportViewModel`, `SemesterRankingViewModel`).
- **`Program.cs`:** Dependency Injection, Kestrel config, Cookie Authentication pipeline, and Rotativa setup.

---

## 🔄 Project Workflow (How it was built)

1. Imagine a project scenario.
2. Write the user story (text limitations and needs).
3. UI/UX analysis: derive final screens + describe the UI.
4. Database design: define tables, relations, and entity attributes.
5. Task assignment (2 developers).
6. Bootstrap solution template + push to GitHub.
7. Prepare the DigitalOcean Droplet (Docker + SQL Server + runner).
8. Write the GitHub Actions YAML for automatic deployment on:
   - push to `main`
   - merge into `main`
9. **Iterative Development:**
   - Establish Core MVC Architecture for Back-Office.
   - Implement EF Core relationships for deep relational grades mapping.
   - Implement Student Dashboard as isolated Razor Pages.
   - Refine PDF and Data processing flows.
