# Chat Application API System

## Table of Contents
- [Project Overview](#project-overview)
- [Technologies Used](#technologies-used)
- [Key Features](#key-features)
- [Architecture](#architecture)
- [System Design Diagram](#system-design-diagram)
- [Setup and Installation](#setup-and-installation)
- [Usage](#usage)
- [API Endpoints](#api-endpoints)
- [Testing](#testing)
- [Contributing](#contributing)
- [License](#license)

## Project Overview
The **Chat Application API System** is a microservices-based application designed to facilitate real-time messaging between users. Built using .NET 8, this system supports one-to-one and group chats, leveraging technologies like SignalR, Redis for caching, and Hangfire for task scheduling. The application emphasizes efficiency and scalability, making it suitable for modern chat applications.

## Technologies Used
- **Backend Framework:** .NET 8
- **Database:** SQL Server
- **Caching:** Redis / Azure Caching
- **Real-time Communication:** SignalR
- **Inter-Service Communication:** gRPC
- **Task Scheduling:** Hangfire
- **Authentication:** OAuth
- **Containerization:** Docker
- **API Management:** Ocelot API Gateway
- **Testing Framework:** xUnit

## Key Features
- **User Registration:** Users can sign up using their mobile numbers.
- **Messaging:** Supports one-to-one chats, group chats, and one-to-group chats.
- **Message Caching:** Messages are stored in Redis cache for 45 minutes if the user is offline.
- **Task Scheduling:** Hangfire is used to sync message data from cache to the database after 45 minutes.
- **Fast User Experience:** Unique user IDs are stored in cache for faster message delivery until disconnection.
- **Queue Data Structure:** Efficiently handles message insertion and retrieval.
- **Service Interconnectivity:** Utilizes gRPC for quick communication between the Message Management Service and Chat Hub, alongside RESTful APIs.
- **Containerization:** Deployed with Docker for easy management and scalability.
- **High Test Coverage:** Achieved a 99% pass rate on unit tests.

## Architecture
This project follows a microservices architecture where each service is responsible for specific functionalities, ensuring modularity and ease of maintenance. The architecture includes:

- **API Gateway:** Ocelot to route requests to the appropriate services.
- **Message Management Service:** Handles message processing and storage.
- **User Service:** Manages user registration and authentication.
- **Notification Service:** Sends real-time updates to users.

## System Design Diagram
![System Design Diagram](https://github.com/amalprasad0/hudyio-microservices-console-app/blob/master/CHATAPP%20SYSTEM%20DESIGN.png?raw=true)

## Setup and Installation
To set up and run the Chat Application API System locally, follow these steps:

1. **Clone the Repository:**
   ```bash
   git clone https://github.com/amalprasad0/chat-application-api.git
   cd chat-application-api
