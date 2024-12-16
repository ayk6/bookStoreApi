# Book Store Api

API for managing a bookstore's inventory and transactions.

## Features
- Authentication (JWT)
- Book management (CRUD)
- Role-based access control (Admin/User)

## Tech Stack
- ASP.NET Core, MSSQL
- Entity Framework Core
- JWT Authentication
- Layered Architecture

## Installation
1. Clone the repository:
    ```bash
    git clone https://github.com/ayk6/bookStoreApi.git
   
    cd bookStoreApi.api
    dotnet restore
    dotnet ef database update
    dotnet run
    ```

## Configuration
- Update the connection string in `appsettings.json` (backend).
- Modify environment variables in `.env` (frontend) if necessary.


