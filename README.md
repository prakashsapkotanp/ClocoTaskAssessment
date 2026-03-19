# Artist Management System

This is a full-stack web application built with ASP.NET Core (.NET 10) for the backend and Angular 20 for the frontend. This system allows you to manage artists and related data.

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Node.js](https://nodejs.org/) (latest LTS recommended)
- [Angular CLI](https://angular.dev/tools/cli) (installed globally via `npm install -g @angular/cli`)
- SQL Server (LocalDB or a full instance)

## Project Structure

- `ArtistManagementSystem.Server/`: The ASP.NET Core Web API backend project.
- `artistmanagementsystem.client/`: The Angular single-page application (SPA) frontend.

## Setup Instructions

### 1. Database Configuration

1. Open the file `ArtistManagementSystem.Server/appsettings.json`.
2. Update the `DefaultConnection` string to point to your local SQL Server instance. By default, it looks like this:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=LENEVO;Database=ArtistManagement_Db;Trusted_Connection=True;TrustServerCertificate=True;"
   }
   ```
   *(Change `Server=LENEVO` to `Server=localhost`, `Server=.\SQLEXPRESS`, or your specific SQL Server instance).*

### 2. Apply Database Migrations

1. Open your terminal or command prompt.
2. Navigate to the backend directory:
   ```bash
   cd ArtistManagementSystem.Server
   ```
3. Apply Entity Framework migrations to create the database schema:
   ```bash
   dotnet ef database update
   ```
   *Note: If you don't have the EF Core CLI tools installed, you'll need to install them first with `dotnet tool install --global dotnet-ef`.*

### 3. Install Frontend Dependencies

1. Navigate to the frontend directory:
   ```bash
   cd ../artistmanagementsystem.client
   ```
2. Install the necessary Node packages:
   ```bash
   npm install
   ```

## Running the Application

The project uses the ASP.NET Core Single Page App (SPA) proxy, which means running the backend will automatically start the Angular development server alongside it.

### Using Visual Studio
1. Open the solution file `ArtistManagementSystem.slnx` using Visual Studio.
2. Ensure the startup project is set to `ArtistManagementSystem.Server`.
3. Press **F5** or click the **Start** button to build and run the application. Visual Studio will automatically launch both the API and the Angular frontend.

### Using the .NET CLI
1. Open a terminal and navigate to the backend directory:
   ```bash
   cd ArtistManagementSystem.Server
   ```
2. Run the application:
   ```bash
   dotnet run
   ```
3. The .NET backend will start up and automatically trigger `npm start` in the `artistmanagementsystem.client` folder. A browser window should open (usually at `https://localhost:49971` or similar, as defined in `launchSettings.json` or `.csproj`).

## Troubleshooting

- **HTTPS Certificate Issues**: The frontend setup requires SSL for the Node API proxy. You may need to create and trust local development certificates. Run the following command if you get SSL errors in the browser:
  ```bash
  dotnet dev-certs https --trust
  ```
- **Entity Framework Errors**: Verify that your SQL Server is running and the credentials/server name in `appsettings.json` are correct.
- **Migration Issues**: If you encounter issues with database migrations, delete the `Migrations` folder inside the `ArtistManagementSystem.Server` directory, then recreate the initial migration and update the database:
  ```bash
  dotnet ef migrations add InitialCreate
  dotnet ef database update
  ```
