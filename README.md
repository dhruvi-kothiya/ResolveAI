# ResolveAI - Enterprise AI-Powered IT Service Management

ResolveAI is a professional, enterprise-style AI-powered IT Service Management (ITSM) platform built with **.NET 10** and **SQL Server 2025**.

The system is designed to automate IT support workflows using intelligent ticket classification, SLA management, secure authentication, and a scalable **Clean Architecture**.

---

## 🚀 Project Status

### Phase 1, 2, 3 & 4 Completed

- [x] **Phase 1 - Identity**
  - User Registration
  - User Login
  - ASP.NET Core Identity
  - JWT Authentication & Security

- [x] **Phase 2 - Organization**
  - Department Management
  - Team Management
  - Organization Structure

- [x] **Phase 3 - Ticketing**
  - Ticket Creation
  - Automatic `INC-XXXX` Ticket Numbering
  - Ticket Status Management
  - AI-based Ticket Classification

- [x] **Phase 4 - SLA Engine**
  - Automatic SLA Deadline Calculation
  - High Priority → 4 Hours
  - Medium Priority → 24 Hours
  - Automatic `DueAt` Calculation

- [X] **Phase 5 - Notifications**
  - Real-time Alerts
  - Email Notifications
  - SignalR Integration
  - SQL logging completed

## 🧠 Smart Features

### AI Priority Classification

ResolveAI automatically analyzes the ticket description and identifies common critical IT issues.

For example:

- `VPN` issue → **High Priority**
- `Network` issue → **High Priority**
- Other issues → **Medium Priority**

This helps IT teams identify critical tickets faster.

### SLA Engine

The SLA engine automatically calculates ticket deadlines based on priority:

| Priority | SLA |
|----------|-----|
| High | 4 Hours |
| Medium | 24 Hours |

The calculated deadline is stored in the `DueAt` field.

---

## 🛠 Tech Stack

### Backend
- .NET 10
- ASP.NET Core Web API
- C#

### Architecture
- Clean Architecture
- Domain Layer
- Application Layer
- Infrastructure Layer
- API Layer

### Database
- SQL Server 2025
- Entity Framework Core

### Security
- ASP.NET Core Identity
- JWT Bearer Authentication

### AI
- Semantic Kernel
- RAG (Retrieval-Augmented Generation)
- AI-powered ticket classification

### Real-Time Communication
- SignalR *(Planned)*

---

## 🏗 Architecture

ResolveAI follows **Clean Architecture** principles to keep the application scalable, maintainable, and loosely coupled.

```mermaid
graph TD             
    Browser[Browser / Postman] --> API[ASP.NET Core Web API]

    API --> Application[Application Layer]

    Application --> Domain[Domain Layer]

    Application --> Infrastructure[Infrastructure Layer]

    Infrastructure --> SQL[(SQL Server 2025)]