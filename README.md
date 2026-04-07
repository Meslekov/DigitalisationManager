# Digitalisation Manager

## Overview

Digitalisation Manager is an ASP.NET Core MVC web application for managing the digitalisation workflow of archival materials.

The application supports archival funds, categorized items, TIFF file uploads, automatic preview generation, file access control, and item history tracking. It is designed as a structured MVC project with clear separation of concerns and a service-driven business logic layer.

The main goal of the project is to simulate a realistic archival digitalisation process in a maintainable and testable ASP.NET Core application.

---

## Project Concept

The system models a real-world archive workflow:

- archival materials are organized into funds
- each item belongs to a fund and category
- TIFF files are uploaded as digital representations of physical archive items
- preview images are generated automatically
- file access can be restricted or allowed
- item changes are tracked through history entries

This makes the project suitable for demonstrating practical business logic, file processing, validation, and role-based access control.

---

## Features

### Core Features

- CRUD operations for Funds
- CRUD operations for Items
- Category classification for items
- TIFF upload with validation
- Automatic JPEG preview generation
- Digital file storage and metadata tracking
- Search, filtering, and pagination
- Item history tracking
- Role-based access control
- ASP.NET Identity authentication

### Manager Features

- Manage funds
- Manage items
- Upload TIFF files
- Delete digital files
- Control download permissions for digital files

### Administrator Features

- Access the Admin panel
- Manage user roles
- Assign User, Manager, and Administrator roles

### User Features

- Browse items
- View item details
- Open preview pages
- Download files only when access is allowed

---

## User Roles

The application uses three roles:

### User
Regular authenticated user.

Permissions:
- browse items
- view details
- view preview pages
- download only allowed files

### Manager
Content management role.

Permissions:
- manage funds
- manage items
- upload TIFF files
- manage digital files
- control file access

### Administrator
Full system administration role.

Permissions:
- everything available to Manager
- manage user accounts and role assignments
- access the Admin area

---

## Architecture

The project follows a layered architecture.

```text
DigitalisationManager
│
├── Data
│   ├── DbContext
│   ├── Entity Configuration
│   └── Seed Data
│
├── Data.Models
│   ├── Entities
│   └── Identity Models
│
├── GCommon
│   ├── Constants
│   ├── Enums
│   └── Shared Helpers
│
├── Services.Core
│   ├── Business Logic
│   ├── Service Contracts
│   └── Processing Helpers
│
├── Web
│   ├── Controllers
│   ├── Areas
│   │   ├── User
│   │   ├── Manager
│   │   └── Admin
│   ├── Razor Views
│   └── UI Layer
│
├── Web.ViewModels
│   └── View Models for Forms, Lists, Details, and Admin
│
└── Tests
    ├── Service Tests
    ├── Fakes
    └── Test Helpers
```

## Design Principle

Controllers stay thin and delegate business logic to the Services layer.

This improves:

- maintainability  
- testability  
- separation of concerns  

---

## Entities Overview

### Fund
Represents an archival collection.

### Category
Represents item classification.

### Item
Represents a physical archival object with metadata.

### DigitalFile
Stores information about:

- original TIFF file  
- generated preview image  
- file paths  
- content types  
- checksums  
- access permissions  

### ItemHistory
Stores history entries related to item changes and actions.

### ApplicationUser
Custom ASP.NET Identity user.

### ApplicationRole
Custom ASP.NET Identity role.

---

## TIFF Workflow

One of the core features of the project is the TIFF workflow.

### Upload Process

1. Manager selects one or more TIFF files  
2. The system validates:
   - allowed extension (`.tif` / `.tiff`)  
   - file size  
3. The original TIFF file is stored  
4. The first page of the TIFF is converted to JPEG  
5. The preview image is stored  
6. A `DigitalFile` record is created in the database  
7. The related item status is updated to `Digitized` when upload succeeds  

### Failure Handling

If processing fails during upload:

- the operation is stopped  
- already saved files are cleaned up  
- the database remains consistent  
- the user receives an error message  

This workflow is implemented in the service layer and covered by unit tests.

---

## Validation and Security

The project includes multiple validation and security measures.

### Validation

- server-side business validation in services  
- unique inventory number per fund  
- valid fund and category checks  
- active category validation  
- file extension validation  
- file size validation  

### Security

- ASP.NET Identity for authentication  
- role-based authorization  
- restricted Manager and Admin areas  
- download access control for digital files  
- safe handling of restricted files  
- antiforgery protection through MVC forms  
- protection against invalid input through model validation and service checks  

---

## Error Handling

The project includes explicit error handling for important workflows.

Examples:

- invalid business rules during item creation or update  
- file processing failures during upload  
- missing file checks during preview/download  
- restricted access to files  
- cleanup logic when a file workflow fails  

The goal is to provide a stable user experience and avoid inconsistent data.

---

## Data Seeding

The database is seeded with demo data to make testing and evaluation easier.

### Seeded Roles

- Administrator  
- Manager  
- User  

### Seeded Accounts

- Administrator account  
- Manager account  
- User account  

### Seeded Domain Data

- multiple Funds  
- multiple Categories  
- multiple Items for search, filtering, and pagination demos  

This allows lecturers and testers to log in immediately and explore the application without manual setup.

---

## Setup Instructions

1. Clone the repository  
2. Open the solution in Visual Studio  
3. Configure the connection string in `appsettings.json`  
4. Apply migrations  
5. Run the application  

### Package Manager Console

Update-Database


## How to Run

- Start the application from Visual Studio  
- Use the seeded accounts or register a new user  
- Navigate through the appropriate area depending on role  

---

## How to Use

### Authentication

The application uses ASP.NET Identity authentication.

You can:

- log in with a seeded account  
- or register a new account  

---

### User Workflow

1. Log in as a User  
2. Open the User area  
3. Browse items  
4. Open item details  
5. View preview pages for available digital files  
6. Download files only if access is allowed  

---

### Manager Workflow

1. Log in as a Manager  
2. Open the Manager area  
3. Manage funds  
4. Manage items  
5. Upload TIFF files  
6. Review preview pages  
7. Enable or disable file download access  

---

### Administrator Workflow

1. Log in as an Administrator  
2. Open the Admin area  
3. Open the Users section  
4. Review registered users  
5. Assign roles:
   - User  
   - Manager  
   - Administrator  

---

## Unit Tests

The project includes NUnit-based unit tests focused on the service layer.

### Tested Services

#### ItemService

Covered scenarios include:

- create validation  
- update logic  
- delete restrictions  
- history creation  
- filtering  
- search  
- pagination  

#### DigitalFileService

Covered scenarios include:

- upload validation  
- successful TIFF upload  
- preview generation workflow  
- failure cleanup  
- preview access  
- download rules  
- file deletion  

### Testing Approach

- isolated test database  
- fake storage services  
- fake TIFF conversion service  
- service-focused business logic tests  

The tests were designed to cover meaningful business logic rather than superficial checks.

---

## Technologies Used

- ASP.NET Core MVC  
- Entity Framework Core  
- ASP.NET Identity  
- SQL Server  
- Razor Views  
- Bootstrap  
- NUnit  

---

## Demo Credentials

### Administrator

- Email: `admin@digitalisationmanager.local`  
- Password: `Admin123!`  

### Manager

- Email: `manager@digitalisationmanager.local`  
- Password: `Manager123!`  

### User

- Email: `user@digitalisationmanager.local`  
- Password: `User123!`  

---

## Conclusion

Digitalisation Manager demonstrates:

- layered ASP.NET Core MVC architecture  
- realistic archival digitalisation workflow  
- TIFF upload and preview processing  
- role-based access separation  
- service-layer business logic  
- meaningful automated tests  
- practical lecturer-friendly demo data  

The project was built to combine technical structure, real workflow simulation, and maintainable code organization in an ASP.NET Core MVC application.
