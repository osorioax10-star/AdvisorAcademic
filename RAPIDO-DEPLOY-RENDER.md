# ⚡ DESPLIEGUE RÁPIDO EN RENDER (5 MINUTOS)

## ❌ POR QUÉ NETLIFY NO FUNCIONA

Tu proyecto **NO tiene archivos HTML**. Es una aplicación **ASP.NET Core** que:
- Usa archivos `.cshtml` (Razor) que se procesan en el servidor
- Ejecuta código C# en cada petición
- Lee/escribe archivos JSON con C#

**Netlify solo sirve HTML estático** → No puede ejecutar C#

---

## ✅ PASOS PARA RENDER

### 1. Ya tienes el Dockerfile creado ✓

Verifica que existe: `Dockerfile` en la raíz

### 2. Sube a GitHub si no lo has hecho

```powershell
cd "c:\Users\bumbl\Downloads\Nueva carpeta\SmartAcademicAdvisor_UI"

# Ver estado
git status

# Si hay cambios, súbelos
git add .
git commit -m "Ready for deployment"
git push
```

### 3. Desplegar en Render

**PASO A PASO:**

1. Ve a: https://render.com
2. Click **"Sign Up"** → **"Sign in with GitHub"**
3. Autoriza Render a acceder a tus repos
4. Click **"New +"** (arriba derecha)
5. Selecciona **"Web Service"**
6. En "Connect a repository":
   - Si no ves tu repo, click **"Configure account"**
   - Autoriza el acceso al repositorio específico
   - Regresa y refresca
7. Click **"Connect"** en tu repositorio
8. Configuración automática:
   - **Name**: `smart-academic-advisor` (o el que prefieras)
   - **Environment**: Docker (auto-detectado)
   - **Branch**: main
   - **Plan**: Free
9. Click **"Create Web Service"**

### 4. ¡Espera 2-3 minutos!

Render:
- ✓ Clonará tu repositorio
- ✓ Construirá la imagen Docker
- ✓ Iniciará tu aplicación
- ✓ Te dará una URL pública

### 5. Tu app estará en:

```
https://smart-academic-advisor.onrender.com
```

(o el nombre que elegiste)

---

## 🔧 SI ALGO FALLA

### Error: "No csproj found"

Verifica que tu Dockerfile tenga la ruta correcta:
```dockerfile
COPY SmartAcademicAdvisor.Web/SmartAcademicAdvisor.Web.csproj SmartAcademicAdvisor.Web/
```

### Error: "Port binding failed"

Asegúrate de que el Dockerfile tenga:
```dockerfile
ENV ASPNETCORE_URLS=http://+:8080
```

### La app se queda "Building..."

Es normal, la primera vez tarda 2-4 minutos.

---

## 📱 DESPUÉS DEL DESPLIEGUE

### Acceder a tu app:
- **Dashboard Admin**: `https://tu-app.onrender.com/`
- **Portal Alumno**: Primero debes elegir perfil en `/Acceso`

### Ver logs:
En Render dashboard → Logs → View Logs

### Redesplegar:
Simplemente haz `git push` → Render redespliega automáticamente

---

## ⚠️ IMPORTANTE: LIMITACIÓN DEL PLAN FREE

El tier free de Render **"duerme" después de 15 minutos sin uso**.

**¿Qué significa?**
- Primera visita después de inactividad: tardará ~30 segundos en cargar
- Visitas subsecuentes: normal (instantáneo)

**¿Cómo evitarlo?**
- Plan pagado: $7/mes (no se duerme)
- O usa Railway: $5/mes de crédito gratis

---

## 🆘 ¿SIGUE SIN FUNCIONAR?

Házmelo saber y te ayudo a:
1. Revisar los logs de Render
2. Ajustar el Dockerfile
3. Probar con Railway como alternativa
