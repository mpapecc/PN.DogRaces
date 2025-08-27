# How to start project
## Summary
This is dev task related to dog racing system. Funcionalities:

- Generating number of rounds in time intervals 
- Processing rounds in time intervals 
- Always keeping some number of rounds active for beting 
- Placing bets
- Processing bets
- Processing tickets and payments (primitive in-memory wallet) 
- Notifying users via websockets about events such as round start, finish etc.

Periodical functinalities are combination of BackgroundService's (HostedService) + Timers. 
Server-Client realtime communication is solved with SignalR strongly typed hubs. 

Architectural design is simple implementation of modular monolith. Solution consits of 6 projects:

- Web => entry point of app
- RoundsModule
- TicketModule
- PaymentModule
- CommonModule
  
Modules are logicaly separated, data isolation is achieved withing same databae but with different schemas.

## Prerequisites
.NET8, SQL Server

If you are using Visual studio open Solution and position yourselfe into root in PMC.

## Runing Migration
Add-Migrations <mig-name> -context RoundModuleDbContext -o Infrastructure\DataContext\Migrations -p PlayNirvana.RoundModule
Update-Database -context RoundModuleDbContext

Add-Migrations <mig-name> -context TicketModuleDbContext -o Infrastructure\DataContext\Migrations -p PlayNirvana.TicketModule
Update-Database -context TicketModuleDbContext

## Running Application
Start it from VS.

# Docker

In root of project run :

docker compose -f docker.compose.yaml -d and acess Swagger on port 8080

# Output
Round and bets processing can be seen in terminal that opens when runnig project in VS 
(not working with others IDE but I guess they all have some sort of debug terminal) or by querying database.
Currently there is not UI for this app but maybe in future I will make implementation for it.
