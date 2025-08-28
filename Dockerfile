FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["PlayNirvana.Web/PlayNirvana.Web.csproj", "PlayNirvana.Web/"]
COPY ["Modules/PlayNirvana.CommonModule/PlayNirvana.CommonModule.csproj", "Modules/PlayNirvana.CommonModule/"]
COPY ["Modules/PlayNirvana.PaymentModule/PlayNirvana.PaymentModule.csproj", "Modules/PlayNirvana.PaymentModule/"]
COPY ["Modules/PlayNirvana.RoundModule/PlayNirvana.RoundModule.csproj", "Modules/PlayNirvana.RoundModule/"]
COPY ["Modules/PlayNirvana.TicketModule/PlayNirvana.TicketModule.csproj", "Modules/PlayNirvana.TicketModule/"]
RUN dotnet restore "./PlayNirvana.Web/PlayNirvana.Web.csproj"
COPY . .
WORKDIR "/src/PlayNirvana.Web"
RUN dotnet tool install --global dotnet-ef
ENV PATH="$PATH:/root/.dotnet/tools"
RUN dotnet build "./PlayNirvana.Web.csproj" -c Release -o /app/build
RUN dotnet publish "./PlayNirvana.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false
RUN dotnet ef migrations bundle --context TicketModuleDbContext -o /app/publish/ticketModuleMigrations.exe
RUN dotnet ef migrations bundle --context RoundModuleDbContext -o /app/publish/roundModuleMigrations.exe

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
EXPOSE 8080
EXPOSE 8081
COPY --from=build /app/publish .
