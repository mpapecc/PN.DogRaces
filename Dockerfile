FROM mcr.microsoft.com/dotnet/sdk:8.0
WORKDIR /usr/src/app
EXPOSE 8080

COPY . .
RUN dotnet restore "./PlayNirvana.Web/PlayNirvana.Web.csproj"

RUN dotnet tool install --global dotnet-ef
ENV PATH="$PATH:/root/.dotnet/tools"

RUN dotnet build -c Debug
RUN dotnet publish -c Debug -o "/publish" /p:UseAppHost=false
