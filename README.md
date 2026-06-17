# MedCareCore - Medical Record & Clinic Management System

**MedCareCore** is an enterprise-grade, secure, and high-performance electronic health record (EHR) and clinic operations management web application. Built utilizing the robust **.NET Core Architectural Ecosystem** and **SQL Server**, the system optimizes clinical workflows, safeguards patient medical history records, tracks physician registries, automates doctor-patient consultation sessions, and safely processes patient digital prescription pipelines alongside diagnostic medical file attachments.

---

## 🚀 Key Features & Business Logic

- **Doctors Directory & Registry Management:** Tracks clinical specialties, consultation schedules, availability statuses, and automated financial fee valuations.
- **Patients Registry & Demographics Indexing:** Comprehensive management of patient medical files, clinical histories, gender-specific demographic attributes, and immediate automated notification dispatch logs.
- **Appointments Architecture:** Advanced relational workflow model binding medical consultants to patients with immutable data integrity constraints.
- **Consultation Sheets & Medical Records:** Multi-tiered input pipelines for complex clinical diagnoses, detailed dynamic prescription outputs, and medical follow-up registries.
- **Secure Medical Attachment Infrastructure:** Enterprise-ready cryptographic file upload mechanism handling diagnostic reports, digital X-rays, and multi-format medical PDFs with background folder validation and absolute server-side security isolation.

---

## 💻 Technology Stack & Architecture

The application is architected under rigorous software engineering standards, utilizing a decoupled, modular, and performance-tuned multi-tier pattern:

### Backend Ecosystem
- **Framework:** .NET 8.0 / 6.0 MVC (Model-View-Controller)
- **Language:** C# (Asynchronous Background Multi-Threading Execution)
- **Data Access:** Entity Framework Core (EF Core) via LINQ Queries

### Database Infrastructure
- **Engine:** Microsoft SQL Server
- **Schema Management:** Code-First approach utilizing EF Core Migrations Engine
- **Data Integrity:** Strict relational integrity mapping with cascade constraint controls

### Frontend & User Interface
- **View Engine:** Razor Pages (CSHTML)
- **Design Framework:** Bootstrap 5 (Responsive Layouts)
- **Iconography:** Bootstrap Icons System
- **Validation Pipeline:** jQuery Validation Engine & Unobtrusive Client-Side Validation

### Design Patterns & Structural Standards
- **Unit of Work Pattern:** Centralizes multi-repository transaction operations into single transaction scopes, ensuring atomic database executions.
- **Repository Pattern (Generic & Specialized):** Completely decouples data access layers from business core controllers, abstracting underlying raw DB persistence contexts.
- **Object Validation Pipeline:** Rigid client-side and server-side model validations (`ModelState` orchestration) with dynamic `ModelState.Remove()` interception for complex media bindings.
- **State Separation & Entity Tracker Detachment:** Explicit manual instance isolation (`EntityState.Detached`) inside repositories to bypass multi-instance key tracking constraints during database update operations.

---

## 📦 NuGet Dependencies & Packages Installation

The system leverages verified enterprise packages for database management, validation workflows, and system UI rendering. Run the following installation commands in your Package Manager Console or terminal:

```bash
# Install Entity Framework Core Context Engine
dotnet add package Microsoft.EntityFrameworkCore --version 8.0.0

# Install SQL Server Runtime Database Provider
dotnet add package Microsoft.EntityFrameworkCore.SqlServer --version 8.0.0

# Install Entity Framework Core Tools for Migration Execution
dotnet add package Microsoft.EntityFrameworkCore.Tools --version 8.0.0

{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_LOCAL_SERVER_NAME;Database=MedCareDb;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}

git clone [https://github.com/YOUR_USERNAME/MedCareCore.git](https://github.com/YOUR_USERNAME/MedCareCore.git)
cd MedCareCore

MedCareCore.Models : Contains Domain entities (Doctor.cs, Patient.cs, Appointment.cs, MedicalRecord.cs).
MedCareCore.Data : Houses ApplicationDbContext, managing relational sets and seed parameters.
MedCareCore.IRepository : Contains contracts defining atomic database interactions (e.g., IMedicalRecordRepository with custom Detach() capabilities).
MedCareCore.Repository :  Concrete data layers implementing the Unit of Work and Generic Repository layers.
wwwroot/medicalFiles : The local storage destination directory for uploaded clinical PDFs, diagnostic charts, and patient attachments. (Auto-created at runtime).



# Install Developer Runtime Compilation Support for Razor Pages
dotnet add package Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation --version 8.0.0
