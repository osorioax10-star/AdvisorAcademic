FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar el archivo .csproj y restaurar dependencias
COPY SmartAcademicAdvisor.Web/SmartAcademicAdvisor.Web.csproj SmartAcademicAdvisor.Web/
RUN dotnet restore SmartAcademicAdvisor.Web/SmartAcademicAdvisor.Web.csproj

# Copiar todo el código y compilar
COPY . .
WORKDIR /src/SmartAcademicAdvisor.Web
RUN dotnet build -c Release -o /app/build
RUN dotnet publish -c Release -o /app/publish

# Etapa final - imagen de runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .

# Configurar puerto para Render
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "SmartAcademicAdvisor.Web.dll"]l"]