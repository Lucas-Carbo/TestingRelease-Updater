using System;
using System.Threading;
using System.Threading.Tasks;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Aplicación v0.0.1 iniciada");
        Console.WriteLine("Mostrará 'Hola como estas' cada 1 minuto");
        Console.WriteLine("Presiona Ctrl+C para salir\n");

        while (true)
        {
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Hola como estas");
            Thread.Sleep(60000); // 60000 ms = 1 minuto
        }
    }
}
