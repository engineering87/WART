// (c) 2025 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi;
using WART_Core.Enum;
using WART_Core.Middleware;
using WART_MinimalApi.Endpoints;

var builder = WebApplication.CreateBuilder(args);

// Add the WART middleware service extension

// Default without authentication
builder.Services.AddWartMiddleware();

// With JWT authentication
//builder.Services.AddWartMiddleware(hubType: HubType.JwtAuthentication, tokenKey: "dn3341fmcscscwe28419brhwbwgbss4t");

// With Cookie authentication
//builder.Services.AddWartMiddleware(hubType: HubType.CookieAuthentication);

// Register the Swagger generator
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "WART-MinimalApi", Version = "v1" });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

// Enable Swagger
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "WART-MinimalApi");
    c.RoutePrefix = string.Empty;
});

// Use the WART middleware builder extension
// Default without authentication
app.UseWartMiddleware();

// With JWT authentication
//app.UseWartMiddleware(HubType.JwtAuthentication);

// With Cookie authentication
//app.UseWartMiddleware(HubType.CookieAuthentication);

// Map all Minimal API endpoints
app.MapTestEndpoints();

app.Run();
