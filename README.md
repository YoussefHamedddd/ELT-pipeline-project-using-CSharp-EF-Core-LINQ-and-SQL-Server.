# End-to-End ELT Pipeline for a Simulated Company System  
*Using C#, EF Core, LINQ, and SQL Server*

## 📌 Project Overview
This project demonstrates an *End-to-End ELT (Extract, Load, Transform) pipeline* built for a *simulated company system*.  
The goal is to simulate real-world data engineering tasks using C#, EF Core, LINQ, and SQL Server.

### Main Features
- *Extract*: Load raw data from JSON files (simulated company data).  
- *Load*: Insert the extracted data into a SQL Server database using EF Core.  
- *Transform*: Apply data cleaning and business rules using LINQ queries and SQL scripts.  

---

## 🛠 Technologies Used
- *C# (.NET 8 / .NET Core)*  
- *Entity Framework Core (Code-First, Migrations)*  
- *LINQ* for data transformations  
- *SQL Server* for storage and advanced transformations  
- *JSON* as the data source  

---

## ⚙ Project Structure
- DataSource/ → Contains JSON files used as raw data sources  
- LoadFunctions/ → C# functions for extracting & loading JSON data into SQL Server  
- Transform-SQL/ → SQL scripts for applying transformations & business rules  
- Models/ → EF Core entity classes  
- Migrations/ → EF Core migration files  
- Configurations/ → DbContext and EF Core configuration  

---

## 🚀 How to Run the Project
*Clone the repository*
   ```bash
   git clone https://github.com/YoussefHamedddd/ELT-pipeline-project-using-CSharp-EF-Core-LINQ-and-SQL-Server..git
   

2. Setup SQL Server

Make sure you have SQL Server installed and running.

Create a new database (e.g., Company_ELT).



3. ⚠ IMPORTANT: Configure the Connection String Before Running

Open your DbContext class inside Configurations/.

Replace the placeholder with your own SQL Server connection string:

// Example:
// optionsBuilder.UseSqlServer("Server=.;Database=Company_ELT;Trusted_Connection=True;");

⚠ The project will NOT run unless you enter a valid connection string here.



4. Apply Migrations

dotnet ef database update


5. Run the Project

dotnet run

This will:

Extract data from JSON.

Load it into SQL Server.

Apply transformations (via LINQ + SQL).





---

📊 Data Transformations Applied

Removed invalid project hours (negative or > 500).

Replaced NULL project hours with the average value.

Normalized employee gender (M, F, others → Unknown).

Fixed invalid birthdates (outside 1970–2025 → NULL).

Cleaned up dependent names (removed duplicates and invalid ones).

Normalized city values (blank → NULL, NULL → Unknown).



---

📝 Notes

The system is simulated and does not represent a real company.

Connection string is NOT included for security reasons — you must add your own before running.

JSON samples are included in the DataSource/ folder to allow testing.
