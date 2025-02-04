# ?? Pension System Management

A robust Pension System Management application designed with Clean Architecture to manage contributions, benefits, transactions, and scheduled jobs efficiently.

## ?? Project Setup Instructions

1. **Clone the repository:**
   ```bash
   git clone https://github.com/Vic-tory96/PensionSystemManagement.git
   cd PensionSystemManagement

2. Set up Database:
  - Ensure SQL Server is running on your machine
  - Open appsettings.json and update the connection string with your SQL Server
	credentials:
  "ConnectionStrings": {
  "DefaultConnection": "Server=YOUR_SERVER_NAME;Database=PensionSystemDB;Trusted_Connection=True;"
}
  - Apply database migrations to create the necessary tables using the Package Manager Console (PMC):
	
	 Add-Migration "InitialCreate"
	 Update-Database

3. Run the Application
   dotnet run

4. Access the API
   - Open your browser or API testing tool (like Postman).
   - Go to http://localhost:5000/swagger to view the Swagger API documentation.

   API DOCUMENTATION

1. ?? Setting up Swagger in your Project
Install the Swagger NuGet package:

Open your Package Manager Console in Visual Studio and run this command to install the Swagger package:
Install-Package Swashbuckle.AspNetCore
This will add the Swagger middleware and the required dependencies to your project.

2. Configure Swagger in Startup.cs or Program.cs:
Depending on your ASP.NET Core version, you’ll add the Swagger configuration in either Startup.cs or Program.cs. Here’s how to do it for both.

- For .NET 6 or Later (Program.cs)
In Program.cs, add Swagger services to the DI container:

using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Register the Swagger services
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "PensionSystem API", Version = "v1" });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(); // Enable Swagger UI in development environment
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "PensionSystem API v1");
        options.RoutePrefix = string.Empty; // Make Swagger UI available at the root
    });
}

app.UseAuthorization();

app.MapControllers();

app.Run();

1. For .NET 5 or Earlier (Startup.cs)
In Startup.cs, add Swagger services to the DI container in the ConfigureServices method:

public void ConfigureServices(IServiceCollection services)
{
    services.AddControllers();
    services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new OpenApiInfo { Title = "PensionSystem API", Version = "v1" });
    });
}

2. In the Configure method, add the middleware to serve Swagger and Swagger UI:

public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
    if (env.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "PensionSystem API v1");
            options.RoutePrefix = string.Empty; // Serve Swagger UI at the root
        });
    }

    app.UseRouting();
    app.UseAuthorization();
    app.MapControllers();
}

3. Run the Application: After setting up Swagger, run the application.

?? Accessing Swagger UI
 - Once you run the application, you should be able to access the Swagger documentation at:
   http://localhost:5000/swagger

?? What’s Next?
Test your API Endpoints: After setting up Swagger, test your API through the Swagger UI. You can directly send requests and see responses right there.

/// <summary>
/// Gets all members in the pension system.
/// </summary>
/// <returns>A list of members</returns>
[HttpGet]
public IActionResult GetMembers()
{
    // Your code here
}

## ??? Architecture Overview

The **Pension System Management** application follows **Clean Architecture**, which ensures separation of concerns, maintainability, and scalability. The application is divided into the following layers:

### 1. **Core Layer (Domain Layer)**
   - **Responsibilities**: Contains the core business logic, domain models, and business rules.
   - **Key Components**:
     - **Entities**: Core business objects like `Member`, `Contribution`, etc.
     - **Interfaces**: Service contracts (e.g., `IMemberService`, `IContributionService`).

### 2. **Application Layer**
   - **Responsibilities**: Contains use cases and application logic that interact with the core domain and external layers.
   - **Key Components**:
     - **Use Cases**: Services defining business operations (e.g., `AddContribution`, `CalculateBenefits`).
     - **DTOs (Data Transfer Objects)**: Used to transfer data between layers.

### 3. **Infrastructure Layer**
   - **Responsibilities**: Implements data access.
   - **Key Components**:
     - **Repositories**: Data access implementation (e.g., `MemberService`, `ContributionService`).
    
### 4. **API Layer (Presentation Layer)**
   - **Responsibilities**: Exposes the application's functionality through the API. Communicates directly with the Application layer.
   - **Key Components**:
     - **Controllers**: API endpoints exposed to the client (e.g., `MemberController`, `ContributionController`).
     - **Swagger**: Automatically generated API documentation.

### 5. **Cross-Cutting Concerns**
   - **Responsibilities**: Shared services and utilities like  validation, and error handling.
   - **Key Components**:
     - **FluentValidation**: Validation for data coming into the system.
   



