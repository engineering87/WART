// (c) 2019 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using System;

namespace WART_Client
{
    public class Program
    {
        private static void Main()
        {
            Console.WriteLine("Starting WartTestClient");
            
            #region JWT Client Test

            WartTestClientJwt.ConnectAsync();

            #endregion

            #region No authentication Client Test

            //WartTestClient.ConnectAsync();

            #endregion

            Console.ReadLine();
        }
    }
}
