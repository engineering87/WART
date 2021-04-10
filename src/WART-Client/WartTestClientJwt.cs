// (c) 2021 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Threading.Tasks;

namespace WART_Client
{
    /// <summary>
    /// A simple SignalR WART test client with JWT authentication.
    /// </summary>
    public class WartTestClientJwt
    {
        private const string Token = "dn3341fmcscscwe28419";
        private static readonly SymmetricSecurityKey SecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Token));
        private static readonly JwtSecurityTokenHandler JwtTokenHandler = new JwtSecurityTokenHandler();

        public static async void ConnectAsync()
        {
            try
            {
                var hubConnection = new HubConnectionBuilder()
                    .WithUrl($"http://localhost:51392/warthub", options =>
                    {
                        options.SkipNegotiation = true;
                        options.Transports = HttpTransportType.WebSockets;
                        options.AccessTokenProvider = () => Task.FromResult(GenerateToken());
                    })
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

        private static string GenerateToken()
        {
            var credentials = new SigningCredentials(SecurityKey, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(expires: DateTime.UtcNow.AddHours(24), signingCredentials: credentials);
            return JwtTokenHandler.WriteToken(token);
        }
    }
}
