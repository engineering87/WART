// (c) 2019 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using System;

namespace WART_Client
{
    public class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("Starting WartTestClient");

            var client = new WartTestClient();
            client.ConnectAsync();

            Console.ReadLine();
        }
    }
}
