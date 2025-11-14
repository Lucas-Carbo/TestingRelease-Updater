# Guía: Automatizar Releases en GitHub con GitHub Actions

Este documento explica cómo configurar GitHub Actions para compilar automáticamente tu aplicación C# y crear releases en GitHub cuando haces push de un tag de versión.

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
      - name: Checkout code
        uses: actions/checkout@v4

      - name: Setup .NET
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '8.0.x'

      - name: Restore dependencies
        run: dotnet restore

      - name: Build Release
        run: dotnet build --configuration Release --no-restore

      - name: Locate executable
        id: find_exe
        shell: powershell
        run: |
          $exePath = Get-ChildItem -Path "bin/Release" -Recurse -Filter "*.exe" | Select-Object -First 1
          if ($exePath) {
            echo "exe_path=$($exePath.FullName)" >> $env:GITHUB_OUTPUT
            echo "Found executable: $($exePath.FullName)"
          } else {
            echo "ERROR: No executable found!"
            exit 1
          }

      - name: Create Release
        uses: softprops/action-gh-release@v2
        with:
          files: ${{ steps.find_exe.outputs.exe_path }}
          generate_release_notes: true
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
| `runs-on: windows-latest` | Ejecuta en Windows (necesario para compilar .exe) |
| `actions/checkout@v4` | Descarga tu código |
| `actions/setup-dotnet@v4` | Instala .NET 8.0 |
| `dotnet restore` | Restaura dependencias NuGet |
| `dotnet build` | Compila en modo Release |
| `softprops/action-gh-release@v2` | Crea la release y sube archivos |

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
5. Cuando termine, ve a **"Releases"** y verás tu release con el `.exe` descargable

## 📦 Paso 5: Descargar el ejecutable

Una vez creada la release:

1. Ve a la sección **"Releases"** de tu repositorio
2. Haz clic en la versión deseada
3. Descarga el `.exe` en la sección de assets

**O usando curl:**
```powershell
curl -L -o HelloAppInstaller.exe `
  "https://github.com/TU_USUARIO/TU_REPO/releases/download/v0.0.1/HelloAppInstaller.exe"
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
   - Compila tu código
   - Crea la release
   - Sube el `.exe` como asset

6. **Descarga disponible en Releases**

## 🛠️ Solución de problemas

### Error: "Resource not accessible by integration"

**Causa:** El token de GitHub Actions no tiene permisos suficientes

**Solución:** Asegúrate de que el archivo `release.yml` tiene:
```yaml
permissions:
  contents: write
  packages: write
```

### Error: "No executable found!"

**Causa:** El `.exe` no se compiló correctamente

**Solución:** 
1. Verifica que tu `.csproj` tiene `<OutputType>Exe</OutputType>`
2. Compila localmente para probar: `dotnet build --configuration Release`
3. Revisa los logs del workflow en GitHub Actions

### El workflow no se ejecuta al hacer push del tag

**Verificar:**
1. ¿El tag comienza con `v`? (ej: v1.0.0)
2. ¿Hiciste `git push origin v1.0.0`?
3. En GitHub → Settings → Actions, verifica que los workflows están habilitados

## 📋 Checklist de configuración

- [ ] Carpeta `.github/workflows/` creada
- [ ] Archivo `release.yml` creado con el contenido correcto
- [ ] `.csproj` tiene `<OutputType>Exe</OutputType>`
- [ ] `.gitignore` excluye `/bin` y `/obj` (para no subir binarios)
- [ ] Archivo `release.yml` pusheado a main
- [ ] Primer tag creado localmente (git tag -a v0.0.1)
- [ ] Tag pusheado a GitHub (git push origin v0.0.1)
- [ ] Release visible en GitHub → Releases

## 📚 Ejemplos de tags versión

```powershell
# Versiones semánticas recomendadas
git tag -a v0.0.1 -m "Initial release"
git tag -a v0.1.0 -m "Added feature X"
git tag -a v1.0.0 -m "First major release"
git tag -a v1.1.2 -m "Bug fixes and improvements"
```

## 🔗 Recursos

- [Documentación GitHub Actions](https://docs.github.com/en/actions)
- [Versionamiento Semántico](https://semver.org/lang/es/)
- [action-gh-release](https://github.com/softprops/action-gh-release)

---

**Nota:** Una vez configurado, este proceso es completamente automático. Solo necesitas hacer commits, crear tags y GitHub se encarga del resto.
