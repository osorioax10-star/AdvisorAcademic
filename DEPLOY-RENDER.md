# 🚀 Despliegue en Render.com

Esta guía te ayudará a desplegar **Smart Academic Advisor** en Render.com de forma gratuita.

## 📋 Requisitos Previos

1. Una cuenta en [Render.com](https://render.com) (gratis)
2. El código del proyecto en un repositorio Git (GitHub, GitLab o Bitbucket)

## 🎯 Método 1: Despliegue Automático (Recomendado)

### Paso 1: Subir el código a GitHub

Si aún no tienes el código en GitHub:

```powershell
cd "c:\Users\bumbl\Downloads\Nueva carpeta\SmartAcademicAdvisor_UI"

# Inicializar repositorio Git
git init
git add .
git commit -m "Initial commit - Smart Academic Advisor"

# Crear repositorio en GitHub y conectar
git remote add origin https://github.com/TU-USUARIO/SmartAcademicAdvisor.git
git branch -M main
git push -u origin main
```

### Paso 2: Conectar Render con GitHub

1. Ve a [https://dashboard.render.com](https://dashboard.render.com)
2. Haz clic en **"New +"** → **"Web Service"**
3. Conecta tu cuenta de GitHub
4. Selecciona el repositorio **SmartAcademicAdvisor**

### Paso 3: Configurar el Web Service

Render detectará automáticamente el `render.yaml`, pero verifica estos valores:

**Configuración básica:**
- **Name**: `smartacademicadvisor` (o el nombre que prefieras)
- **Region**: Selecciona la más cercana a ti
- **Branch**: `main`
- **Runtime**: Docker
- **Plan**: Free

**Variables de entorno (opcional):**
```
ASPNETCORE_ENVIRONMENT=Production
```

### Paso 4: Desplegar

1. Haz clic en **"Create Web Service"**
2. Render comenzará a construir la imagen Docker
3. Espera 5-10 minutos (la primera vez tarda más)
4. ¡Listo! Tu app estará en: `https://smartacademicadvisor.onrender.com`

## 🎯 Método 2: Despliegue Manual (Sin Git)

Si no quieres usar Git, puedes usar Render CLI:

### Paso 1: Instalar Render CLI

```powershell
# Con npm
npm install -g render-cli

# O descargar desde: https://render.com/docs/cli
```

### Paso 2: Autenticarse

```powershell
render login
```

### Paso 3: Crear el servicio

```powershell
cd "c:\Users\bumbl\Downloads\Nueva carpeta\SmartAcademicAdvisor_UI"
render create
```

Sigue las instrucciones en pantalla.

## 🔧 Configuración del Proyecto para Render

Ya están incluidos estos archivos necesarios:

### ✅ `render.yaml`
Configuración de infraestructura como código:
```yaml
services:
  - type: web
    name: smartacademicadvisor
    runtime: docker
    plan: free
    dockerfilePath: ./Dockerfile
    envVars:
      - key: ASPNETCORE_ENVIRONMENT
        value: Production
```

### ✅ `Dockerfile`
Configurado para usar el puerto dinámico de Render:
```dockerfile
ENV ASPNETCORE_URLS=http://+:${PORT:-80}
```

## 📊 Monitoreo y Logs

Una vez desplegado, puedes:

1. **Ver logs en tiempo real:**
   - Dashboard de Render → Tu servicio → Pestaña "Logs"

2. **Métricas de uso:**
   - Dashboard → Pestaña "Metrics"

3. **Configurar dominios personalizados:**
   - Dashboard → Pestaña "Settings" → "Custom Domain"

## 🆓 Plan Gratuito de Render

**Incluye:**
- ✅ 750 horas/mes (suficiente para estar siempre activo)
- ✅ SSL/HTTPS automático
- ✅ Builds automáticos desde Git
- ✅ Despliegues ilimitados

**Limitaciones:**
- ⚠️ El servicio se "duerme" después de 15 minutos de inactividad
- ⚠️ Primera petición después de dormir tarda ~30 segundos
- ⚠️ 512 MB RAM
- ⚠️ Almacenamiento efímero (los datos se pierden al reiniciar)

## 💾 Persistencia de Datos

**Problema:** Los archivos JSON en `/app/Data` se pierden cuando el contenedor se reinicia.

**Soluciones:**

### Opción 1: Usar Render Disk (Plan Starter - $7/mes)
```yaml
services:
  - type: web
    name: smartacademicadvisor
    runtime: docker
    plan: starter
    disk:
      name: data
      mountPath: /app/Data
      sizeGB: 1
```

### Opción 2: Usar Base de Datos PostgreSQL (Gratis)

1. Crear un servicio PostgreSQL en Render (Plan Free)
2. Modificar el código para usar Entity Framework Core con PostgreSQL
3. Conectar mediante variable de entorno `DATABASE_URL`

### Opción 3: Usar Storage Externo (S3, Azure Blob)

Para mantener el plan gratuito, puedes:
- Guardar los JSON en AWS S3 (plan gratuito: 5GB)
- Usar Azure Blob Storage
- Usar Cloudinary (imágenes)

## 🔄 Actualizaciones Automáticas

Render detecta cambios en tu repositorio:

```powershell
# Hacer cambios en el código
git add .
git commit -m "Actualización de funcionalidad"
git push origin main

# Render automáticamente:
# 1. Detecta el push
# 2. Construye la nueva imagen
# 3. Despliega la nueva versión
# 4. Zero-downtime deployment
```

## 🐛 Solución de Problemas

### El build falla
```powershell
# Ver logs detallados en Render Dashboard
# Revisar que el Dockerfile esté en la raíz del repo
# Verificar que todos los archivos estén commiteados
```

### La app no responde
```
# Verificar logs en Render
# Asegurarse que el puerto sea dinámico (variable PORT)
# Confirmar que ASPNETCORE_URLS use ${PORT}
```

### Datos se pierden al reiniciar
```
# Esto es normal en el plan Free
# Considera las opciones de persistencia mencionadas arriba
# O usa el plan Starter ($7/mes) con Render Disk
```

## 🌐 URLs de Ejemplo

- **Tu app**: `https://smartacademicadvisor.onrender.com`
- **Dashboard**: `https://smartacademicadvisor.onrender.com/Home/Index`
- **Portal alumno**: `https://smartacademicadvisor.onrender.com/Portal`

## 📞 Soporte

- **Documentación oficial**: https://render.com/docs
- **Community Forum**: https://community.render.com
- **Status**: https://status.render.com

## 🎉 ¡Listo!

Tu aplicación estará disponible 24/7 en internet con:
- ✅ HTTPS automático
- ✅ Actualizaciones automáticas desde Git
- ✅ Logs y métricas en tiempo real
- ✅ Sin costo inicial

---

**Nota:** Si necesitas persistencia de datos real y mejor rendimiento, considera el plan Starter de Render ($7/mes) o migrar a Azure/AWS para producción.
