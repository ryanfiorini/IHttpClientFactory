using Microsoft.Extensions.DependencyInjection;
using Polly;
using System;
using System.Net.Http;

namespace IHttpClientFactoryExample
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("START");

            var services = new ServiceCollection();

            services.AddHttpClient("GitHub", client =>
                {
                    client.BaseAddress = new Uri("https://api.github.com/");
                    client.DefaultRequestHeaders.Add("Accept", "application/vnd.github.v3+json");
                    client.DefaultRequestHeaders.Add("User-Agent", "IHttpClientFactoryExample");
                })
                .AddTransientHttpErrorPolicy(pol => 
                    pol.WaitAndRetryAsync(3, _ => TimeSpan.FromMilliseconds(300))
                );

            var serviceProvider = services.BuildServiceProvider();

            var httpClientFactory = serviceProvider.GetService<IHttpClientFactory>();
            var client = httpClientFactory.CreateClient("GitHub");
            var accountInfo = client.GetStringAsync("users/ryanfiorini").Result;

            Console.WriteLine($"Account Info: { accountInfo }");
            Console.WriteLine("END");
        }
    }
}
