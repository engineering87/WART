// (c) 2019 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using System;
using Microsoft.Extensions.Configuration;

namespace WART_Client
{
    public class Program
    {
        private static void Main()
        {
            Console.WriteLine("Starting WartTestClient");

            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            var wartHubUrl = $"{configuration["Scheme"]}://{configuration["Host"]}:{configuration["Port"]}/{configuration["Hubname"]}";

            Console.WriteLine($"Connecting to {wartHubUrl}");

            var auth = configuration["AuthenticationJwt"];

            if (bool.Parse(auth))
            {
                var key = configuration["Key"];
                WartTestClientJwt.ConnectAsync(wartHubUrl, key);
            }
            else
            {
                WartTestClient.ConnectAsync(wartHubUrl);
            }

            Console.WriteLine($"Connected to {wartHubUrl}");
            Console.ReadLine();
        }
    }
}
