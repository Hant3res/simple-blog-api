#!/bin/bash
echo "Запуск всех тестов..."
dotnet test tests/SimpleBlog.API.Tests/SimpleBlog.API.Tests.csproj --verbosity normal

echo ""
echo "Запуск только unit тестов..."
dotnet test tests/SimpleBlog.API.Tests/SimpleBlog.API.Tests.csproj --filter "FullyQualifiedName~Controllers" --verbosity normal

echo ""
echo "Запуск только интеграционных тестов..."
dotnet test tests/SimpleBlog.API.Tests/SimpleBlog.API.Tests.csproj --filter "FullyQualifiedName~IntegrationTests" --verbosity normal
