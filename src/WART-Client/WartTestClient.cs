// (c) 2019 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Threading.Tasks;

namespace WART_Client
{
    /// <summary>
    /// A simple SignalR WART test client
    /// </summary>
    public class WartTestClient
    {
        /// <summary>
        /// Simple SignalR WART test client
        /// </summary>
        public WartTestClient()
        {

        }

        public async void ConnectAsync()
        {
            try
            {
                var hubConnection = new HubConnectionBuilder()
                    .WithUrl("http://localhost:52086/warthub")
                    .Build();

                hubConnection.Closed += async (error) =>
                {
                    Console.WriteLine("Closed connection");
                    await Task.Delay(new Random().Next(0, 5) * 1000);
                    await hubConnection.StartAsync();
                };

                hubConnection.Reconnected += async (message) =>
                {
                    Console.WriteLine("Reconnected");
                    await Task.Delay(new Random().Next(0, 5) * 1000);
                };

                hubConnection.On<string>("Send", (data) =>
                {
                    Console.WriteLine(data);
                    Console.WriteLine(Environment.NewLine);
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
