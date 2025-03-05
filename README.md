# CMS Solution - HSOne

## Overview
This CMS solution is built using best practices such as:
- **Clean Architecture**
- **Distributed Architecture**
- **API Client Generation**
- **Domain Driven Design (DDD)**
- **Repository Pattern**
- **Unit Of Work**
- **Dependency Injection**

The goal of this project is to provide a robust content management system for creating a blog website that displays posts and manages website content.

## Key Features
- **Admin Panel**:  
  - Role-based access control  
  - Management of posts, categories, series, authors, and royalties  
  - Content review and publishing

- **Portal**:  
  - News display  
  - Tag management  
  - Search functionality  
  - User registration and login  
  - Post submission  
  - Analytics and statistics display

## Project Structure

### Backend (Using .NET Core 8)
The backend is divided into four main projects:

1. **HSOne.Data**  
   - **Purpose**: Direct database interactions.  
   - **Components**: Repository, RepositoryBase, UnitOfWork, Migrations, DataSeeder, DBContext.

2. **HSOne.Core**  
   - **Purpose**: Contains the core business logic and shared components.  
   - **Components**: Request models, DTOs, IRepository, IUnitOfWork, Constants, Event Handlers, Domain models, ConfigOptions.

3. **HSOne.Api**  
   - **Purpose**: Provides API endpoints for the Admin CMS.  
   - **Components**: API Controllers, Extension methods, IParameterFilter.

4. **HSOne.WebApp**  
   - **Purpose**: MVC Web Application for public content display.  
   - **Components**: Controllers, Models, Views.

### Frontend (Using Angular 18)
- **admin-ui**:  
  - **Technologies**: CoreUI Angular combined with PrimeNG.  
  - **Purpose**: Administrative interface for managing users, roles, posts, categories, series, and royalties.  
  - **API Client Generation**: Uses NSwag to generate API classes for frontend consumption.  
  - **Scripts**:
    - `"ng": "ng"`
    - `"start": "ng serve -o"`
    - `"build-prod": "ng build --c production"`
    - `"build-staging": "ng build --c staging"`
    - `"watch": "ng build --watch --configuration development"`
    - `"test": "ng test"`
    - `"nswag-admin": "node node_modules/nswag/bin/nswag.js run nswag-admin.json"`

## Setup & Installation

### Prerequisites
- **Backend**: .NET Core 8 SDK, Visual Studio/VSCode or any compatible IDE.
- **Frontend**: Node.js and Angular CLI.

### Backend Setup
1. **Restore NuGet Packages**:  
   Ensure all necessary packages for each project are restored.
   
2. **Configure Database**:  
   Update the connection strings in the `appsettings.json` files (especially in HSOne.Data and HSOne.Api).

3. **Database Migrations & Data Seeding**:  
   Run the migrations and seed the database if necessary.
   - Set **HSOne.Data** as the startup project 
   - Run the following commands in the Package Manager Console
   - Set the default project to **HSOne.Data** in the Package Manager Console
   - Seed the database with initial data
	   ```bash
	   Add-Migration Initial
	   Update-Database
	   ```
   - Create Scripts changes from the last migration
   - Run the following commands in the Package Manager Console
        ```bash
        Script-Migration -From <PreviousMigration> -To <LastMigration>
        # Example for the first migration: Script-Migration -From 0 -To Initial
		```
   - Save the generated script to the `/database/schemas` folder in the physical path of the project.

4. **Launch Applications**:  
   - Run **HSOne.Api** to start the Admin CMS API.
   - Run **HSOne.WebApp** to launch the public web application.

### Frontend Setup
   **Install Dependencies**
   ```bash
   npm install
   ```

   **Start Development Server**
   ```bash
   npm run start
   ```
    
   **Build for Production or Staging**
   ```bash
   npm run build-prod
   npm run build-staging
   ```

   **Generate API Client**
   ```bash
   npm run nswag-admin
   ```
    

## Architectural Highlights & Best Practices

- **Clean Architecture**: 
    Separation of concerns across layers (Presentation, Application, Domain, Infrastructure) enhances maintainability and scalability.

- **Distributed Architecture**:
    Supports system scaling in a distributed environment.

- **Domain Driven Design (DDD)**:
    Focuses on the core domain and business logic, ensuring consistency and extendibility.

- **Repository & Unit Of Work**:
    Efficient and controlled data access management.

- **Dependency Injection**:
    Facilitates modular design and easier unit testing.



## Development Guidelines

- **Follow Coding Conventions**: Adhere to clean code practices and the established best practices.
- **Feature Development**: Begin with defining the domain model and apply appropriate design patterns.
- **Testing**: Write unit tests for each module to ensure stability and quality.


## Contact
- For any inquiries or feedback, please contact the development team at: ltienhoang2@gmail.com
