// (c) 2025 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace WART_Client
{
    /// <summary>
    /// A simple SignalR WART test client with Cookie authentication.
    /// </summary>
    public static class WartTestClientCookie
    {
        public static async Task ConnectAsync(string hubUrl)
        {
            try
            {
                var cookieContainer = new CookieContainer();
                var handler = new HttpClientHandler
                {
                    CookieContainer = cookieContainer,
                    UseCookies = true,
                    AllowAutoRedirect = true
                };

                using var httpClient = new HttpClient(handler);

                var loginContent = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("username", "test_username"),
                    new KeyValuePair<string, string>("password", "test_password")
                });

                var loginUri = new Uri(new Uri(hubUrl), "/api/TestCookie/login");
                var loginResponse = await httpClient.PostAsync(loginUri, loginContent);
                loginResponse.EnsureSuccessStatusCode();

                Console.WriteLine("Login successful. Connecting to SignalR...");

                //var uri = new Uri(hubUrl);
                //cookieContainer.Add(uri, new Cookie("WART.AuthCookie", "sample_value"));

                var hubConnection = new HubConnectionBuilder()
                        .WithUrl(hubUrl, options =>
                        {
                            options.HttpMessageHandlerFactory = _ => handler;
                            options.Transports = HttpTransportType.WebSockets |
                                                 HttpTransportType.ServerSentEvents |
                                                 HttpTransportType.LongPolling;
                        })
                        .WithAutomaticReconnect()
                        .Build();

                hubConnection.On<string>("Send", (data) =>
                {
                    Console.WriteLine(data);
                    Console.WriteLine($"Message size: {Encoding.UTF8.GetBytes(data).Length} bytes");
                    Console.WriteLine();
                });

                hubConnection.Closed += async (ex) =>
                {
                    Console.WriteLine($"Connection closed: {ex?.Message}");
                    await Task.Delay(new Random().Next(0, 5) * 1000);
                    if (hubConnection != null)
                        await hubConnection.StartAsync();
                };

                hubConnection.On<Exception>("ConnectionFailed", (ex) =>
                {
                    Console.WriteLine($"Connection failed: {ex.Message}");
                    return Task.CompletedTask;
                });

                await hubConnection.StartAsync();
                Console.WriteLine("SignalR connection started.");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            await Task.CompletedTask;
        }
    }
}