To-Do Test Project
Overview
The To-Do Test Project is a .NET Core web API designed to manage a simple to-do list application. This project implements user authentication using JWT (JSON Web Tokens) and allows users to perform CRUD (Create, Read, Update, Delete) operations on to-do items. The application is structured to facilitate unit testing and integration testing to ensure code quality and reliability.
Features
•	User Authentication:
o	Implements JWT-based authentication to secure API endpoints.
o	Users can log in with predefined credentials (username: test, password: password).
o	Basic authentication and authorization are applied to restrict access to certain endpoints.
•	To-Do Item Management:
o	Create, read, update, and delete to-do items.
o	Mark to-do items as completed.
o	Filtering of to-do items (e.g., by due date and completion status).
o	Sorting of to-do items based on different criteria (e.g., title, creation date).
Security Headers: Implementation of security headers to protect against common web vulnerabilities:
•	X-Content-Type-Options: Prevents MIME-sniffing attacks.
•	X-Frame-Options: Prevents clickjacking attacks by disallowing iframes.
•	X-XSS-Protection: Enables the Cross-Site Scripting filter.
•	Strict-Transport-Security: Enforces HTTPS connections for enhanced security.
•	Referrer-Policy: Controls the information sent in the Referer header.
o	
•	Input Validation:
o	Ensures that all input is validated before processing to prevent invalid data and enhance security.
•	Testing:
o	Unit tests for the AuthController to validate login functionality.
•	Configuration Management:
o	Uses appsettings.json for configuration, allowing easy updates to JWT settings such as key, issuer, and audience.
o	Mocks configuration in unit tests to isolate test cases from the actual configuration file.
Design Choices
•	.NET Core REST API:
o	Built using .NET Core to leverage its performance and scalability for creating RESTful services.
o	Follows REST principles for resource management, making it easy to interact with the API.
•	Separation of Concerns:
o	The application follows the MVC pattern, separating the data access layer, business logic, and presentation layer.
o	Controllers handle HTTPS requests and responses, while services manage the business logic.
•	JWT for Authentication:
o	Chose JWT for its statelessness and scalability, allowing for efficient user authentication without server-side sessions.
•	Mocking for Testing:
o	Utilized mocking frameworks to simulate dependencies during testing, ensuring unit tests are isolated and reliable.
o	This approach allows for testing the behavior of controllers and services without relying on actual database calls or external services.
•	Configuration Injection:
o	Used dependency injection to provide configuration settings to the controllers, promoting flexibility and testability.
Getting Started
Command-Line Interface Commands
1: Navigate to the Project Directory:
cd <project-directory>
2: Restore Dependencies:
dotnet restore
3: Trust HTTPS Development Certificates: To run the application with HTTPS, trust the development certificates:
dotnet dev-certs https –trust
4: Use .Net Framework: 8.0


