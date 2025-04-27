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
    "Host=localhost;Database=coworking_db;Username=postgres;Password=postgres;" \
    Npgsql.EntityFrameworkCore.PostgreSQL \
    -o Models/DataModels/ \
    --context-dir Data \
    --context CoworkingDbContext \
    --force \
    --data-annotations
```

dotnet ef dbcontext scaffold "Host=localhost;Database=coworking_db;Username=postgres;Password=postgres;" Npgsql.EntityFrameworkCore.PostgreSQL -o Models/DataModels/ --context-dir Data --context CoworkingDbContext --force --data-annotations

# NOTES

CHANGE THE PRICE PER HOUR TO PRICE PER DAY!!!

Should there be a difference between data models and admin DTOs?
Should admin DTOs be the same as data models?

## Tools

Links:
- [[https://www.jstoolset.com/jwt]]


## Style

Services return data models (EF data models). 
It's up to the action of the controller to map the served resource as an admin or user DTO.


