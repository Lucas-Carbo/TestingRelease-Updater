# HelloApp v0.0.1

Aplicación simple que imprime "Hola como estas" cada 1 minuto y permite seleccionar la ruta de instalación.

## Compilar Localmente

```bash
dotnet build -c Release
```

## Ejecutar

```bash
dotnet run
```

O ejecutar el .exe directamente:
```bash
bin/Release/net8.0/HelloApp.exe
```

## Crear una Release en GitHub

1. Asegúrate de que todos los cambios estén committeados
2. Crea un tag con la versión:
   ```bash
   git tag -a v0.0.2 -m "Release version 0.0.2"
   git push origin v0.0.2
   ```
3. Ve a https://github.com/Lucas-Carbo/TestingRelease-Updater/releases
4. Haz clic en "Draft a new release"
5. Selecciona el tag que acabas de crear
6. Completa el título y descripción
7. Haz clic en "Publish release"

**GitHub Actions compilará automáticamente el proyecto y subirá `HelloAppInstaller.exe` como asset a la release.**

## Características

- ✅ Selecciona dónde instalar la aplicación
- ✅ Imprime "Hola como estas" cada 1 minuto
- ✅ Compilación automática en releases
- ✅ Descargable directamente desde GitHub

## Uso para Actualización Automática

Tu otro proyecto puede:
1. Verificar nuevas releases: `https://api.github.com/repos/Lucas-Carbo/TestingRelease-Updater/releases/latest`
2. Descargar `HelloAppInstaller.exe`
3. Reemplazar la versión anterior
4. Ejecutar la nueva versión

Ejemplo JSON de respuesta:
```json
{
  "tag_name": "v0.0.1",
  "assets": [
    {
      "name": "HelloAppInstaller.exe",
      "browser_download_url": "https://github.com/Lucas-Carbo/TestingRelease-Updater/releases/download/v0.0.1/HelloAppInstaller.exe"
    }
  ]
}
```

