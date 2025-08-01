# Ruta al compilador FXC
$fxcPath = "C:\Program Files (x86)\Windows Kits\10\bin\10.0.26100.0\x64\fxc.exe"

# Verificar si existe FXC
if (-Not (Test-Path $fxcPath)) {
    Write-Host "ERROR: No se encontró fxc.exe en $fxcPath"
    Write-Host "Por favor, ajusta la ruta en el script."
    exit 1
}

# Crear carpeta Shaders si no existe
if (-Not (Test-Path "Shaders")) {
    New-Item -ItemType Directory -Path "Shaders" | Out-Null
}

Write-Host "Compilando shaders..."

# Función para compilar shader
function Compile-Shader {
    param (
        [string]$inputFile,
        [string]$outputFile
    )

    Write-Host "Compilando $inputFile ..."
    & "$fxcPath" "/T" "ps_3_0" "/E" "main" "/Fo" $outputFile $inputFile
    if ($LASTEXITCODE -eq 0) {
        Write-Host "[OK] $outputFile compilado correctamente."
    } else {
        Write-Host "[ERROR] Error compilando $inputFile"
    }
}

# Compilar ambos shaders
Compile-Shader "Shaders\GlowEffect.ps" "Shaders\GlowEffect.ps"
Compile-Shader "Shaders\DistortionEffect.ps" "Shaders\DistortionEffect.ps"

Write-Host "Proceso completado."
