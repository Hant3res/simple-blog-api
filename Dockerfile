# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files
COPY ["src/SimpleBlog.API/SimpleBlog.API.csproj", "src/SimpleBlog.API/"]
COPY ["tests/SimpleBlog.API.Tests/SimpleBlog.API.Tests.csproj", "tests/SimpleBlog.API.Tests/"]

# Restore dependencies
RUN dotnet restore "src/SimpleBlog.API/SimpleBlog.API.csproj"
RUN dotnet restore "tests/SimpleBlog.API.Tests/SimpleBlog.API.Tests.csproj"

# Copy everything else
COPY . .

# Build
WORKDIR "/src/src/SimpleBlog.API"
RUN dotnet build "SimpleBlog.API.csproj" -c Release -o /app/build

# Publish
FROM build AS publish
RUN dotnet publish "SimpleBlog.API.csproj" -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 80
EXPOSE 443
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SimpleBlog.API.dll"]
