# ResolveAI - Enterprise AI-Powered IT Service Management

ResolveAI is a professional portfolio product built with .NET 10 and SQL Server 2025. It uses AI (RAG) to automate IT support workflows.

## 🚀 Status: Phase 1, 2, 3 & 4 Completed
- [x] **Phase 1 (Identity):** User Registration, Login, and JWT Security.
- [x] **Phase 2 (Organization):** Departments and Teams management.
- [x] **Phase 3 (Ticketing):** Ticket creation with INC-XXXX numbering.
- [x] **Phase 4 (SLA Engine):** Automatic deadline calculation (High: 4h, Medium: 24h).

## 🧠 Smart Features Added
- **AI Classification:** Automatically detects "VPN" or "Network" issues and sets priority to 'High'.
- **SLA Engine:** Business rules implemented for response and resolution times.

## 🛠 Tech Stack
- **Backend:** .NET 10 (ASP.NET Core Web API)
- **Architecture:** Clean Architecture (Domain, Application, Infrastructure, API)
- **Database:** SQL Server 2025 (Native Vector Support)
- **Security:** ASP.NET Core Identity + JWT Bearer Tokens
- **Real-time:** SignalR (Planned)
- **AI:** Semantic Kernel + RAG (Planned)

## 🏗 Architecture Diagram
```mermaid
graph TD
    Browser[Browser / Postman] --> API[ASP.NET Core API]
    API --> Application[Application Layer]
    Application --> Domain[Domain Layer]
    Application --> Infrastructure[Infrastructure Layer]
    Infrastructure --> SQL[(SQL Server 2025)]