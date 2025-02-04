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
