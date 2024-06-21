using System;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace B39Notifier
{
    class Program
    {
        private static readonly string Url = "https://docs.base39.com.br/release-notes/intro";

        private static readonly string LastHashFilePath = "lasthash.txt";

        static async Task Main()
        {
            await CheckForUpdates();

            Console.WriteLine("Pressione qualquer tecla para sair...");            
            Thread.Sleep(2000);
            Console.ReadKey();
        }

        private static async Task CheckForUpdates()
        {
            Console.WriteLine("Verificando atualizações...");

            string? currentContent = await GetSiteContent();
            if (string.IsNullOrEmpty(currentContent))
                return;

            string currentHash = ComputeHash(currentContent);

            if (File.Exists(LastHashFilePath))
            {
                string lastHash = await File.ReadAllTextAsync(LastHashFilePath);

                if (lastHash == currentHash)
                {
                   Console.WriteLine("Nenhuma atualização detectada.");
                    ShowAlert("SEM ATUALIZAÇÃO DO B39", $"O site {Url} não teve atualizações desde a última verificação.");
                    return;
                }
            }

            await File.WriteAllTextAsync(LastHashFilePath, currentHash);
            Console.WriteLine("Atualização detectada.");
            ShowAlert("ATUALIZAÇÃO DO B39", $"O site {Url} foi atualizado.");
        }

        private static async Task<string?> GetSiteContent()
        {
            try
            {
                using var httpClient = new HttpClient();
                var response = await httpClient.GetStringAsync(Url);
                return response;
            }
            catch (Exception ex)
            {
               Console.WriteLine($"Erro ao obter o conteúdo do site: {ex.Message}");
                return null;
            }
        }

        private static string ComputeHash(string input)
        {
           byte[] inputBytes = Encoding.ASCII.GetBytes(input);

            byte[] hashBytes = MD5.HashData(inputBytes);

           return Convert.ToHexString(hashBytes);
        }

        private static void ShowAlert(string title, string message)
        {
            Console.WriteLine("===============================================================================");
            Console.WriteLine(title);
            Console.WriteLine(message);
            Console.WriteLine("===============================================================================");
        }
    }
}
