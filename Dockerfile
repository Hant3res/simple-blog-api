FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/SimpleBlog.API/SimpleBlog.API.csproj", "src/SimpleBlog.API/"]
RUN dotnet restore "src/SimpleBlog.API/SimpleBlog.API.csproj"
COPY . .
WORKDIR "/src/src/SimpleBlog.API"
RUN dotnet build "SimpleBlog.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SimpleBlog.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SimpleBlog.API.dll"]
