# Guía: Automatizar Releases en GitHub con GitHub Actions

Este documento explica cómo configurar GitHub Actions para compilar automáticamente tu aplicación C# y crear releases en GitHub cuando haces push de un tag de versión, **empaquetando todos los binarios en un ZIP**.

## 📋 Requisitos

- Proyecto C# compilable (.NET 8.0 o superior)
- Repositorio en GitHub
- Acceso a los Settings del repositorio

## 🔧 Paso 1: Crear la carpeta de workflows

En la raíz de tu proyecto, crea la siguiente estructura de carpetas:

```
.github/
└── workflows/
```

**Comando:**
```powershell
mkdir -p .github/workflows
```

## 📝 Paso 2: Crear el archivo de workflow

Crea un archivo llamado `.github/workflows/release.yml` en la carpeta que acabas de crear:

```yaml
name: Build and Release

on:
  push:
    tags:
      - 'v*'
  workflow_dispatch:

permissions:
  contents: write
  packages: write

jobs:
  build:
    runs-on: windows-latest
    
    steps:
    - uses: actions/checkout@v3
    
    - name: Setup .NET
      uses: actions/setup-dotnet@v3
      with:
        dotnet-version: '8.0.x'
    
    - name: Restore dependencies
      run: dotnet restore
    
    - name: Build Release
      run: dotnet build -c Release --no-restore
    
    - name: Create ZIP with all binaries
      run: |
        # Crear carpeta temporal con el nombre de la app
        $zipName = "HelloApp-Release.zip"
        $sourceDir = "bin\Release\net8.0"
        
        # Comprimir todos los archivos compilados
        Compress-Archive -Path "$sourceDir\*" -DestinationPath $zipName -Force
        
        # Verificar que se creó
        if (Test-Path $zipName) {
          Write-Host "ZIP creado exitosamente: $zipName"
          Get-Item $zipName | Select-Object FullName, Length
        } else {
          Write-Host "ERROR: No se pudo crear el ZIP"
          exit 1
        }
      shell: powershell
    
    - name: Create Release with ZIP Asset
      uses: softprops/action-gh-release@v2
      with:
        files: HelloApp-Release.zip
      env:
        GITHUB_TOKEN: ${{ secrets.GITHUB_TOKEN }}
```

### 📌 Explicación del archivo:

| Sección | Descripción |
|---------|------------|
| `name` | Nombre del workflow en GitHub Actions |
| `on: push: tags: 'v*'` | Se ejecuta cuando haces push de tags que comienzan con `v` (ej: v0.0.1, v1.0.0) |
| `workflow_dispatch` | Permite ejecutar manualmente desde GitHub |
| `permissions` | Permite que GitHub Actions escriba releases y paquetes |
| `runs-on: windows-latest` | Ejecuta en Windows (necesario para compilar .exe con todas sus dependencias) |
| `actions/checkout@v3` | Descarga tu código |
| `actions/setup-dotnet@v3` | Instala .NET 8.0 |
| `dotnet restore` | Restaura dependencias NuGet |
| `dotnet build` | Compila en modo Release |
| **`Compress-Archive`** | **Comprime TODOS los archivos compilados en un ZIP** |
| `softprops/action-gh-release@v2` | Crea la release y sube archivos |

### 🎯 ¿Por qué comprimir en ZIP?

**Importante:** Una aplicación C# compilada necesita:
- `.exe` (ejecutable)
- `.dll` (librerías necesarias)
- `.json` (configuración de runtime)
- Otros archivos de soporte

Si solo subas el `.exe` sin las DLL, **no funcionará**. Por eso comprimimos todo en un ZIP.

**El ZIP contiene:**
```
HelloApp-Release.zip
├── HelloApp.exe
├── HelloApp.dll
├── HelloApp.deps.json
├── HelloApp.runtimeconfig.json
└── (otros archivos necesarios)
```

## 🚀 Paso 3: Configurar Git Tags localmente

### Crear un tag de versión:

```powershell
git tag -a v0.0.1 -m "Release version 0.0.1"
```

**Parámetros:**
- `-a`: Crea un tag anotado (recomendado)
- `-m`: Mensaje del tag

### Push del tag a GitHub:

```powershell
git push origin v0.0.1
```

O para subir todos los tags:

```powershell
git push origin --tags
```

## ✅ Paso 4: Verificar el workflow

1. Después de hacer push del tag, ve a tu repositorio en GitHub
2. Haz clic en la pestaña **"Actions"**
3. Verás el workflow ejecutándose con el nombre "Build and Release"
4. Espera a que se complete (normalmente 2-3 minutos)
5. Cuando termine, ve a **"Releases"** y verás tu release con el ZIP descargable

## 📦 Paso 5: Descargar el ZIP

Una vez creada la release:

1. Ve a la sección **"Releases"** de tu repositorio
2. Haz clic en la versión deseada
3. Descarga el **`HelloApp-Release.zip`** en la sección de assets

**O usando PowerShell:**
```powershell
$url = "https://github.com/TU_USUARIO/TU_REPO/releases/download/v0.0.1/HelloApp-Release.zip"
$output = "C:\Apps\HelloApp-Release.zip"

Invoke-WebRequest -Uri $url -OutFile $output

# Descomprimir
Expand-Archive -Path $output -DestinationPath "C:\Apps\HelloApp\" -Force
```

## 🔄 Flujo de trabajo típico

### Para crear una nueva versión:

1. **Haz cambios en tu código**
   ```powershell
   # Edita tu código...
   ```

2. **Commit los cambios**
   ```powershell
   git add .
   git commit -m "Nueva funcionalidad en v0.1.0"
   ```

3. **Crea el tag de versión**
   ```powershell
   git tag -a v0.1.0 -m "Release version 0.1.0"
   ```

4. **Push a GitHub**
   ```powershell
   git push origin main
   git push origin v0.1.0
   ```

5. **GitHub Actions se ejecuta automáticamente**
   - ✅ Compila tu código
   - ✅ Comprime todos los binarios en ZIP
   - ✅ Crea la release
   - ✅ Sube el ZIP como asset

6. **ZIP disponible en Releases**
   - Descargar y descomprimir en carpeta de destino

## 🛠️ Solución de problemas

### Error: "Resource not accessible by integration"

**Causa:** El token de GitHub Actions no tiene permisos suficientes

**Solución:** Asegúrate de que el archivo `release.yml` tiene:
```yaml
permissions:
  contents: write
  packages: write
```

### Error: "No se pudo crear el ZIP"

**Causa:** Los archivos compilados no se encontraron en `bin\Release\net8.0`

**Solución:** 
1. Verifica que tu `.csproj` tiene `<TargetFramework>net8.0</TargetFramework>`
2. Compila localmente para probar: `dotnet build --configuration Release`
3. Revisa los logs del workflow en GitHub Actions

### El workflow no se ejecuta al hacer push del tag

**Verificar:**
1. ¿El tag comienza con `v`? (ej: v1.0.0)
2. ¿Hiciste `git push origin v1.0.0`?
3. En GitHub → Settings → Actions, verifica que los workflows están habilitados

### El ZIP está vacío o incompleto

**Solución:**
Asegúrate de que en el workflow, la variable `$sourceDir` apunta al directorio correcto:
```powershell
$sourceDir = "bin\Release\net8.0"  # Cambiar según tu versión de .NET
```

## 📋 Checklist de configuración

- [ ] Carpeta `.github/workflows/` creada
- [ ] Archivo `release.yml` creado con el contenido correcto
- [ ] `.csproj` tiene `<TargetFramework>net8.0</TargetFramework>` (o la versión que uses)
- [ ] `.gitignore` excluye `/bin` y `/obj` (para no subir binarios)
- [ ] Archivo `release.yml` pusheado a main
- [ ] Primer tag creado localmente (git tag -a v0.0.1)
- [ ] Tag pusheado a GitHub (git push origin v0.0.1)
- [ ] Release visible en GitHub → Releases
- [ ] ZIP disponible en assets

## 📚 Ejemplos de tags versión

```powershell
# Versiones semánticas recomendadas
git tag -a v0.0.1 -m "Initial release"
git tag -a v0.1.0 -m "Added feature X"
git tag -a v1.0.0 -m "First major release"
git tag -a v1.1.2 -m "Bug fixes and improvements"
```

## 🔗 Integración con UpdaterService

El ZIP generado es **perfecto para usar con UpdaterService**:

1. **Descargar el ZIP** desde la release
2. **Descomprimir** en la carpeta de instalación
3. **Configurar en `appsettings.json` del UpdaterService:**

```json
{
  "Name": "HelloApp",
  "Type": "Executable",
  "ExecutableName": "HelloApp.exe",
  "RepositoryUrl": "https://github.com/TU_USUARIO/TU_REPO",
  "Branch": "main",
  "Provider": "GitHub",
  "PossiblePaths": [
    "C:\\Apps\\HelloApp",
    "C:\\Program Files\\HelloApp"
  ],
  "Enabled": true
}
```

4. **El UpdaterService:**
   - Detecta la aplicación instalada
   - Lee su versión desde el tag
   - Descarga el ZIP automáticamente
   - Extrae en carpeta temporal
   - Reemplaza los archivos
   - Reinicia la aplicación

## 🔍 Verificación manual

Para verificar que el ZIP contiene todo lo necesario:

```powershell
# Descargar el ZIP
$url = "https://github.com/.../releases/download/v0.4.0/HelloApp-Release.zip"
Invoke-WebRequest -Uri $url -OutFile "HelloApp-Release.zip"

# Ver contenido
Expand-Archive -Path "HelloApp-Release.zip" -DestinationPath ".\test" -Force
Get-ChildItem ".\test" | Select-Object Name, Length

# Ejecutar directamente
& ".\test\HelloApp.exe"
```

---

**Nota:** Una vez configurado, este proceso es completamente automático. Solo necesitas hacer commits, crear tags y GitHub se encarga del resto.
