# WART (WebApi Real-Time)
Turns WebApi calls into SignalR events.

WART is a C# .NET Core library that allows extending any WebApi controller and forwarding any calls received by the controller to a SignalR hub.
The SignalR hub on which the controller's call events will be sent will be used to send notifications with information about the call including the request and the response.

### How it works
WART implements a custom controller which overrides the OnActionExecuting and OnActionExecuted methods to retrieve the request and the response and encapsulates them in a **WarEvent** object which will be sent via SignalR on the **WartHub**.

### How to use it

To use the WART library, each WebApi controller must extend the **WartController** controller:

```csharp
[ApiController]
[Route("api/[controller]")]
public class TestController : WartController
```

you will also need to enable SignalR in the solution and map the **WartHub**.
In the ConfigureServices section add following:

```csharp
services.AddWartMiddleware();
```

In the Configure section add the following:

```csharp
app.UseEndpoints(endpoints =>
{
	endpoints.MapControllers();
	endpoints.MapHub<WartHub>("/warthub");
});
```

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
