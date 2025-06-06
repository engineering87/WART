﻿// (c) 2021 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
namespace WART_Core.Enum
{
    /// <summary>
    /// Types of hubs supported.
    /// </summary>
    public enum HubType
    {
        /// <summary>
        /// Simple SignalR hub without authentication
        /// </summary>
        NoAuthentication,

        /// <summary>
        /// SignalR hub with JWT authentication
        /// </summary>
        JwtAuthentication,

        /// <summary>
        /// SignalR hub with Cookie authentication
        /// </summary>
        CookieAuthentication
    }
}