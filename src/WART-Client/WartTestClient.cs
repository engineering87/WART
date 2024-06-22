// (c) 2021 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Text;
using System.Threading.Tasks;

namespace WART_Client
{
    /// <summary>
    /// A simple SignalR WART test client without authentication.
    /// </summary>
    public class WartTestClient
    {
        public static async void ConnectAsync(string wartHubUrl)
        {
            try
            {
                var hubConnection = new HubConnectionBuilder()
                    .WithUrl(wartHubUrl, options =>
                    {
                        options.Transports = HttpTransportType.WebSockets |
                                             HttpTransportType.ServerSentEvents |
                                             HttpTransportType.LongPolling;
                    })
                    .WithAutomaticReconnect()
                    .Build();

                hubConnection.On<string>("Send", (data) =>
                {
                    Console.WriteLine(data);
                    Console.WriteLine($"Message size: {Encoding.UTF8.GetBytes(data).Length} byte");
                    Console.WriteLine(Environment.NewLine);
                });

                hubConnection.Closed += async (exception) =>
                {
                    Console.WriteLine(exception);
                    Console.WriteLine(Environment.NewLine);
                    await Task.Delay(new Random().Next(0, 5) * 1000);
                    await hubConnection.StartAsync();
                };

                hubConnection.On<Exception>("ConnectionFailed", (exception) =>
                {
                    Console.WriteLine(exception);
                    Console.WriteLine(Environment.NewLine);
                    return Task.CompletedTask;
                });

                await hubConnection.StartAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
