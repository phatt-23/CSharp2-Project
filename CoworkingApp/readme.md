# Project

## Features

### User

- User can register, login and logout of the system. 
    > The system is using cookies that are stored in the browser.
    > If the client is not using browser, they can manually store the 
    > access and optionally the refresh token on their own.

- User can make reservations, check their reservations and cancel them. 
    > The reservations can only be cancelled before they start.
    > User can only check their reservations. They don't see reservations of other users.
    
- User can check available coworking centers and their workspaces. 
See current pricing of the workspace. 

- User can view the status history of workspaces. 

### Admin

- Admin can assign roles to users.
- Admin can view all the reservations.
- Admin can add new coworking center, workspaces and update pricing of workspaces.

---

## Scaffolding from DB

```bash
dotnet ef dbcontext scaffold \
    "Host=localhost;Database=coworking_db;Username=coworking_user;Password=coworking_user;" \
    Npgsql.EntityFrameworkCore.PostgreSQL \
    -o Models/DataModels/ \
    --context-dir Data \
    --context CoworkingDbContext \
    --force \
    --data-annotations
```

---

## **🌍 Public API Endpoints (Users)**
| **Method** | **Endpoint** | **Description** |
|------------|-------------|-----------------|
| `GET` | `/api/workspaces` | Get a paginated list of workspaces with filtering options (status, coworking center, price, etc.). |
| `GET` | `/api/workspaces/{id}` | Get details of a specific workspace. |
| `GET` | `/api/workspaces/{id}/history` | Fetch the status history of a workspace. |
| `GET` | `/api/coworking-centers` | Get a list of coworking centers. |
| `GET` | `/api/workspace-statuses` | Get a list of possible workspace statuses. |
| `POST` | `/api/reservations` | Create a reservation for a workspace. |
| `GET` | `/api/reservations` | Get a paginated list of reservations for the authenticated user. |
| `GET` | `/api/reservations/{id}` | Get details of a specific reservation. |
| `DELETE` | `/api/reservations/{id}` | Cancel a reservation (if allowed). |

---

## **🛠️ Admin API Endpoints**
| **Method** | **Endpoint** | **Description** |
|------------|-------------|-----------------|
| `GET` | `/api/admin/workspaces` | Get a list of all workspaces (with admin privileges). |
| `POST` | `/api/admin/workspaces` | Create a new workspace. |
| `PUT` | `/api/admin/workspaces/{id}` | Update workspace details (name, description, coworking center, etc.). |
| `PUT` | `/api/admin/workspaces/{id}/history` | Get status history of a workspace. |
| `PUT` | `/api/admin/workspaces/{id}/status` | Change the status of a workspace. |
| `DELETE` | `/api/admin/workspaces/{id}` | Delete a workspace. |
| `GET` | `/api/admin/reservations` | Get all reservations (for management). |
| `PUT` | `/api/admin/reservations/{id}` | Modify a reservation (e.g., change timing). |
| `DELETE` | `/api/admin/reservations/{id}` | Force cancel a reservation. |
| `GET` | `/api/admin/pricing` | Get all pricing rules for workspaces. |
| `POST` | `/api/admin/pricing` | Set or update pricing for a workspace. |
| `GET` | `/api/admin/users` | Get a list of all users. |
| `PUT` | `/api/admin/users/{id}/role` | Change a user's role (admin/user). |
| `DELETE` | `/api/admin/users/{id}` | Delete a user. |

---

## **🔐 Authentication & User Management API**
| **Method** | **Endpoint** | **Description** |
|------------|-------------|-----------------|
| `POST` | `/api/auth/register` | Register a new user. |
| `POST` | `/api/auth/login` | Log in and receive a JWT token. |
| `GET` | `/api/auth/me` | Get details about the currently authenticated user. |
| `POST` | `/api/auth/logout` | Log out the current user. |
| `POST` | `/api/auth/refresh-tokens` | Refreshes JWT access token and refresh token, if refresh token is not expired. |


---

## Tools

Links:
- [[https://www.jstoolset.com/jwt]]


## Style

Services return data models (EF data models). 
It's up to the action of the controller to map the served resource as an admin or user DTO.


