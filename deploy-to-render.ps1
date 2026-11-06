# Script para desplegar Smart Academic Advisor en Render.com
# Asegúrate de tener Git instalado y configurado

param(
    [Parameter(Mandatory=$false)]
    [string]$RepoUrl = ""
)

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Smart Academic Advisor - Deploy Tool" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Verificar si Git está instalado
try {
    git --version | Out-Null
} catch {
    Write-Host "❌ Git no está instalado. Por favor instálalo desde: https://git-scm.com/" -ForegroundColor Red
    exit 1
}

# Verificar si ya existe un repositorio Git
if (Test-Path ".git") {
    Write-Host "✅ Repositorio Git ya inicializado" -ForegroundColor Green
} else {
    Write-Host "📦 Inicializando repositorio Git..." -ForegroundColor Yellow
    git init
    Write-Host "✅ Repositorio inicializado" -ForegroundColor Green
}

# Agregar archivos
Write-Host "📝 Agregando archivos al staging..." -ForegroundColor Yellow
git add .

# Verificar si hay cambios
$status = git status --porcelain
if ([string]::IsNullOrEmpty($status)) {
    Write-Host "✅ No hay cambios para commitear" -ForegroundColor Green
} else {
    Write-Host "💾 Creando commit..." -ForegroundColor Yellow
    git commit -m "Deploy: Smart Academic Advisor - $(Get-Date -Format 'yyyy-MM-dd HH:mm')"
    Write-Host "✅ Commit creado" -ForegroundColor Green
}

# Configurar rama principal
$currentBranch = git branch --show-current
if ($currentBranch -ne "main") {
    Write-Host "🔄 Renombrando rama a 'main'..." -ForegroundColor Yellow
    git branch -M main
}

# Configurar remote si se proporcionó URL
if (-not [string]::IsNullOrEmpty($RepoUrl)) {
    Write-Host "🔗 Configurando repositorio remoto..." -ForegroundColor Yellow
    
    # Remover remote existente si hay
    $remotes = git remote
    if ($remotes -contains "origin") {
        git remote remove origin
    }
    
    git remote add origin $RepoUrl
    Write-Host "✅ Remote configurado: $RepoUrl" -ForegroundColor Green
    
    # Push al repositorio
    Write-Host "🚀 Subiendo código a GitHub..." -ForegroundColor Yellow
    git push -u origin main --force
    Write-Host "✅ Código subido exitosamente" -ForegroundColor Green
} else {
    Write-Host ""
    Write-Host "ℹ️  Para subir a GitHub, ejecuta:" -ForegroundColor Cyan
    Write-Host "   .\deploy-to-render.ps1 -RepoUrl 'https://github.com/TU-USUARIO/TU-REPO.git'" -ForegroundColor White
    Write-Host ""
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Siguientes pasos:" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "1. 🌐 Ve a: https://dashboard.render.com" -ForegroundColor White
Write-Host "2. 🔗 Conecta tu cuenta de GitHub" -ForegroundColor White
Write-Host "3. ➕ Click en 'New +' → 'Web Service'" -ForegroundColor White
Write-Host "4. 📦 Selecciona tu repositorio" -ForegroundColor White
Write-Host "5. ⚙️  Render detectará automáticamente el render.yaml" -ForegroundColor White
Write-Host "6. 🚀 Click en 'Create Web Service'" -ForegroundColor White
Write-Host "7. ⏳ Espera 5-10 minutos..." -ForegroundColor White
Write-Host "8. ✅ ¡Tu app estará en línea!" -ForegroundColor White
Write-Host ""
Write-Host "📖 Ver guía completa: DEPLOY-RENDER.md" -ForegroundColor Yellow
Write-Host ""
