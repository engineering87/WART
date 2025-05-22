// (c) 2019 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace WART_Client
{
    public class Program
    {
        private static async Task Main()
        {
            Console.WriteLine("Starting WartTestClient");

            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            var wartHubUrl = $"{configuration["Scheme"]}://{configuration["Host"]}:{configuration["Port"]}/{configuration["Hubname"]}";
            var wartHubUrlGroup = configuration["WartGroup"] != string.Empty ? $"?WartGroup={configuration["WartGroup"]}" : string.Empty;
            wartHubUrl += wartHubUrlGroup;

            Console.WriteLine($"Connecting to {wartHubUrl}");

            var auth = configuration["AuthenticationType"] ?? "NoAuth";

            switch (auth.ToLowerInvariant())
            {
                default:
                case "noauth":
                    {
                        await WartTestClient.ConnectAsync(wartHubUrl);
                        break;
                    }
                case "jwt":
                    {
                        var key = configuration["Key"];
                        await WartTestClientJwt.ConnectAsync(wartHubUrl, key);
                        break;
                    }
                case "cookie":
                    {
                        await WartTestClientCookie.ConnectAsync(wartHubUrl);
                        break;
                    }
            }

            Console.WriteLine($"Connected to {wartHubUrl}");
            Console.ReadLine();
        }
    }
}
