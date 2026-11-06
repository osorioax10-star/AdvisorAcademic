# Etapa 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar el proyecto y restaurar dependencias
COPY SmartAcademicAdvisor.Web/SmartAcademicAdvisor.Web.csproj SmartAcademicAdvisor.Web/
RUN dotnet restore "SmartAcademicAdvisor.Web/SmartAcademicAdvisor.Web.csproj"

# Copiar todo el código y compilar
COPY SmartAcademicAdvisor.Web/ SmartAcademicAdvisor.Web/
WORKDIR /src/SmartAcademicAdvisor.Web
RUN dotnet build "SmartAcademicAdvisor.Web.csproj" -c Release -o /app/build

# Etapa 2: Publish
FROM build AS publish
RUN dotnet publish "SmartAcademicAdvisor.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Etapa 3: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Copiar la aplicación publicada
COPY --from=publish /app/publish .

# Crear el directorio Data si no existe
RUN mkdir -p /app/Data

# Render asigna el puerto dinámicamente en la variable PORT
ENV ASPNETCORE_URLS=http://+:${PORT:-80}
EXPOSE ${PORT:-80}

ENTRYPOINT ["dotnet", "SmartAcademicAdvisor.Web.dll"]
