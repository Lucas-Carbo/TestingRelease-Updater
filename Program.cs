using System;
using System.IO;
using System.Threading;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("═══════════════════════════════════════════");
        Console.WriteLine("         HelloApp v0.0.1");
        Console.WriteLine("═══════════════════════════════════════════\n");

        string installPath = GetInstallPath();
        
        Console.WriteLine($"✓ Ruta de instalación: {installPath}\n");
        Console.WriteLine("Presiona Ctrl+C para salir\n");

        RunApplication();
    }

    static string GetInstallPath()
    {
        // Ruta por defecto: Documentos del usuario
        string defaultPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            "HelloApp"
        );

        Console.WriteLine("¿Dónde deseas instalar la aplicación?");
        Console.WriteLine($"1. Documentos (Recomendado): {defaultPath}");
        Console.WriteLine($"2. Escritorio: {Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "HelloApp")}");
        Console.WriteLine($"3. Personalizado: Ingresa tu propia ruta");
        Console.WriteLine($"4. Ruta actual (sin instalar): {Directory.GetCurrentDirectory()}");
        Console.Write("\nSelecciona una opción (1-4): ");

        string choice = Console.ReadLine() ?? "1";
        string selectedPath = choice switch
        {
            "1" => defaultPath,
            "2" => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "HelloApp"),
            "3" => GetCustomPath(),
            "4" => Directory.GetCurrentDirectory(),
            _ => defaultPath
        };

        // Crear directorio si no existe
        if (!Directory.Exists(selectedPath))
        {
            try
            {
                Directory.CreateDirectory(selectedPath);
                Console.WriteLine($"✓ Directorio creado: {selectedPath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ Error al crear directorio: {ex.Message}");
                Console.WriteLine($"  Usando: {defaultPath}");
                Directory.CreateDirectory(defaultPath);
                return defaultPath;
            }
        }

        return selectedPath;
    }

    static string GetCustomPath()
    {
        while (true)
        {
            Console.Write("Ingresa la ruta completa: ");
            string path = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(path))
            {
                Console.WriteLine("La ruta no puede estar vacía. Intenta de nuevo.");
                continue;
            }

            // Validar que la ruta sea válida
            try
            {
                var pathInfo = new DirectoryInfo(path);
                return path;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ruta inválida: {ex.Message}");
                Console.WriteLine("Intenta de nuevo.\n");
            }
        }
    }

    static void RunApplication()
    {
        Console.WriteLine("Iniciando... Mostrará 'Hola como estas' cada 1 minuto\n");
        Console.WriteLine("─────────────────────────────────────────");

        while (true)
        {
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Hola como estas");
            Thread.Sleep(60000); // 60000 ms = 1 minuto
        }
    }
}
