# DigitalisationManager

DigitalisationManager is an ASP.NET Core MVC application for managing archival Funds, Items, and their associated digital TIFF files.

The system provides structured archival storage, metadata management, controlled file access, and authentication using ASP.NET Core Identity and Entity Framework Core with SQL Server.

---

# Project Architecture

The solution follows a clean layered architecture:

```
DigitalisationManager
│
├── DigitalisationManager.Web            → MVC UI (Controllers + Views)
├── DigitalisationManager.Services.Core  → Business Logic Layer
├── DigitalisationManager.Data           → EF Core DbContext
├── DigitalisationManager.Data.Models    → Entity Models
├── DigitalisationManager.Web.ViewModels → ViewModels
├── DigitalisationManager.GCommon        → Shared Enums & Constants
```

### Design Principles

- Controllers are thin
- Business logic lives in Services
- Files are stored on disk (not in database)
- Metadata is stored in SQL Server
- Authorization via ASP.NET Core Identity
- Clean separation of concerns

---

# Database Structure

## Fund
- Id
- Code
- Title
- Description
- CreatedAt
- ICollection<Item>

## Item
- Id
- InventoryNumber
- Description
- DocumentDateText
- Status
- CreatedAt
- FundId
- ICollection<DigitalFile>

## DigitalFile
- Id
- OriginalFileName
- StoredFileName
- RelativePath
- SizeBytes
- UploadedAt
- ChecksumSha256
- ItemId

---

# File Storage Structure

Uploaded TIFF files are stored on disk using this structure:

```
Storage/
 └── Funds/
     └── {FundId}/
         └── Items/
             └── {ItemId}/
                 └── Tiffs/
                     └── {guid}.tif
```

Files are:
- Validated (.tif / .tiff only)
- Size restricted
- Saved to disk
- Registered in the database
- Protected through authorized download endpoints

The `Storage/` folder is excluded from Git.

---

# Requirements

- .NET 8 SDK (or your project version)
- SQL Server (LocalDB works)
- Visual Studio 2022+ or VS Code

---

# How to Clone and Run

## 1. Clone the repository

```bash
git clone https://github.com/YOUR_USERNAME/DigitalisationManager.git
cd DigitalisationManager
```

---

## 2. Configure the database

Open:

```
DigitalisationManager.Web/appsettings.json
```

Update the connection string:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=DigitalisationManagerDb;Trusted_Connection=True;MultipleActiveResultSets=true"
}
```

---

## 3. Apply migrations

Using Package Manager Console:

```bash
Update-Database
```

Or CLI:

```bash
dotnet ef database update
```

---

## 4. Run the application

Visual Studio:
- Set `DigitalisationManager.Web` as Startup Project
- Press F5

CLI:

```bash
dotnet run --project DigitalisationManager.Web
```

---


# Authorization

- ASP.NET Core Identity is configured
- `[Authorize]` attributes protect application controllers
- Only authenticated users can:
  - Create/Edit Funds
  - Create/Edit Items
  - Upload/Delete files

---

# Configuration

File storage settings in:

```
DigitalisationManager.Web/appsettings.json
```

```json
"FileStorage": {
  "RootFolder": "Storage",
  "MaxTiffUploadSizeBytes": 524288000
}
```

Adjust maximum upload size as needed.

---

# .gitignore Requirements

Make sure the following are ignored:

```
DigitalisationManager.Web/Storage/
bin/
obj/
.vs/
```


# Typical Workflow

1. Register a user
2. Login
3. Create a Fund
4. Create an Item
5. Upload TIFF files
6. Download or delete files
7. Items cannot be deleted if files exist

---

# Security Notes

- Files are not publicly accessible
- Files are streamed via controller endpoints
- SHA256 checksum stored for integrity verification
- Only authenticated users can modify data

---
