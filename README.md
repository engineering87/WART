[![Github license](mit.svg)](https://github.com/engineering87/WART/blob/develop/LICENSE)

<img src="https://github.com/engineering87/WART/blob/master/wart_logo.jpg" width="300">

WART is a C# .NET Core library that allows extending any WebApi controller and forwarding any calls received by the controller to a SignalR hub.
The SignalR hub on which the controller's call events will be sent will be used to send notifications with information about the call including the request and the response. WART supports JWT authentication for SignalR.

### How it works
WART implements a custom controller which overrides the OnActionExecuting and OnActionExecuted methods to retrieve the request and the response and encapsulates them in a **WartEvent** object which will be sent via SignalR on the **WartHub**.

### How to use it

To use the WART library, each WebApi controller must extend the **WartController** controller:

```csharp
using WART_Core.Controllers;
using WART_Core.Hubs;

[ApiController]
[Route("api/[controller]")]
public class TestController : WartController
```

each controller must implement the following constructor, for example:

```csharp
public TestController(IHubContext<WartHub> messageHubContext, 
ILogger<WartController> logger) : base(messageHubContext, logger)
{
}
```

WART support JWT bearer authentication on SignalR hub, if you want to use JWT authentication use the followinf controller extension:

```csharp
using WART_Core.Controllers;
using WART_Core.Hubs;

[ApiController]
[Route("api/[controller]")]
public class TestController : WartControllerJwt
```

You also need to enable SignalR in the WebAPI solution and map the **WartHub**.
To do this, add the following configurations in the Startup.cs class:

```csharp
using WART_Core.Middleware;
```

In the ConfigureServices section add following:

```csharp
services.AddWartMiddleware();
```

or by specifying JWT authentication:


```csharp
services.AddWartMiddleware(hubType:HubType.JwtAuthentication, tokenKey:"password_here");
```

In the Configure section add the following:

```csharp
app.UseWartMiddleware();
```

or by specifying JWT authentication:

```csharp
app.UseWartMiddleware(HubType.JwtAuthentication);
```

Alternatively, it is possible to specify a custom hub name:

```csharp
app.UseWartMiddleware("hubname");
```

at this point it will be sufficient to connect via SignalR to the WartHub to receive notifications in real time of any call on the controller endpoints. 
For example:

```csharp
var hubConnection = new HubConnectionBuilder()
    .WithUrl("http://localhost:52086/warthub")
    .Build();
    
hubConnection.On<string>("Send", (data) =>
{
  // data is the WartEvent JSON
});
```

or with JWT authentication:

```csharp
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
  // data is the WartEvent JSON
});
```

In the source code you can find a simple test client and WebApi project.

### NuGet

The library is available on NuGet packetmanager.

https://www.nuget.org/packages/WART-Core/

### Contributing
Thank you for considering to help out with the source code!
If you'd like to contribute, please fork, fix, commit and send a pull request for the maintainers to review and merge into the main code base.

**Getting started with Git and GitHub**

 * [Setting up Git for Windows and connecting to GitHub](http://help.github.com/win-set-up-git/)
 * [Forking a GitHub repository](http://help.github.com/fork-a-repo/)
 * [The simple guide to GIT guide](http://rogerdudler.github.com/git-guide/)
 * [Open an issue](https://github.com/engineering87/WART/issues) if you encounter a bug or have a suggestion for improvements/features

### Licensee
WART source code is available under MIT License, see license in the source.

### Contact
Please contact at francesco.delre.87[at]gmail.com for any details.
