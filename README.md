# إتقان (Etqan) Enterprise Resource Planning System

## Overview

Etqan ERP is a comprehensive enterprise resource planning system designed specifically for small to medium-sized enterprises, with full Arabic language support. The system integrates warehouse management, manufacturing processes, and financial operations into a unified platform with a responsive, user-friendly interface.

![Dashboard Screenshot](docs/images/dashboard.png)

## Features

### Core Modules

- **Warehouse Management**
  - Stock entry tracking with batch numbers
  - Inventory movement recording
  - Warehouse transfers with approval workflows
  - Real-time stock level monitoring

- **Manufacturing Module**
  - Bill of Materials management
  - Production planning and scheduling
  - Work order processing
  - Material Requirements Planning (MRP)
  - Quality control integration

- **Financial Management**
  - General ledger and chart of accounts
  - Accounts payable and receivable
  - Financial statements generation
  - Tax management
  - Fixed asset tracking
  - Budget management

### Technical Features

- Responsive design for desktop and mobile devices
- Complete right-to-left (RTL) support for Arabic language
- Dark/light mode themes
- Role-based access control
- Comprehensive audit logging
- Data import/export capabilities
- Advanced dashboards and reporting

## Technology Stack

- **Backend**: ASP.NET Core 6.0 MVC
- **Database**: Microsoft SQL Server
- **ORM**: Entity Framework Core
- **Frontend**: Bootstrap 5, jQuery, Chart.js
- **Authentication**: ASP.NET Core Identity
- **Reporting**: EPPlus for Excel generation

## Getting Started

### Prerequisites

- .NET 6.0 SDK or later
- SQL Server 2019 or later
- Visual Studio 2022 (recommended) or VS Code

### Installation

1. Clone the repository


2. Update database connection string in `appsettings.json`
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=YOUR_SERVER;Database=EtqanERP;Trusted_Connection=True;MultipleActiveResultSets=true"
}

Apply database migrations
Copydotnet ef database update

Run the application
dotnet run

Initial Setup
After installation, you'll need to:

Create an administrator account
Configure company information
Set up warehouses and inventory
Configure chart of accounts (for financial module)
Define manufacturing resources (for manufacturing module)

Documentation
For detailed documentation, please refer to the User Guide and API Documentation.
Project Structure
Copy/
├── src/                  # Source code
│   ├── Etqan.Web/        # Web application project
│   ├── Etqan.Core/       # Core business logic and entities
│   ├── Etqan.Data/       # Data access layer
│   └── Etqan.Services/   # Business services
├── tests/                # Test projects
│   ├── Etqan.Tests.Unit/
│   └── Etqan.Tests.Integration/
├── docs/                 # Documentation
└── scripts/              # Helper scripts
Contributing

Fork the repository
Create your feature branch (git checkout -b feature/amazing-feature)
Commit your changes (git commit -m 'Add some amazing feature')
Push to the branch (git push origin feature/amazing-feature)
Open a Pull Request

License
This project is licensed under the MIT License - see the LICENSE file for details.
Acknowledgements

Dr. Shymaa Haridy for project supervision
Ain Shams University Faculty of Computer & Information Sciences
All contributors who participated in this project

Screenshots
Dashboard
Show Image
Warehouse Management
Show Image
Manufacturing Module
Show Image
Financial Reports
Show Image
Copy
This README provides a comprehensive overview of your project, including installation instructions, features, and project structure. You can customize it further to match your specific implementation details.

