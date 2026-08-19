// (c) 2021 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using System;

namespace WART_Core.Enum
{
    /// <summary>
    /// Types of hubs supported.
    /// </summary>
    public enum HubType
    {
        /// <summary>
        /// Simple SignalR hub without authentication.
        /// </summary>
        /// <remarks>
        /// <b>SECURITY WARNING:</b> This mode maps an unauthenticated SignalR hub that broadcasts
        /// live API request and response payloads to every connected client with no credential check.
        /// Any anonymous network client can connect and receive sensitive data (request bodies,
        /// response bodies, HTTP paths). Use <see cref="JwtAuthentication"/> or
        /// <see cref="CookieAuthentication"/> instead.
        /// See https://github.com/engineering87/WART/security/advisories
        /// </remarks>
        [Obsolete("NoAuthentication broadcasts all API events to any anonymous client and is insecure. " +
                  "Use HubType.JwtAuthentication or HubType.CookieAuthentication instead. " +
                  "See https://github.com/engineering87/WART/security/advisories")]
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