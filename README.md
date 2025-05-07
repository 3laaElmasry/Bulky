Bulky Book - Online Bookstore 📚

Project Overview
Bulky Book is a powerful online bookstore developed using ASP.NET Core MVC with a modern 
front-end built using HTML, CSS, JavaScript, and Bootstrap. The platform enables users to register,
browse, and purchase books, while providing an advanced Admin Control Panel for system management.
Additionally, it allows administrators to add new books, categories, and partnered companies for enhanced functionality.

Technologies Used
* Backend: ASP.NET Core MVC

* Frontend: HTML, CSS, JavaScript, Bootstrap

* Database: SQL Server with Entity Framework Core

* Architecture: N-Tier Architecture

* Design Patterns: Repository Pattern, Unit of Work

User Roles in the System
Customer: Must pay immediately when purchasing a book.

Admin: Has full access to manage the system.

Employee: Handles orders and customer support.

Company: Can buy books with a deferred payment option (up to 30 days).

Key Features
✅ Facebook Login for easy registration
✅ Full Role-Based Authorization with predefined access levels 
✅ Admin Control Panel to manage users, books, categories, and transactions
✅ Responsive UI designed with Bootstrap 
✅ Immediate payments, with deferred payment options for company users 
✅ Add and manage books, categories, and partnered companies

Admin Control Panel - Full Management System
The Admin Control Panel empowers administrators with complete control over the system, offering the following functionalities:

User Management
✅ View all registered users and their assigned roles (Customer, Admin, Employee, Company) 
✅ Modify user details, including role assignments 
✅ Block & Unblock users – Admin can restrict and restore access when necessary 
✅ Delete accounts when required

Book & Category Management
✅ Add, edit, and remove books from the catalog 
✅ Manage book categories, pricing, and availability

Company Management
✅ Add partnered companies and manage their profiles 
✅ Enable companies to purchase books with deferred payment terms 
✅ Track transactions and outstanding payments

Order & Payment Control
✅ Monitor all purchases and transactions 
✅ Validate payments and manage deferred payments for company users
✅ Generate reports on sales, revenue, and outstanding payments

Security & Access Control
✅ Role-based access ensures only authorized users can perform specific actions ✅ Secure authentication using Facebook Login

The Admin Dashboard is designed for ease of use, providing a clean interface for efficient system management. 🚀

How to Install the Project
Ensure .NET SDK and SQL Server are installed

Clone the repository:

bash
git clone https://github.com/3laaElmasry/Bulky.git
cd BulkyBook
Set up the database using Entity Framework Core:

bash
dotnet ef database update
Run the project:

bash
dotnet run
Contributors
[3laa Elmasry] - Lead Developer

License
This project is open-source under the MIT License.
