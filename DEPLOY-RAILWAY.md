# Despliegue en Railway (Alternativa Fácil)

Railway es una plataforma moderna que soporta .NET y es más simple que Azure.

## Por qué Railway?
- ✅ Despliegue automático desde GitHub
- ✅ Soporta .NET nativamente
- ✅ $5 gratis al mes
- ✅ SSL automático
- ✅ Fácil configuración

## Paso 1: Preparar el proyecto

1. Crea un `railway.toml` en la raíz:

```toml
[build]
builder = "nixpacks"

[deploy]
startCommand = "cd SmartAcademicAdvisor.Web && dotnet run --urls http://0.0.0.0:$PORT"
```

2. Asegúrate de que tu proyecto compile:
```powershell
cd SmartAcademicAdvisor.Web
dotnet build
```

## Paso 2: Subir a GitHub

```powershell
cd "c:\Users\bumbl\Downloads\Nueva carpeta\SmartAcademicAdvisor_UI"

# Si no has inicializado git
git init

# Crear .gitignore
echo "bin/" > .gitignore
echo "obj/" >> .gitignore
echo "*.user" >> .gitignore
echo ".vs/" >> .gitignore

# Commit
git add .
git commit -m "Ready for Railway deployment"

# Crear repositorio en GitHub y subirlo
git remote add origin https://github.com/TU-USUARIO/SmartAcademicAdvisor.git
git branch -M main
git push -u origin main
```

## Paso 3: Desplegar en Railway

1. Ve a https://railway.app
2. Regístrate/Login con GitHub
3. Click "New Project"
4. Selecciona "Deploy from GitHub repo"
5. Autoriza Railway y selecciona tu repositorio
6. Railway detectará automáticamente que es .NET
7. Click "Deploy"

## Paso 4: Configurar variables (opcional)

En Railway dashboard:
- Variables → Add Variable
- Ejemplo: `ASPNETCORE_ENVIRONMENT=Production`

## Paso 5: Obtener URL

Railway te asignará automáticamente una URL como:
```
https://smartacademicadvisor-production.up.railway.app
```

## Comandos útiles

```powershell
# Instalar Railway CLI (opcional)
npm i -g @railway/cli

# Login
railway login

# Ver logs
railway logs

# Vincular proyecto local
railway link
```

## Ventajas de Railway sobre otras opciones

| Característica | Railway | Azure | Netlify |
|---------------|---------|-------|---------|
| Soporta .NET | ✅ | ✅ | ❌ |
| Gratis | $5/mes | Limitado | N/A |
| SSL automático | ✅ | ✅ | ✅ |
| GitHub integration | ✅ | ✅ | ✅ |
| Fácil setup | ⭐⭐⭐⭐⭐ | ⭐⭐⭐ | ⭐⭐⭐⭐⭐ |
| Base de datos | ✅ Incluida | Extra | ❌ |
