using System;
using System.Threading;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

class Program
{
    static async Task Main()
    {
        string[] urls =
        {
            "https://github.com/Aria0104",
            "https://support.microsoft.com/ru-ru/microsoft-edge"

        };
        CancellationTokenSource cts = new CancellationTokenSource();

        Console.WriteLine("Нажмите любую клавишу для отмены");


        Task.Run(() =>
        {
            Console.ReadKey();
            cts.Cancel();
        });

        Progress<int> progress = new Progress<int>(percent =>
        {
            Console.WriteLine($"Прогресс: {percent}% | Поток: {Thread.CurrentThread.ManagedThreadId}");
        });

        try
        {
            int totalChars = 0;
            foreach (var uri in urls)
            {
                totalChars += await DownloadAndProcessAsync(uri, cts.Token, progress);
            }
            Console.WriteLine($"Общее количество символов: {totalChars}");
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("Загрузка была отменена.");
        }
    }
    static async Task<int> DownloadAndProcessAsync(string url, CancellationToken token, IProgress<int> progress)
    {
        using HttpClient client = new HttpClient();
        Console.WriteLine($"Начало загрузки {url} | Поток: {Thread.CurrentThread.ManagedThreadId}");

        string content = await client.GetStringAsync(url, token);

        for (int i = 1; i <= 10; i++)
        {
            token.ThrowIfCancellationRequested();
            await Task.Delay(100, token);
            progress.Report(i * 10);
        }

        Console.WriteLine($"Загрузка завершена {url}");

        return content.Length;


    }

}