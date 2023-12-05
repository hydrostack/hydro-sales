FROM mcr.microsoft.com/dotnet/aspnet:8.0

EXPOSE 8080
EXPOSE 8081

WORKDIR /app
COPY . ./
ENTRYPOINT ["dotnet", "HydroSales.dll"]
