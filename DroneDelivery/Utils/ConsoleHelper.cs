using System;

namespace DroneDelivery.Utils
{
    public static class ConsoleHelper
    {
        public static void ColorText(string texto, string cor)
        {
            if (!Enum.TryParse<ConsoleColor>(cor, true, out ConsoleColor consoleColor))
                throw new ArgumentException("Cor inválida", nameof(cor));

            Console.ForegroundColor = consoleColor;
            Console.WriteLine(texto);
            Console.ResetColor();
        }

        public static void Encerrar(string texto = null)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(texto))
                    Console.WriteLine($"\n{texto}");
                else
                    Console.WriteLine("\nAperte ENTER para sair");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro: {ex.Message}");
            }

            Console.ReadKey();
        }

        // Atalhos de cor
        public static void Info(string text) => ColorText(text, "Cyan");
        public static void Sucesso(string text) => ColorText(text, "Green");
        public static void Erro(string text) => ColorText(text, "Red");
        public static void Alerta(string text) => ColorText(text, "Yellow");

        public static void PrioridadeAlta(string texto) => ColorText(texto, "DarkMagenta");
        public static void PrioridadeMedia(string texto) => ColorText(texto, "Blue");
        public static void PrioridadeBaixa(string texto) => ColorText(texto, "DarkCyan");

        public static void States(string texto) => ColorText(texto, "DarkGray");

    }
}
