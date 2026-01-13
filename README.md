# University Grading System (.NET Core)

A university **grading management** web application built as a two-person university project (binôme).  
It covers the full flow from academic structure setup (diplomas, semesters, UEs, subjects) to student grades, rankings, dashboards, and exports.

> Tech stack: .NET Core (MVC + Razor Pages) + SQL Server  
> Deployment: CI/CD with GitHub Actions to a DigitalOcean Droplet (GitHub Education)

---

## Features

### Admin / Professor side
- Manage academic structure:
  - Diplomas (Licence, Master)
  - Promotions (cohorts) attached to diplomas
  - Semesters (S1–S4), options (Dev / DB / Web & Design)
  - Semester planning (start/end dates) by option + promotion
  - Teaching units (UE) and subjects (matières), linked to semester planning + credits
- Student management:
  - Create students (matricule, identity, DOB, current semester/planning, promotion)
  - View student history (all semesters), grades, UE average, rank
  - Edit grades
- Grades management:
  - Manual entry (matricule, planning semester, subject, grade)
  - Import grades from CSV
- Analytics / dashboards:
  - Admitted vs. failed (ajourné/admis) per semester planning / promotion
  - Variance and standard deviation of grades (filters: planning semester, promotion, option, date range)
- Exports:
  - Export semester student lists (grades, ranks, averages)
  - Export reports as PDF

### Student side
- Authentication
- View own semesters, grades and averages
- Export own results as PDF
- See progression between semesters (ex: average increase %)

---

## Tech Stack

- Backend / Web: **.NET Core** (MVC + Razor Pages)
- Database: **SQL Server**
- Containerization: **Docker** (SQL Server in container on the server)
- CI/CD: **GitHub Actions**
- Hosting: **DigitalOcean Droplet** (via GitHub Education)

---

## Project Workflow (how it was built)

1. Imagine a project scenario
2. Write the user story (text)
3. UI/UX analysis: derive final screens + describe the UI
4. Database design: define tables and relations
5. Task assignment (binôme)
6. Bootstrap solution template + push to GitHub
7. Prepare the DigitalOcean Droplet (Docker + SQL Server + runner)
8. Write the GitHub Actions YAML for automatic deployment on:
   - push to `main`
   - merge into `main`
9. Start development

---

