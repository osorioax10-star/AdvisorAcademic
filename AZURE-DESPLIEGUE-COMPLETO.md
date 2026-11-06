# 🚀 DESPLIEGUE COMPLETO EN AZURE - PASO A PASO

## ✅ VENTAJAS DE AZURE PARA TU PROYECTO

- 🎓 **Gratis con cuenta estudiantil** ($100 créditos)
- 🏢 **Más profesional** (usado en empresas)
- ⚡ **Mejor rendimiento** que opciones gratuitas
- 🔒 **Más seguro y escalable**
- 📊 **Integración con Visual Studio**

---

## 📋 REQUISITOS PREVIOS

### 1. Cuenta de Azure

**Opción A - Estudiante (GRATIS $100):**
- Ve a: https://azure.microsoft.com/es-es/free/students/
- Regístrate con tu correo institucional (@tese.edu.mx)
- No necesitas tarjeta de crédito
- $100 USD de crédito gratis

**Opción B - Cuenta gratuita:**
- Ve a: https://azure.microsoft.com/es-es/free/
- Regístrate (requiere tarjeta para verificación)
- 12 meses gratis de varios servicios

### 2. Instalar herramientas

```powershell
# Verificar si tienes .NET instalado
dotnet --version

# Si no tienes .NET 8, descárgalo de:
# https://dotnet.microsoft.com/download/dotnet/8.0
```

---

## 🎯 MÉTODO 1: DESPLIEGUE CON VISUAL STUDIO (MÁS FÁCIL)

### Paso 1: Abrir el proyecto en Visual Studio

1. Abre **Visual Studio 2022**
2. **File** → **Open** → **Project/Solution**
3. Navega a: `C:\Users\bumbl\Downloads\Nueva carpeta\SmartAcademicAdvisor_UI`
4. Selecciona `SmartAcademicAdvisor.Web.csproj`

### Paso 2: Publicar a Azure

1. En el **Solution Explorer**, clic derecho en el proyecto
2. Selecciona **"Publish..."**
3. En la ventana:
   - Target: **Azure**
   - Specific target: **Azure App Service (Windows)**
4. Click **Next**
5. **Sign in** con tu cuenta de Azure
6. Click **"Create a new Azure App Service"** (signo +)
7. Configuración:
   ```
   Name: smart-academic-advisor
   Subscription: Azure for Students (o tu suscripción)
   Resource Group: (Create new) → SmartAdvisorRG
   Hosting Plan: (Create new) → SmartAdvisorPlan
     - Location: East US
     - Size: F1 (Free)
   ```
8. Click **Create**
9. Click **Finish**
10. Click **Publish** (botón verde)

### Paso 3: ¡Espera 2-3 minutos!

Visual Studio:
- ✓ Compilará tu proyecto
- ✓ Lo empaquetará
- ✓ Lo subirá a Azure
- ✓ Abrirá el navegador automáticamente

Tu app estará en:
```
https://smart-academic-advisor.azurewebsites.net
```

---

## 🎯 MÉTODO 2: DESPLIEGUE CON AZURE CLI (MÁS CONTROL)

### Paso 1: Instalar Azure CLI

```powershell
# Instalar con winget
winget install Microsoft.AzureCLI

# O descargar desde:
# https://aka.ms/installazurecliwindows
```

Reinicia PowerShell después de instalar.

### Paso 2: Login a Azure

```powershell
# Iniciar sesión
az login

# Se abrirá el navegador, ingresa tus credenciales

# Verificar suscripción
az account show
```

### Paso 3: Crear recursos en Azure

```powershell
# Variables (personaliza si quieres)
$resourceGroup = "SmartAdvisorRG"
$location = "eastus"
$planName = "SmartAdvisorPlan"
$appName = "smart-academic-advisor-2024"

# 1. Crear grupo de recursos
az group create --name $resourceGroup --location $location

# 2. Crear App Service Plan (gratis)
az appservice plan create `
  --name $planName `
  --resource-group $resourceGroup `
  --sku FREE `
  --location $location

# 3. Crear Web App
az webapp create `
  --name $appName `
  --resource-group $resourceGroup `
  --plan $planName `
  --runtime "DOTNET|8.0"

Write-Host "✓ Recursos creados exitosamente" -ForegroundColor Green
Write-Host "Tu app estará en: https://$appName.azurewebsites.net" -ForegroundColor Cyan
```

### Paso 4: Compilar y empaquetar

```powershell
# Ir a la carpeta del proyecto
cd "c:\Users\bumbl\Downloads\Nueva carpeta\SmartAcademicAdvisor_UI\SmartAcademicAdvisor.Web"

# Compilar
dotnet build -c Release

# Publicar (crear paquete)
dotnet publish -c Release -o ./publish

# Crear archivo ZIP
Compress-Archive -Path ./publish/* -DestinationPath ./deploy.zip -Force

Write-Host "✓ Paquete creado: deploy.zip" -ForegroundColor Green
```

### Paso 5: Desplegar a Azure

```powershell
# Desplegar el ZIP
az webapp deployment source config-zip `
  --resource-group $resourceGroup `
  --name $appName `
  --src ./deploy.zip

Write-Host "✓ ¡Despliegue completado!" -ForegroundColor Green
Write-Host "Visita: https://$appName.azurewebsites.net" -ForegroundColor Cyan
```

---

## 🎯 MÉTODO 3: DESPLIEGUE DESDE GITHUB (AUTOMÁTICO)

### Paso 1: Subir tu código a GitHub

```powershell
cd "c:\Users\bumbl\Downloads\Nueva carpeta\SmartAcademicAdvisor_UI"

# Si no has inicializado git
git init
git add .
git commit -m "Ready for Azure deployment"

# Crear repo en GitHub y subirlo
git remote add origin https://github.com/TU-USUARIO/SmartAcademicAdvisor.git
git branch -M main
git push -u origin main
```

### Paso 2: Configurar GitHub Actions

```powershell
# En Azure CLI, obtener credenciales de publicación
az webapp deployment list-publishing-profiles `
  --name smart-academic-advisor-2024 `
  --resource-group SmartAdvisorRG `
  --xml > publishprofile.xml

# O desde el Portal de Azure:
# 1. Ve a tu App Service
# 2. Deployment Center → Settings
# 3. Source: GitHub
# 4. Autoriza y selecciona tu repositorio
# 5. Azure creará automáticamente el workflow
```

### Paso 3: Crear workflow manualmente (opcional)

Crea: `.github/workflows/azure-deploy.yml`

```yaml
name: Deploy to Azure

on:
  push:
    branches: [ main ]

env:
  AZURE_WEBAPP_NAME: smart-academic-advisor-2024
  DOTNET_VERSION: '8.0.x'

jobs:
  build-and-deploy:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      
      - name: Setup .NET
        uses: actions/setup-dotnet@v3
        with:
          dotnet-version: ${{ env.DOTNET_VERSION }}
      
      - name: Restore dependencies
        run: dotnet restore SmartAcademicAdvisor.Web/SmartAcademicAdvisor.Web.csproj
      
      - name: Build
        run: dotnet build SmartAcademicAdvisor.Web/SmartAcademicAdvisor.Web.csproj -c Release --no-restore
      
      - name: Publish
        run: dotnet publish SmartAcademicAdvisor.Web/SmartAcademicAdvisor.Web.csproj -c Release -o ./publish
      
      - name: Deploy to Azure
        uses: azure/webapps-deploy@v2
        with:
          app-name: ${{ env.AZURE_WEBAPP_NAME }}
          publish-profile: ${{ secrets.AZURE_WEBAPP_PUBLISH_PROFILE }}
          package: ./publish
```

---

## 🔧 CONFIGURACIÓN POST-DESPLIEGUE

### 1. Verificar que funcione

```powershell
# Abrir en navegador
Start-Process "https://smart-academic-advisor-2024.azurewebsites.net"
```

### 2. Ver logs en tiempo real

```powershell
az webapp log tail `
  --name smart-academic-advisor-2024 `
  --resource-group SmartAdvisorRG
```

### 3. Configurar variables de entorno (si necesitas)

**En Azure Portal:**
1. Ve a tu App Service
2. Settings → Configuration
3. Application settings → New application setting
4. Agrega: `ASPNETCORE_ENVIRONMENT = Production`

**Por CLI:**
```powershell
az webapp config appsettings set `
  --resource-group SmartAdvisorRG `
  --name smart-academic-advisor-2024 `
  --settings ASPNETCORE_ENVIRONMENT=Production
```

### 4. Configurar dominio personalizado (opcional)

**En Azure Portal:**
1. Settings → Custom domains
2. Add custom domain
3. Sigue las instrucciones de DNS

---

## 📊 MONITOREO Y DIAGNÓSTICO

### Ver métricas

```powershell
# CPU y memoria
az monitor metrics list `
  --resource "/subscriptions/[ID]/resourceGroups/SmartAdvisorRG/providers/Microsoft.Web/sites/smart-academic-advisor-2024" `
  --metric "CpuPercentage,MemoryPercentage"
```

### Descargar logs

```powershell
az webapp log download `
  --name smart-academic-advisor-2024 `
  --resource-group SmartAdvisorRG `
  --log-file logs.zip
```

---

## 🔄 ACTUALIZAR EL DESPLIEGUE

### Método Visual Studio:
1. Haz cambios en tu código
2. Clic derecho → Publish
3. Click Publish

### Método CLI:
```powershell
cd "c:\Users\bumbl\Downloads\Nueva carpeta\SmartAcademicAdvisor_UI\SmartAcademicAdvisor.Web"
dotnet publish -c Release -o ./publish
Compress-Archive -Path ./publish/* -DestinationPath ./deploy.zip -Force
az webapp deployment source config-zip --resource-group SmartAdvisorRG --name smart-academic-advisor-2024 --src ./deploy.zip
```

### Método GitHub:
```powershell
git add .
git commit -m "Update"
git push
# GitHub Actions despliega automáticamente
```

---

## ⚠️ SOLUCIÓN DE PROBLEMAS

### Error: "Application Error"

**Ver logs:**
```powershell
az webapp log tail --name smart-academic-advisor-2024 --resource-group SmartAdvisorRG
```

**Habilitar logs detallados:**
```powershell
az webapp log config `
  --name smart-academic-advisor-2024 `
  --resource-group SmartAdvisorRG `
  --application-logging filesystem `
  --detailed-error-messages true `
  --failed-request-tracing true `
  --web-server-logging filesystem
```

### Error: "dotnet not found"

Verifica la versión de runtime:
```powershell
az webapp config show `
  --name smart-academic-advisor-2024 `
  --resource-group SmartAdvisorRG `
  --query "netFrameworkVersion"
```

### Los archivos JSON no persisten

**Opción A - Azure Blob Storage:**
```powershell
# Crear storage account
az storage account create `
  --name smartadvisorstorage `
  --resource-group SmartAdvisorRG `
  --location eastus `
  --sku Standard_LRS
```

**Opción B - Azure SQL Database:**
Migrar de JSON a base de datos real.

---

## 💰 COSTOS (PLAN GRATUITO)

| Recurso | Tier | Costo |
|---------|------|-------|
| App Service | F1 (Free) | $0/mes |
| Resource Group | N/A | Gratis |
| Bandwidth | 1GB salida | Gratis |

**Limitaciones F1:**
- 60 minutos CPU/día
- 1 GB RAM
- 1 GB almacenamiento
- No custom domains SSL

**Para producción real:** Upgrade a B1 ($13/mes)

---

## 🎓 RECURSOS ADICIONALES

- [Documentación Azure App Service](https://docs.microsoft.com/azure/app-service/)
- [Azure for Students](https://azure.microsoft.com/students/)
- [Tutoriales ASP.NET Core en Azure](https://docs.microsoft.com/aspnet/core/tutorials/)

---

## ✅ CHECKLIST FINAL

- [ ] Cuenta de Azure creada
- [ ] Azure CLI instalado
- [ ] Proyecto compila localmente (`dotnet build`)
- [ ] Recursos creados en Azure
- [ ] App desplegada exitosamente
- [ ] URL funciona en navegador
- [ ] Logs revisados
- [ ] Variables de entorno configuradas

---

## 🆘 ¿NECESITAS AYUDA?

Si algo falla:
1. Revisa los logs: `az webapp log tail ...`
2. Verifica que el .csproj exista
3. Asegúrate de que compile localmente
4. Checa que la suscripción de Azure esté activa

**Comando de diagnóstico:**
```powershell
az webapp show --name smart-academic-advisor-2024 --resource-group SmartAdvisorRG
```
