using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Async
{
    class Program
    {
        static readonly HttpClient httpClient = new HttpClient();
        

        static readonly IEnumerable<string> listOfUrls = new string[]
        {
            "https://docs.microsoft.com",
            "https://www.facebook.com/acainfo",
            "https://www.youtube.com/watch?v=vE3BAgh_VAQ"
        };

        static Task Main() => SumPageSizesAsync();

        static async Task SumPageSizesAsync()
        {
            var stopwatch = Stopwatch.StartNew();

            IEnumerable<Task<int>> downloadTasksQuery =
                from url in listOfUrls
                select ProcessUrlAsync(url, httpClient);

            List<Task<int>> downloadTasks = downloadTasksQuery.ToList();

            int total=0;
            
            while (downloadTasks.Any())
            {
                Task<int> finishedTask = await Task.WhenAny(downloadTasks);
                downloadTasks.Remove(finishedTask);
                total += await finishedTask;
            }

            stopwatch.Stop();

            Console.WriteLine($"\nTotal bytes returned:  {total:#,#}");

            Console.WriteLine($"\nElapsed time: {stopwatch.Elapsed}\n");
        }

        static async Task<int> ProcessUrlAsync(string url, HttpClient client)
        {
            byte[] content = await client.GetByteArrayAsync(url);
            Console.WriteLine($"{url} {content.Length,10:#,#}");

            return content.Length;
        }
    }
}
