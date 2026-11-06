# Despliegue en Render (Opción Sencilla y Gratuita)

Render es similar a Railway pero con plan gratuito más generoso.

## Por qué Render?
- ✅ **100% GRATIS** (tier free permanente)
- ✅ Soporta .NET directamente
- ✅ Despliegue automático desde GitHub
- ✅ SSL gratuito
- ✅ Muy fácil de usar

## Paso 1: Preparar el proyecto

1. Crea un archivo `render.yaml` en la raíz:

```yaml
services:
  - type: web
    name: smart-academic-advisor
    env: docker
    plan: free
    buildCommand: dotnet publish SmartAcademicAdvisor.Web/SmartAcademicAdvisor.Web.csproj -c Release -o out
    startCommand: dotnet out/SmartAcademicAdvisor.Web.dll --urls http://0.0.0.0:$PORT
    envVars:
      - key: ASPNETCORE_ENVIRONMENT
        value: Production
```

2. O crea un `Dockerfile` (más control):

```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY SmartAcademicAdvisor.Web/SmartAcademicAdvisor.Web.csproj SmartAcademicAdvisor.Web/
RUN dotnet restore SmartAcademicAdvisor.Web/SmartAcademicAdvisor.Web.csproj
COPY . .
WORKDIR /src/SmartAcademicAdvisor.Web
RUN dotnet build -c Release -o /app/build
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "SmartAcademicAdvisor.Web.dll"]
```

## Paso 2: Subir a GitHub

```powershell
cd "c:\Users\bumbl\Downloads\Nueva carpeta\SmartAcademicAdvisor_UI"

git init
git add .
git commit -m "Deploy to Render"
git remote add origin https://github.com/TU-USUARIO/SmartAcademicAdvisor.git
git push -u origin main
```

## Paso 3: Desplegar en Render

1. Ve a https://render.com
2. Regístrate con GitHub
3. Click "New +" → "Web Service"
4. Conecta tu repositorio
5. Configura:
   - **Name**: smart-academic-advisor
   - **Environment**: Docker
   - **Branch**: main
   - **Build Command**: (auto-detectado del Dockerfile)
   - **Start Command**: (auto-detectado del Dockerfile)
6. Click "Create Web Service"

## Paso 4: Esperar despliegue

Render:
1. Clonará tu repo
2. Construirá la imagen Docker
3. Desplegará automáticamente
4. Te dará una URL: `https://smart-academic-advisor.onrender.com`

## Limitaciones del plan Free

⚠️ **Importante**: El tier free "duerme" después de 15 min de inactividad
- Primera carga después de inactividad: ~30 segundos
- Ideal para demos y prototipos
- Para producción: upgrade a plan pagado ($7/mes)

## Ver logs

```powershell
# En el dashboard de Render
Logs → View Logs (tiempo real)
```

## Alternativa: Sin Dockerfile

Si no quieres usar Docker, configura en Render:

**Build Command:**
```bash
dotnet publish SmartAcademicAdvisor.Web/SmartAcademicAdvisor.Web.csproj -c Release -o out
```

**Start Command:**
```bash
cd out && dotnet SmartAcademicAdvisor.Web.dll
```

**Environment:**
- Runtime: .NET

## Actualizar despliegue

```powershell
# Simplemente haz push a GitHub
git add .
git commit -m "Update"
git push

# Render redesplega automáticamente
```
