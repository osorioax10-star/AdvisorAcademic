# Smart Academic Advisor - Sistema de Recomendación Académica

Sistema web ASP.NET Core para la gestión de alumnos, materias, especialidades y generación de recomendaciones académicas personalizadas.

## 🚀 Características

- **Gestión de Alumnos**: Registro y seguimiento de estudiantes
- **Catálogo de Materias**: Administración de materias y prerrequisitos
- **Especialidades**: Configuración de especialidades con criterios de afinidad
- **Motor de Recomendaciones**: Sistema inteligente de recomendaciones basado en:
  - Promedio académico
  - Intereses del alumno
  - Materias cursadas
  - Reglas de inferencia personalizables
- **Dashboard Analítico**: Visualización de estadísticas y métricas
- **CRUD Dinámico**: Gestión de cursos, certificaciones y reglas

## 📋 Requisitos

### Para ejecutar localmente:
- .NET 8.0 SDK
- Windows, Linux o macOS

### Para Docker:
- Docker Desktop o Docker Engine
- Docker Compose (opcional)

## 🏃 Ejecución Local

### Opción 1: Con .NET CLI

```powershell
cd SmartAcademicAdvisor.Web
dotnet restore
dotnet build
dotnet run
```

La aplicación estará disponible en:
- HTTP: `http://localhost:5000`
- HTTPS: `https://localhost:5001`

### Opción 2: Con Visual Studio

1. Abrir `SmartAcademicAdvisor.Web.sln`
2. Presionar F5 o hacer clic en "Run"

## 🐳 Ejecución con Docker

### Construcción de la imagen:

```powershell
docker build -t smartacademicadvisor .
```

### Ejecución del contenedor:

```powershell
docker run -d -p 8080:80 --name smart-advisor smartacademicadvisor
```

La aplicación estará disponible en: `http://localhost:8080`

### Con Docker Compose:

```powershell
docker-compose up -d
```

Para detener:

```powershell
docker-compose down
```

## ☁️ Despliegue en la Nube

### Render.com (Recomendado - GRATIS)

**Ver guía completa**: [DEPLOY-RENDER.md](DEPLOY-RENDER.md)

**Pasos rápidos:**

1. Sube el código a GitHub
2. Conecta Render.com con tu repositorio
3. ¡Listo! Render detecta el `render.yaml` y despliega automáticamente

Tu app estará en: `https://tu-app.onrender.com`

**Ventajas:**
- ✅ 100% Gratis
- ✅ HTTPS automático
- ✅ Despliegues automáticos desde Git
- ✅ Sin tarjeta de crédito requerida

### Azure App Service

```powershell
# Compilar y publicar
dotnet publish -c Release -o ./publish

# Crear un Web App en Azure
az webapp up --name smart-academic-advisor --resource-group MyResourceGroup --runtime "DOTNET|8.0"
```

## 📁 Estructura del Proyecto

```
SmartAcademicAdvisor.Web/
├── Controllers/          # Controladores MVC
├── Core/
│   ├── Interfaces.cs    # Interfaces del repositorio
│   └── Models.cs        # Modelos de dominio
├── Crud/                # Sistema CRUD dinámico
├── Data/                # Archivos JSON de datos
├── Models/              # ViewModels y FormModels
├── Perfiles/            # Sistema de autenticación/autorización
├── Services/            # Servicios de negocio
├── Views/               # Vistas Razor
└── wwwroot/             # Archivos estáticos (CSS, JS, imágenes)
```

## 🗄️ Datos

Los datos se almacenan en archivos JSON en la carpeta `Data/`:

- `alumnos.json` - Información de estudiantes
- `materias.json` - Catálogo de materias
- `especialidades.json` - Especialidades disponibles
- `historial.json` - Historial académico
- `recomendaciones.json` - Recomendaciones generadas
- `cursos.json` - Cursos complementarios
- `certificaciones.json` - Certificaciones profesionales
- `reglas.json` - Reglas del motor de inferencia

## 🔑 Credenciales de Acceso

Por defecto, el sistema tiene dos perfiles:

**Administrador:**
- Usuario: `admin`
- Contraseña: `admin123`
- Acceso completo al sistema

**Alumno:**
- Usuario: `alumno`
- Contraseña: `alumno123`
- Acceso limitado al portal estudiantil

## 🛠️ Tecnologías Utilizadas

- **Backend**: ASP.NET Core 8.0 MVC
- **Frontend**: HTML5, CSS3, JavaScript
- **Iconos**: Font Awesome
- **Almacenamiento**: JSON (System.Text.Json)
- **Patrones**: Repository Pattern, Dependency Injection

## 📝 Funcionalidades Principales

### Dashboard
- Estadísticas generales de alumnos y especialidades
- Gráficas de distribución y promedios
- Top de intereses y materias

### Gestión de Alumnos
- CRUD completo con validaciones
- Asignación de intereses
- Tracking de estatus académico

### Motor de Recomendaciones
- Cálculo de afinidad por especialidad
- Ponderación configurable (promedio, intereses, materias, reglas)
- Justificación detallada de recomendaciones
- Árbol de inferencia visual

### Sistema de Reglas
- Editor de reglas de programación lógica
- Encadenamiento hacia adelante
- Predicados personalizables

## 🤝 Contribuciones

Las contribuciones son bienvenidas. Por favor:

1. Haz fork del proyecto
2. Crea una rama para tu feature (`git checkout -b feature/AmazingFeature`)
3. Commit tus cambios (`git commit -m 'Add some AmazingFeature'`)
4. Push a la rama (`git push origin feature/AmazingFeature`)
5. Abre un Pull Request

## 📄 Licencia

Este proyecto está bajo la Licencia MIT.

## 📞 Soporte

Para preguntas o problemas, por favor abre un issue en el repositorio.

## 🎓 Acerca del Proyecto

Smart Academic Advisor es un sistema desarrollado para facilitar la orientación vocacional y académica en instituciones educativas, utilizando técnicas de inteligencia artificial y sistemas expertos para proporcionar recomendaciones personalizadas a cada estudiante.
