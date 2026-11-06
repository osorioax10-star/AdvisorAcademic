<#
  Instala la interfaz, el CRUD completo, los perfiles (administrador y alumno) y los datos de Smart Academic Advisor.

  Uso (PowerShell, desde la carpeta donde descomprimiste el zip):
    powershell -ExecutionPolicy Bypass -File .\Instalar-UI.ps1 -Proyecto "C:\Users\bumbl\SmartAcademicAdvisor" -Compilar

  -Proyecto  Carpeta de la solucion (la que contiene SmartAcademicAdvisor.Web).
  -SinDatos  No reemplaza alumnos.json, historial.json ni recomendaciones.json.
  -Compilar  Ejecuta "dotnet build" al terminar y muestra los errores, si los hay.

  Antes de tocar nada se hace un respaldo en _respaldo_ui_<fecha> dentro de la carpeta de la solucion.
#>
param(
    [Parameter(Mandatory = $true)][string]$Proyecto,
    [switch]$SinDatos,
    [switch]$Compilar
)
$ErrorActionPreference = 'Stop'

$web = Join-Path $Proyecto 'SmartAcademicAdvisor.Web'
if (-not (Test-Path (Join-Path $web 'Views'))) { throw "No encuentro $web\Views. Revisa la ruta de -Proyecto." }
$origen = Join-Path $PSScriptRoot 'SmartAcademicAdvisor.Web'
if (-not (Test-Path $origen)) { throw "No encuentro la carpeta SmartAcademicAdvisor.Web junto a este script." }

$respaldo = Join-Path $Proyecto ('_respaldo_ui_' + (Get-Date -Format 'yyyyMMdd_HHmmss'))
New-Item -ItemType Directory -Force $respaldo | Out-Null

Write-Host "1/4 Respaldando Views, Controllers, CSS y Data en $respaldo"
foreach ($carpeta in @('Views', 'Controllers', 'Data', 'wwwroot\css')) {
    $desde = Join-Path $web $carpeta
    if (Test-Path $desde) { Copy-Item $desde (Join-Path $respaldo $carpeta) -Recurse }
}

Write-Host "2/4 Copiando los archivos nuevos"
Get-ChildItem $origen -Recurse -File | ForEach-Object {
    $rel = $_.FullName.Substring($origen.Length).TrimStart('\', '/')
    if ($SinDatos -and $rel -like 'Data\*') { Write-Host "    (omitido) $rel"; return }
    $dest = Join-Path $web $rel
    New-Item -ItemType Directory -Force (Split-Path $dest) | Out-Null
    Copy-Item $_.FullName $dest -Force
    Write-Host "    $rel"
}

Write-Host "3/4 IDs unicos, proteccion por perfil y unificacion de colores"
$utf8 = New-Object System.Text.UTF8Encoding($true)
$ajustes = @(
    @{ Archivo = 'Controllers\AlumnosController.cs';  Viejo = 'ALU-{DateTime.Now.Ticks % 10000:D3}'; Nuevo = 'ALU-{DateTime.Now.Ticks % 100000:D5}' },
    @{ Archivo = 'Controllers\MateriasController.cs'; Viejo = 'MAT-{DateTime.Now.Ticks % 10000:D3}'; Nuevo = 'MAT-{DateTime.Now.Ticks % 100000:D5}' }
)
foreach ($a in $ajustes) {
    $p = Join-Path $web $a.Archivo
    if (Test-Path $p) {
        $t = [System.IO.File]::ReadAllText($p, [System.Text.Encoding]::UTF8)
        if ($t.Contains($a.Viejo)) { [System.IO.File]::WriteAllText($p, $t.Replace($a.Viejo, $a.Nuevo), $utf8); Write-Host "    ID corregido en $($a.Archivo)" }
    }
}
foreach ($c in @('AlumnosController', 'MateriasController')) {
    $p = Join-Path $web "Controllers\$c.cs"
    if (Test-Path $p) {
        $t = [System.IO.File]::ReadAllText($p, [System.Text.Encoding]::UTF8)
        if (-not $t.Contains('SoloAdmin')) {
            $t = $t.Replace("public class $c", "[SmartAcademicAdvisor.Web.Perfiles.SoloAdmin]`r`npublic class $c")
            [System.IO.File]::WriteAllText($p, $t, $utf8)
            Write-Host "    Perfil de administrador exigido en $c"
        }
    }
}
Get-ChildItem (Join-Path $web 'Views') -Recurse -Filter *.cshtml | ForEach-Object {
    $t = [System.IO.File]::ReadAllText($_.FullName, [System.Text.Encoding]::UTF8)
    $n = $t.Replace('#14B8A6', '#059669').Replace('#0D9488', '#047857').Replace('rgba(20, 184, 166', 'rgba(5, 150, 105').Replace('#8B5CF6', '#7C3AED').Replace('#F59E0B', '#EA580C')
    if ($n -ne $t) { [System.IO.File]::WriteAllText($_.FullName, $n, $utf8) }
}

if ($Compilar) {
    Write-Host "4/4 Compilando (dotnet build)"
    Push-Location $Proyecto
    dotnet build
    Pop-Location
} else {
    Write-Host "4/4 Omitido. Compila tu con:  cd `"$Proyecto`" ; dotnet build"
}

Write-Host ""
Write-Host "Listo. Para probar:  cd `"$web`" ; dotnet run"
Write-Host "Si algo falla, tu version anterior esta en: $respaldo"
