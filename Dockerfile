FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar todo primero
COPY . .

# Restaurar y publicar directamente (sin build intermedio)
WORKDIR /src/SmartAcademicAdvisor.Web
RUN dotnet restore SmartAcademicAdvisor.Web.csproj || true
RUN dotnet publish -c Release -o /app/publish /p:ErrorOnDuplicatePublishOutputFiles=false /p:TreatWarningsAsErrors=false || dotnet publish -c Release -o /app/publish --no-restore /p:ContinueOnError=true

# Etapa final - imagen de runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .

# Configurar puerto para Render
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "SmartAcademicAdvisor.Web.dll"]l"]