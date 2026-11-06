# Despliegue en Azure App Service

⚠️ **NOTA:** Para una guía más completa y actualizada, consulta: `AZURE-DESPLIEGUE-COMPLETO.md`

## Paso 1: Preparar el proyecto

1. Asegúrate de que tu proyecto compile:
```powershell
cd "c:\Users\bumbl\Downloads\Nueva carpeta\SmartAcademicAdvisor_UI\SmartAcademicAdvisor.Web"
dotnet build
```

2. Crea un archivo `.gitignore` si no existe:
```
bin/
obj/
*.user
.vs/
```

## Paso 2: Instalar Azure CLI

```powershell
# Instalar Azure CLI
winget install Microsoft.AzureCLI

# O descargar desde: https://aka.ms/installazurecliwindows
```

## Paso 3: Login y crear recursos

```powershell
# Login a Azure
az login

# Crear grupo de recursos
az group create --name SmartAdvisorRG --location eastus

# Crear App Service Plan (gratis)
az appservice plan create --name SmartAdvisorPlan --resource-group SmartAdvisorRG --sku FREE

# Crear Web App
az webapp create --name smart-academic-advisor-2024 --resource-group SmartAdvisorRG --plan SmartAdvisorPlan --runtime "DOTNET|8.0"
```

## Paso 4: Configurar despliegue desde Git

```powershell
# Obtener URL de Git deployment
az webapp deployment source config-local-git --name smart-academic-advisor-2024 --resource-group SmartAdvisorRG

# Te dará una URL como:
# https://smart-academic-advisor-2024.scm.azurewebsites.net/smart-academic-advisor-2024.git
```

## Paso 5: Desplegar

```powershell
# En tu carpeta del proyecto
cd "c:\Users\bumbl\Downloads\Nueva carpeta\SmartAcademicAdvisor_UI"

# Si aún no has inicializado git
git init
git add .
git commit -m "Initial commit"

# Agregar Azure como remote
git remote add azure <URL-que-te-dio-Azure>

# Configurar credenciales
az webapp deployment user set --user-name <tu-usuario> --password <tu-password>

# Desplegar
git push azure master
```

## Paso 6: Verificar

Tu app estará disponible en:
```
https://smart-academic-advisor-2024.azurewebsites.net
```

## Comandos útiles

```powershell
# Ver logs en tiempo real
az webapp log tail --name smart-academic-advisor-2024 --resource-group SmartAdvisorRG

# Reiniciar la app
az webapp restart --name smart-academic-advisor-2024 --resource-group SmartAdvisorRG

# Ver configuración
az webapp config show --name smart-academic-advisor-2024 --resource-group SmartAdvisorRG
```

## Notas importantes

1. **Archivos JSON**: Los archivos en `/Data` se perderán al redesplegar. Considera:
   - Usar Azure Blob Storage
   - Usar Azure SQL Database
   - Usar Azure Cosmos DB

2. **Variables de entorno**: Configura en Azure Portal → Configuration → Application Settings

3. **Custom domain**: Puedes agregar tu propio dominio en Azure Portal

## Alternativa: Despliegue con Visual Studio

1. Clic derecho en el proyecto → Publish
2. Seleccionar Azure → Azure App Service (Windows)
3. Crear nuevo o seleccionar existente
4. Publish
