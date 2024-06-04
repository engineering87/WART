// (c) 2021 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using Microsoft.AspNetCore.SignalR.Client;
using System;

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
                    .WithUrl(wartHubUrl)
                    .WithAutomaticReconnect()
                    .Build();

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
