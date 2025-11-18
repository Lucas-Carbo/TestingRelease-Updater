using System;
using System.IO;
using System.Threading;
using System.Diagnostics;

class Program
{
    private static string _logFilePath;
    private static bool _isRunning = true;

    static void Main(string[] args)
    {
        string appDir = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
        _logFilePath = Path.Combine(appDir, "helloapp.log");

        LogMessage("═══════════════════════════════════════════");
        LogMessage("         HelloApp v1.0.3");
        LogMessage("═══════════════════════════════════════════");
        LogMessage($"Iniciado en: {appDir}");

        // Capturar Ctrl+C para salida limpia
        Console.CancelKeyPress += (sender, e) =>
        {
            e.Cancel = true;
            _isRunning = false;
            LogMessage("Deteniendo aplicación...");
        };

        RunApplication();
        LogMessage("Aplicación terminada");
    }

    static void RunApplication()
    {
        LogMessage("HelloApp corriendo en background - Registrando eventos cada 1 minuto");
        LogMessage("Presiona Ctrl+C para salir\n");

        int count = 0;
        while (_isRunning)
        {
            try
            {
                count++;
                LogMessage($"[Evento #{count}] Hola como estas");
                Thread.Sleep(30000); // 30000 ms = 30 segundos
            }
            catch (ThreadAbortException)
            {
                LogMessage("Aplicación interrumpida");
                break;
            }
            catch (Exception ex)
            {
                LogMessage($"Error: {ex.Message}");
            }
        }
    }

    static void LogMessage(string message)
    {
        string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        string logEntry = $"[{timestamp}] {message}";

        // Mostrar en consola (útil para debugging local)
        Console.WriteLine(logEntry);

        // Guardar en archivo (para el UpdaterService pueda verificar que funciona)
        try
        {
            File.AppendAllText(_logFilePath, logEntry + Environment.NewLine);
        }
        catch
        {
            // Ignorar errores de escritura de log
        }
    }
}
