# ctcom.product-service

# Product Microservice

## Overview

The **Product Microservice** is part of the `ctcom` platform and handles product-related operations such as creating, updating, deleting, and retrieving products. It follows a microservice architecture with Event-Driven Architecture (EDA) principles using Kafka (or RabbitMQ) for event messaging.

This service is built using **.NET 8 Web API**, adhering to **SOLID principles** and ensuring scalability, separation of concerns, and ease of maintenance.

## Features

- **CRUD Operations**: Create, Read, Update, and Delete products.
- **Event Publishing**: Events such as `ProductCreated`, `ProductUpdated`, and `ProductDeleted` are published using Kafka/RabbitMQ.
- **Decoupled Design**: Follows SOLID principles, ensuring a clean and maintainable codebase.
- **In-memory Data Store**: Currently using an in-memory list for product data storage. You can integrate with a persistent database (e.g., SQL Server, MongoDB).

## Technology Stack

- **.NET 8 Web API**
- **Dependency Injection**
- **Docker** (for containerization)
- **Polly** (for resilience and retry logic)
- **RabbitMQ** (for message/event handling)
- **MassTransit** (for managing RabbitMQ)

## Endpoints

### Base URL: `/api/products`

| HTTP Method | Endpoint       | Description                |
| ----------- | -------------- | -------------------------- |
| GET         | `/`          | Retrieve all products      |
| GET         | `/{id:guid}` | Retrieve a product by ID   |
| POST        | `/`          | Create a new product       |
| PUT         | `/{id:guid}` | Update an existing product |
| DELETE      | `/{id:guid}` | Delete a product by ID     |

### Sample Request Payloads

#### Create Product (POST `/api/products`)

```json
{
  "name": "Sample Product",
  "description": "This is a sample product.",
  "price": 99.99,
  "stock": 100
}
```

## structure

```
ctcom.ProductService
│
├── Controllers
│ └── ProductController.cs
├── Models
│ └── Product.cs
├── Repositories
│ └── IProductRepository.cs
│ └── ProductRepository.cs
├── Services
│ └── IProductService.cs
│ └── ProductService.cs
├── Events
│ └── ProductCreatedEvent.cs
│ └── ProductUpdatedEvent.cs
│ └── ProductDeletedEvent.cs
├── Messaging
│ └── IMessageProducer.cs
│ └── KafkaMessageProducer.cs (or RabbitMQMessageProducer.cs)
└── Program.cs
```
