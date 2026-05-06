FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["backend/DriveNow/DriveNow.csproj", "backend/DriveNow/"]
RUN dotnet restore "backend/DriveNow/DriveNow.csproj"

COPY . .
WORKDIR "/src/backend/DriveNow"
RUN dotnet publish "DriveNow.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "DriveNow.dll"]
