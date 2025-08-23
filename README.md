# WART - WebApi Real Time

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![Nuget](https://img.shields.io/nuget/v/WART-Core?style=plastic)](https://www.nuget.org/packages/WART-Core)
![NuGet Downloads](https://img.shields.io/nuget/dt/WART-Core)
[![issues - wart](https://img.shields.io/github/issues/engineering87/WART)](https://github.com/engineering87/WART/issues)
[![stars - wart](https://img.shields.io/github/stars/engineering87/WART?style=social)](https://github.com/engineering87/WART)
[![Sponsor me](https://img.shields.io/badge/Sponsor-❤-pink)](https://github.com/sponsors/engineering87)

<img src="https://github.com/engineering87/WART/blob/develop/wart_logo.jpg" width="300">

WART is a lightweight C# .NET library that extends your Web API controllers to forward incoming calls directly to a SignalR Hub.  
The Hub broadcasts rich, structured events containing request and response details in **real-time**.  
Supports **JWT** and **Cookie Authentication** for secure communication.

## 📑 Table of Contents
- [Features](#-features)
- [Installation](#-installation)
- [How It Works](#️-how-it-works)
- [Usage](#-usage)
  - [Basic Setup](#basic-setup)
  - [Using JWT Authentication](#using-jwt-authentication)
  - [Custom Hub Names](#custom-hub-names)
  - [Multiple Hubs](#multiple-hubs)
  - [Client Example](#client-example)
- [Supported Authentication Modes](#-supported-authentication-modes)
- [Excluding APIs from Event Propagation](#-excluding-apis-from-event-propagation)
- [Group-based Event Dispatching](#-group-based-event-dispatching)
- [NuGet](#-nuget)
- [Contributing](#-contributing)
- [License](#-license)
- [Contact](#-contact)

## ✨ Features
- Converts REST API calls into SignalR events, enabling real-time communication.
- Provides controllers (`WartController`, `WartControllerJwt`, `WartControllerCookie`) for automatic SignalR event broadcasting.
- Supports JWT authentication for SignalR hub connections.
- Allows API exclusion from event broadcasting with `[ExcludeWart]` attribute.
- Enables group-specific event dispatching with `[GroupWart("group_name")]`.
- Configurable middleware (`AddWartMiddleware`) for flexible integration.

## 📦 Installation
Install from **NuGet**

```bash
dotnet add package WART-Core
```

### ⚙️ How it works
WART overrides `OnActionExecuting` and `OnActionExecuted` in a custom base controller.
For every API request/response:
1) Captures request and response data.
2) Wraps them in a `WartEvent`.
3) Publishes it through a SignalR Hub to all connected clients.

## 🚀 Usage
### Basic Setup
Extend your API controllers from `WartController`:

```csharp
using WART_Core.Controllers;
using WART_Core.Hubs;

[ApiController]
[Route("api/[controller]")]
public class TestController : WartController
{
    public TestController(IHubContext<WartHub> hubContext, ILogger<WartController> logger)
        : base(hubContext, logger) { }
}
```

Register WART in `Startup.cs`:

```csharp
using WART_Core.Middleware;

public void ConfigureServices(IServiceCollection services)
{
    services.AddWartMiddleware(); // No authentication
}

public void Configure(IApplicationBuilder app)
{
    app.UseWartMiddleware();
}
```

### Using JWT Authentication

```csharp
services.AddWartMiddleware(hubType: HubType.JwtAuthentication, tokenKey: "your_secret_key");
app.UseWartMiddleware(HubType.JwtAuthentication);
```

Extend from `WartControllerJwt`:

```csharp
public class TestController : WartControllerJwt
{
    public TestController(IHubContext<WartHubJwt> hubContext, ILogger<WartControllerJwt> logger)
        : base(hubContext, logger) { }
}
```

### Custom Hub Names
You can specify custom hub routes:

```csharp
app.UseWartMiddleware("customhub");
```

### Multiple Hubs
You can configure multiple hubs at once by passing a list of hub names:

```csharp
var hubs = new[] { "orders", "products", "notifications" };

app.UseWartMiddleware(hubs);
```

This is useful for separating traffic by domain.

### Client Example
#### Without authentication:

```csharp
var hubConnection = new HubConnectionBuilder()
    .WithUrl("http://localhost:5000/warthub")
    .Build();

hubConnection.On<string>("Send", data =>
{
    // 'data' is a WartEvent JSON
});

await hubConnection.StartAsync();
```

#### With JWT authentication:

```csharp
var hubConnection = new HubConnectionBuilder()
    .WithUrl("http://localhost:5000/warthub", options =>
    {
        options.AccessTokenProvider = () => Task.FromResult(GenerateToken());
    })
    .WithAutomaticReconnect()
    .Build();

hubConnection.On<string>("Send", data =>
{
    // Handle WartEvent JSON
});

await hubConnection.StartAsync();
```

## 🔐 Supported Authentication Modes

| Mode                     | Description                                                               | Hub Class           | Required Middleware      |
|--------------------------|---------------------------------------------------------------------------|----------------------|---------------------------|
| **No Authentication**    | Open access without identity verification                                 | `WartHub`            | None                      |
| **JWT (Bearer Token)**   | Authentication via JWT token in the `Authorization: Bearer <token>` header | `WartHubJwt`         | `UseJwtMiddleware()`      |
| **Cookie Authentication**| Authentication via HTTP cookies issued after login                        | `WartHubCookie`      | `UseCookieMiddleware()`   |

> ⚙️ Authentication mode is selected through the `HubType` configuration in the application startup.

### 🚫 Excluding APIs from Event Propagation
There might be scenarios where you want to exclude specific APIs from propagating events to connected clients. This can be particularly useful when certain endpoints should not trigger updates, notifications, or other real-time messages through SignalR. To achieve this, you can use a custom filter called `ExcludeWartAttribute`. By decorating the desired API endpoints with this attribute, you can prevent them from being included in the SignalR event propagation logic, for example:

```csharp
[HttpGet("{id}")]
[ExcludeWart]
public ActionResult<TestEntity> Get(int id)
{
    var item = Items.FirstOrDefault(x => x.Id == id);
    if (item == null)
    {
        return NotFound();
    }
    return item;
}
```

### 👥 Group-based Event Dispatching
WART enables sending API events to specific groups in SignalR by specifying the group name in the query string. This approach allows for flexible and targeted event broadcasting, ensuring that only the intended group of clients receives the event. 
By decorating an API method with `[GroupWart("group_name")]`, it is possible to specify the SignalR group name to which the dispatch of specific events for that API is restricted. This ensures that only the clients subscribed to the specified group ("SampleGroupName") will receive the related events, allowing for targeted, group-based communication in a SignalR environment.

```csharp
[HttpPost]
[GroupWart("SampleGroupName")]
public ActionResult<TestEntity> Post([FromBody] TestEntity entity)
{
    Items.Add(entity);
    return entity;
}
```

By appending `?WartGroup=group_name` to the URL, the library enables dispatching events from individual APIs to a specific SignalR group, identified by `group_name`. This allows for granular control over which clients receive the event, leveraging SignalR’s built-in group functionality.

### 📦 NuGet
The library is available on [NuGet](https://www.nuget.org/packages/WART-Core/).

### 🤝 Contributing
Contributions are welcome!
Steps to get started:

 * [Setting up Git](https://docs.github.com/en/get-started/getting-started-with-git/set-up-git)
 * [Fork the repository](https://docs.github.com/en/pull-requests/collaborating-with-pull-requests/working-with-forks/fork-a-repo)
 * [Open an issue](https://github.com/engineering87/WART/issues) if you encounter a bug or have a suggestion for improvements/features
 * Submit a Pull Request.

### 📄 License
WART source code is available under MIT License, see license in the source.

### 📬 Contact
Please contact at francesco.delre[at]protonmail.com for any details.
