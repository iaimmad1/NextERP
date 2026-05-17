# NextERP – ERP + E-Commerce Platform

> **A cloud-ready, multi-tenant ERP and e-commerce platform** built with ASP.NET Core 9 + React 19.

---

## Table of Contents

1. [Project Overview](#1-project-overview)
2. [Tech Stack](#2-tech-stack)
3. [What Has Been Completed](#3-what-has-been-completed)
4. [What Still Needs to Be Built (MVP)](#4-what-still-needs-to-be-built-mvp)
5. [Development Roadmap](#5-development-roadmap)
6. [Project Structure](#6-project-structure)
7. [Getting Started](#7-getting-started)
8. [API Endpoints Reference](#8-api-endpoints-reference)
9. [Authentication Flow](#9-authentication-flow)
10. [Deployment Guidelines](#10-deployment-guidelines)
11. [Contributing](#11-contributing)

---

## 1. Project Overview

**Goal:** Build a cloud-ready, multi-tenant ERP + e-commerce platform.

The MVP focuses on:

- ✅ Company registration with admin user (multi-tenant)
- ✅ Role-based & permission-based access control (RBAC)
- ✅ Product catalog, shopping cart, and checkout (simulated payment)
- ✅ Order management for admin
- ✅ Clean, responsive UI using React + Bootstrap

---

## 2. Tech Stack

| Layer | Technology |
|---|---|
| **Backend** | ASP.NET Core 9, CQRS (MediatR), EF Core, JWT |
| **Database** | SQL Server (local for dev) |
| **Frontend** | React 19, React Router 7, Bootstrap 5, native `fetch` |
| **Auth** | JWT access + refresh tokens, role/permission claims |
| **Hosting** | TBD (Azure / AWS / Render) |

---

## 3. What Has Been Completed

### Backend (ASP.NET Core)

**✅ Authentication & Authorization**
- JWT authentication with access + refresh tokens
- `JwtService` generates tokens including roles and permissions as claims
- `CurrentUserService` reads claims from `HttpContext`
- Authorization policies registered for all permissions (e.g., `users.view`, `roles.create`)
- `IAuthorizedRequest` interface + `AuthorizationBehavior` for MediatR pipeline

**✅ CQRS & MediatR**
- Commands and queries separated by feature (Auth, Users, Roles, Permissions)
- Handlers implemented for:
  - `RegisterCompanyCommand` (creates tenant, admin user, admin role, assigns all permissions)
  - `LoginCommand`, `RefreshTokenCommand`, `LogoutCommand`
  - Full user CRUD: Create, Update, Delete, Assign/Remove Role, Activate/Deactivate
  - Full role CRUD: Create, Update, Delete, Assign/Remove Permission
  - Queries: GetCurrentUser, GetUserById, GetUsersByTenant, GetRolesByTenant, GetRoleById, GetAllPermissions, GetUserPermissions

**✅ Entities & Database**
- `UserModel`, `TenantModel`, `RoleModel`, `PermissionModel`, `UserRoleModel`, `RolePermissionModel`, `RefreshTokenModel`, `AuditLogModel`
- Primary keys: `int` (auto-increment)
- String-based enums (`Gender`, `UserStatus`, `PermissionCategory`)
- `ApplicationDbContext` with full configurations and migrations applied

**✅ Repository & Error Handling**
- Generic `IRepository<T>` and `Repository<T>` with standard CRUD
- Global exception middleware returns consistent `ApiResponse<T>` with data sanitization (JWT masking)
- Custom exceptions (`NotFoundException`, `ForbiddenException`, `ValidationException`, etc.)

**✅ E-Commerce Backend (Phase 2)**
- `ProductModel`, `OrderModel`, and `OrderItemModel` entities
- Full CRUD for products with permission gating
- Order management logic (Create, List, Update Status)

### Frontend (React)

**✅ Authentication Flow**
- Login page (`/login`) — calls `/api/auth/login`, stores tokens + user in `localStorage`
- Registration page (`/register`) — calls `/api/auth/register`, auto-login after success
- `apiFetch` wrapper (no Axios) — injects `Authorization` header, handles 401 → refresh → retry with promise queue
- Logout — clears storage and calls `/api/auth/logout`

**✅ Route Protection**
- `AuthGuard` component checks for access token; redirects to `/login` if missing
- Protected routes: Dashboard (`/`), Users (`/users`)

**✅ Admin Management Pages**
- Dashboard — displays welcome + current user info (roles, permissions, tenant) from `localStorage`
- Users Management — integrated with `GET /api/users`, paginated table with create/edit/delete, role assignment, and status toggling
- Roles & Permissions — full CRUD for roles with categorized permission assignment
- Products Management — admin table for catalog items (SKU, price, stock, images)
- Orders Management — admin table to track and update order status (Pending, Processing, Shipped)

**✅ E-Commerce Shopping Experience**
- Product Catalog (`/`) — grid view for users to search and add products to cart
- Persistent Cart — React context syncing with `localStorage` for cart state
- Checkout Flow — address and payment selection generating authenticated orders

**✅ Proxy & CORS**
- `src/setupProxy.js` using `http-proxy-middleware` v3 proxies `/api/*` to `https://localhost:7129`
- Backend `Program.cs` configured with `UseCors` allowing `http://localhost:3000`

---

## 4. What Still Needs to Be Built (MVP)

### Frontend – High Priority

| Page / Feature | Description | Permissions Needed |
|---|---|---|
| **Settings / Company Profile** | Update company name, address, phone, logo | `tenants.edit` |
| **Dashboard (complete)** | Real stats: user count, product count, order count, revenue | — |
| **Responsive Sidebar** | Collapsible sidebar (offcanvas on mobile) with active link highlighting | — |

### Backend – Missing Endpoints

| Entity | Required Endpoints |
|---|---|
| **Dashboard** | Statistics summary endpoint (users, orders, revenue) |
| **Tenant** | Update tenant details endpoint |

---

## 5. Development Roadmap

### Week 1 – Core Admin UI
- **Day 1–2:** Complete Users Management UI (list, create, edit, delete, assign roles, toggle status)
- **Day 3–4:** Complete Roles & Permissions UI (list, create, edit, permission assignment with checkboxes)
- **Day 5:** Dashboard stats (integrate with backend summary endpoint)

### Week 2 – Product & Shopping (Completed)
- **Day 1–2:** Products CRUD — backend entities + CQRS + frontend card grid
- **Day 3–4:** Cart & Checkout flow (frontend cart via React context, create order endpoint)
- **Day 5:** Order management UI for admin (list, detail modal, status update)

### Week 3 – Polish & Deploy (In Progress)
- **Day 1–2:** Settings page, company profile update, responsive sidebar
- **Day 3:** Testing (manual + unit/integration tests for critical paths)
- **Day 4:** Deploy backend (Azure App Service / DigitalOcean), frontend (Vercel / Netlify)
- **Day 5:** API docs, user manual, and handover

---

## 6. Project Structure

```
NextERP/
├── backend/
│   └── NextERP/
│       └── NextERP/
│           ├── Controllers/          # Auth, Users, Roles, Permissions
│           ├── Features/             # CQRS Commands & Queries per feature
│           │   ├── Auth/
│           │   ├── Users/
│           │   ├── Roles/
│           │   └── Permissions/
│           ├── Common/               # DTOs, Behaviours, Constants
│           ├── Core/                 # Interfaces, Domain Entities
│           ├── Infrastructure/       # EF Core, Repositories, JWT, Services
│           ├── Extensions/           # ServiceExtensions (DI setup)
│           ├── Middleware/           # Global exception handler
│           ├── Migrations/           # EF Core migrations
│           └── Program.cs
│
└── frontend/
    └── nexterp/
        ├── public/
        └── src/
            ├── components/           # Reusable components
            │   ├── AuthGuard.jsx
            │   └── Navbar.jsx
            ├── pages/                # Page-level components
            │   ├── Login.jsx
            │   ├── Register.jsx
            │   ├── Dashboard.jsx
            │   ├── Users.jsx
            │   ├── Roles.jsx         # [TODO]
            │   ├── Products.jsx      # [TODO]
            │   ├── Orders.jsx        # [TODO]
            │   ├── Cart.jsx          # [TODO]
            │   └── Settings.jsx      # [TODO]
            ├── services/             # API service layer
            │   ├── api.js            # apiFetch + token helpers
            │   └── authService.js    # Auth, user, role API calls
            ├── context/              # React contexts
            │   └── CartContext.jsx   # [TODO] – cart state
            ├── App.js                # Router + route definitions
            ├── index.js
            └── setupProxy.js         # Dev proxy to backend
```

---

## 7. Getting Started

### Prerequisites
- Node.js ≥ 18
- .NET 9 SDK
- SQL Server (local) or SQL Server Express

### Backend Setup

```bash
cd backend/NextERP/NextERP

# Restore packages
dotnet restore

# Apply migrations
dotnet ef database update

# Run the API (listens on https://localhost:7129 and http://localhost:5290)
dotnet run
```

### Frontend Setup

```bash
cd frontend/nexterp

# Install dependencies
npm install

# Start development server (proxies /api to https://localhost:7129)
npm start
```

The app will open at **http://localhost:3000**.

> **Note:** Ensure the backend is running before starting the frontend. The proxy in `src/setupProxy.js` forwards all `/api/*` requests to the backend automatically.

### First Run
1. Navigate to `http://localhost:3000/register`
2. Create your company and admin account
3. You will be automatically logged in and redirected to the Dashboard

---

## 8. API Endpoints Reference

All endpoints are relative URLs proxied through the dev server:

| Method | Endpoint | Description | Auth Required |
|---|---|---|---|
| `POST` | `/api/auth/register` | Register company + admin user | No |
| `POST` | `/api/auth/login` | Login | No |
| `POST` | `/api/auth/refresh` | Refresh access token | No |
| `POST` | `/api/auth/logout` | Logout (revoke refresh token) | Yes |
| `GET` | `/api/auth/current` | Get current user (roles & permissions) | Yes |
| `GET` | `/api/users` | List users (paginated) | `users.view` |
| `GET` | `/api/users/{id}` | Get user by ID | `users.view` |
| `POST` | `/api/users` | Create user | `users.create` |
| `PUT` | `/api/users/{id}` | Update user | `users.edit` |
| `DELETE` | `/api/users/{id}` | Delete user | `users.delete` |
| `PATCH` | `/api/users/{id}/status` | Activate / Deactivate user | `users.manage` |
| `POST` | `/api/users/{userId}/roles/{roleId}` | Assign role to user | `roles.assign` |
| `DELETE` | `/api/users/{userId}/roles/{roleId}` | Remove role from user | `roles.assign` |
| `GET` | `/api/roles` | List roles | `roles.view` |
| `GET` | `/api/roles/{id}` | Get role with permissions | `roles.view` |
| `POST` | `/api/roles` | Create role | `roles.create` |
| `PUT` | `/api/roles/{id}` | Update role | `roles.edit` |
| `DELETE` | `/api/roles/{id}` | Delete role | `roles.delete` |
| `POST` | `/api/roles/{roleId}/permissions/{permissionId}` | Grant permission to role | `roles.assign` |
| `DELETE` | `/api/roles/{roleId}/permissions/{permissionId}` | Revoke permission from role | `roles.assign` |
| `GET` | `/api/permissions` | All permissions (grouped by category) | Yes |
| `GET` | `/api/permissions/my-permissions` | Current user's permissions | Yes |

### API Response Format

All responses follow a consistent envelope:

```json
{
  "success": true,
  "message": "Operation successful",
  "data": { ... },
  "errors": [],
  "statusCode": 200,
  "timestamp": "2026-05-15T12:00:00Z"
}
```

---

## 9. Authentication Flow

```
User submits login form
       ↓
POST /api/auth/login
       ↓
Store accessToken + refreshToken + user in localStorage
       ↓
Every API request → attach Authorization: Bearer <accessToken>
       ↓
On 401 response → POST /api/auth/refresh (with refreshToken)
       ↓ (success)            ↓ (failure)
Update tokens in storage   Clear storage → redirect to /login
Retry original request
```

---

## 10. Deployment Guidelines

### Backend
- Publish to **Azure App Service**, **AWS Elastic Beanstalk**, or **DigitalOcean droplet**
- Use **Azure SQL** or **AWS RDS** for SQL Server in production
- Store JWT secret and connection string in **environment variables** (never commit to source)

### Frontend
- Build with `npm run build`
- Deploy to **Vercel**, **Netlify**, or **Azure Static Web Apps**
- Set `REACT_APP_API_BASE_URL` environment variable for the production API URL

### Security Checklist
- [ ] Enforce HTTPS on all endpoints
- [ ] Restrict CORS to the frontend domain only
- [ ] Set short JWT expiry (15 min access / 7 days refresh)
- [ ] Enable rate limiting on `/login` and `/register`
- [ ] Implement account lockout after 5 failed attempts

---

## 11. Contributing

1. Branch naming: `feature/<name>`, `fix/<name>`, `chore/<name>`
2. All API calls must use the `apiFetch` wrapper (no direct `fetch` calls in components)
3. Follow the existing CQRS pattern for any new backend features
4. Always handle loading states and error messages in UI components
5. Use Bootstrap + React-Bootstrap components only (no custom CSS unless strictly necessary)
