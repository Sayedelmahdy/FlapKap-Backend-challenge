# Vending Machine API

This project implements a vending machine API allowing users with different roles to interact with the system. Sellers can manage products, while buyers can deposit coins, make purchases, and reset their deposit.

# Table of Contents
- [Technologies Used](#technologies-used)
- [Getting Started](#getting-started)
- [API Endpoints](#api-endpoints)
- [Security Considerations](#security-considerations)
- [Edge Cases](#edge-cases)
- [Production Readiness](#production-readiness)


# Technologies Used
- Language/Framework: [C# .NET CORE API]
- Database: [SQL / MSSQL]
- Authentication: [JWT]
## Other Dependencies
- **Logging Library**: [Serilog]
- **Database ORM**: [Entity Framework Core]
- **Identity Management**: IdentityDbContext

  
# Getting Started
To run the project locally, follow these steps:
1. Clone the repository:
   git clone https://github.com/Sayedelmahdy/FlapKap-Backend-challenge.git
   cd FlapKap
2. Open FlapKap.sln on visual Studio
3. Configure the database:
  - First: you need to setup MSSQL in your machine 
  - Second: Go to Tools -> Nuget Package Manager -> Package Manager Console 
  - Third: Opening the Package Manager Console and write "Update-Database"
4.Run the application by pressing Ctrl+f5
5.Access the API from Swagger page at ':https://localhost:7029/swagger/index.html'

# API Endpoints

## Authentication

### Register User

- **Endpoint**: `POST /api/Authentication/Register`
- **Description**: Registers a new user.
- **Authentication**: Not required

### Login

- **Endpoint**: `POST /api/Authentication/Login`
- **Description**: Authenticates a user and returns a token.
- **Authentication**: Not required

### Refresh Token

- **Endpoint**: `GET /api/Authentication/RefreshToken`
- **Description**: Refreshes the authentication token.
- **Authentication**: Required

### Revoke Token

- **Endpoint**: `POST /api/Authentication/revokeToken`
- **Description**: Revokes the authentication token.
- **Authentication**: Required

## Product

### Get All Products

- **Endpoint**: `GET /api/Product/GetAllProduct`
- **Description**: Retrieves a list of all products.
- **Authentication**: Not required

### Get Product by ID

- **Endpoint**: `GET /api/Product/GetProduct/{id}`
- **Description**: Retrieves details of a specific product.
- **Authentication**: Not required

### Add Product

- **Endpoint**: `POST /api/Product/AddProduct`
- **Description**: Adds a new product to the vending machine.
- **Authentication**: Required (Seller role)

### Update Product

- **Endpoint**: `PUT /api/Product/UpdateProduct`
- **Description**: Updates information for a specific product.
- **Authentication**: Required (Seller role)

### Delete Product

- **Endpoint**: `DELETE /api/Product/DeleteProduct/{id}`
- **Description**: Deletes a specific product.
- **Authentication**: Required (Seller role)

## User

### Get All Users

- **Endpoint**: `GET /api/User`
- **Description**: Retrieves a list of all users.
- **Authentication**: Required (Seller role)

### Update User

- **Endpoint**: `PUT /api/User/UpdateUser`
- **Description**: Updates information for a specific user.
- **Authentication**: Required (Seller role)

### Delete User

- **Endpoint**: `DELETE /api/User/DeleteUser`
- **Description**: Deletes a specific user.
- **Authentication**: Required (Seller role)

## Vending Machine

### Deposit Coin

- **Endpoint**: `PUT /api/VendingMachine/deposit/{coin}`
- **Description**: Deposits a coin into the vending machine.
- **Authentication**: Required (Buyer role)

### Buy Product

- **Endpoint**: `POST /api/VendingMachine/buy`
- **Description**: Allows users to buy products.
- **Authentication**: Required (Buyer role)

### Reset Deposit

- **Endpoint**: `POST /api/VendingMachine/reset`
- **Description**: Resets the deposit of a user.
- **Authentication**: Required (Buyer role)

### Withdraw

- **Endpoint**: `POST /api/VendingMachine/withdraw`
- **Description**: Withdraws funds from the vending machine.
- **Authentication**: Required (Seller role)

# Security Considerations

This section outlines the security measures implemented in the project to ensure the integrity and confidentiality of user data and interactions.

## Authentication

The project uses JSON Web Tokens (JWT) for user authentication, providing a secure and stateless mechanism to verify the identity of users. Access tokens are set to expire after 5 minutes, enhancing security by minimizing the window of exposure in case of a token compromise.

To extend the user session, a refresh token mechanism has been implemented. Users can request a new access token by calling the `/api/Authentication/RefreshToken` endpoint, which requires proper authentication.

## Authorization

Role-based access control (RBAC) is enforced to ensure that each user has access only to the endpoints and actions corresponding to their role. This is achieved through the use of authorization attributes, preventing unauthorized access to sensitive functionality.

## Endpoint Security

Endpoints requiring elevated privileges, such as adding, updating, or deleting products, are protected by requiring the user to have the "Seller" role. Similarly, actions like depositing coins, buying products, and resetting deposits are restricted to users with the "Buyer" role. This granularity ensures that each user can only perform actions appropriate for their role.

## Best Practices

Throughout the development process, industry best practices for secure coding and data protection have been followed. This includes input validation, secure storage of sensitive information, and adherence to the principle of least privilege.

# Edge Cases

## Overview
This section discusses the various edge cases considered during the development of the project and outlines the strategies employed to handle them.

## Edge Cases Considered
1. **Invalid Input:** Robust input validation mechanisms are implemented to handle cases where users provide invalid or unexpected input. This helps prevent security vulnerabilities and ensures the stability of the application.

2. **Concurrency Issues:** Measures are in place to address potential concurrency issues, especially in scenarios where multiple users attempt to modify shared resources simultaneously.

3. **Token Expiry:** Edge cases related to token expiry are considered, and mechanisms are implemented to manage token expiration gracefully. Users are prompted to refresh their tokens when needed.

4. **Transaction Failures:** In the event of transaction failures during product purchases or deposit operations, the system is designed to handle such failures securely and provide appropriate feedback to users.

# Production Readiness

## Logging

### Overview
A robust logging strategy is employed to capture relevant information and events within the application. This aids in monitoring, debugging, and identifying potential security issues.

### Logging Strategy
1. **Request and Response Logging:** The implemented logging middleware captures details of each incoming request and its corresponding response. This includes request method, body, URL, user information, and response details.

2. **Exception Logging:** Unhandled exceptions are logged with details such as the endpoint, user, request method, path, and exception details. This information is crucial for diagnosing and addressing issues promptly.


## Exception Handling

### Overview
Exception handling is a critical aspect of the project to ensure that unexpected issues are handled gracefully without compromising the security and stability of the application.

### Exception Handling Strategy
1. **Try-Catch Blocks:** Relevant portions of the code are enclosed within try-catch blocks to capture and handle exceptions. This prevents unhandled exceptions from propagating up the call stack.

2. **Centralized Exception Logging:** Unhandled exceptions are logged centrally, providing a comprehensive view of issues across the application. This facilitates quick identification and resolution of potential problems.

3. **User-Friendly Error Responses:** Where applicable, user-friendly error responses are generated to communicate issues effectively to end-users without revealing sensitive information.



