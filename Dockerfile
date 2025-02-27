# Use official .NET SDK as a build environment
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Use .NET SDK image to build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["erpv0.1.csproj", "./"]
RUN dotnet restore "./erpv0.1.csproj"

# Copy the entire project
COPY . .
WORKDIR "/src/."
RUN dotnet build "erpv0.1.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "erpv0.1.csproj" -c Release -o /app/publish

# Use the ASP.NET Core runtime for the final stage
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "erpv0.1.dll"]
