# Guía para Crear Releases Automáticas

## ¿Cómo funciona?

1. **Localmente**: Editas el código y actualizas la versión en:
   - `HelloApp.csproj` (Version, AssemblyVersion, FileVersion)
   - `Program.cs` (en el mensaje de bienvenida)

2. **Git**: Haces commit y luego creas un tag:
   ```bash
   git tag -a v0.0.2 -m "Release version 0.0.2"
   git push origin v0.0.2
   ```

3. **GitHub**: Automáticamente:
   - Dispara GitHub Actions (workflow en `.github/workflows/release.yml`)
   - Compila el proyecto en modo Release
   - Genera `HelloAppInstaller.exe`
   - Lo sube como asset a la release

4. **Descarga**: Tu proyecto de actualizaciones puede descargar desde:
   ```
   https://api.github.com/repos/Lucas-Carbo/TestingRelease-Updater/releases/latest
   ```

## Pasos Rápidos

### 1. Actualizar versión en csproj
```xml
<Version>0.0.2</Version>
<AssemblyVersion>0.0.2</AssemblyVersion>
<FileVersion>0.0.2</FileVersion>
```

### 2. Actualizar versión en Program.cs
```csharp
Console.WriteLine("         HelloApp v0.0.2");
```

### 3. Commitear cambios
```bash
git add .
git commit -m "v0.0.2: [descripción de cambios]"
git push
```

### 4. Crear tag y release
```bash
git tag -a v0.0.2 -m "Release version 0.0.2"
git push origin v0.0.2
```

### 5. Ir a GitHub y publicar la release
Opcionalmente puedes crear una release en GitHub con notas de cambios (el .exe se carga automáticamente).

## API para tu Proyecto de Actualizaciones

```csharp
// Obtener información de la última release
GET https://api.github.com/repos/Lucas-Carbo/TestingRelease-Updater/releases/latest

// Respuesta JSON:
{
  "tag_name": "v0.0.1",
  "assets": [
    {
      "name": "HelloAppInstaller.exe",
      "browser_download_url": "https://github.com/.../releases/download/v0.0.1/HelloAppInstaller.exe"
    }
  ]
}

// Descargar el .exe
wget https://github.com/.../releases/download/v0.0.1/HelloAppInstaller.exe
```

## Estado de Builds

Puedes ver el estado de los builds en: https://github.com/Lucas-Carbo/TestingRelease-Updater/actions
