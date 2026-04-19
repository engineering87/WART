// (c) 2025 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using WART_Core.Middleware;
using WART_MinimalApi.Entity;

namespace WART_MinimalApi.Endpoints
{
    /// <summary>
    /// Minimal API endpoint definitions equivalent to TestController in WART-Api.
    /// Demonstrates WART integration using the UseWart() endpoint filter.
    /// </summary>
    public static class TestEndpoints
    {
        private static readonly List<TestEntity> Items =
        [
            new TestEntity { Id = 1, Param = "Item1" },
            new TestEntity { Id = 2, Param = "Item2" },
            new TestEntity { Id = 3, Param = "Item3" }
        ];

        public static void MapTestEndpoints(this WebApplication app)
        {
            var group = app.MapGroup("/api/test")
                .WithTags("Test");

            // GET /api/test — returns all items (WART enabled)
            group.MapGet("/", () => Results.Ok(Items))
                .UseWart();

            // GET /api/test/{id} — returns a single item (excluded from WART)
            group.MapGet("/{id:int}", (int id) =>
            {
                var item = Items.FirstOrDefault(x => x.Id == id);
                return item is not null ? Results.Ok(item) : Results.NotFound();
            })
            .UseWart()
            .ExcludeFromWart();

            // POST /api/test — creates an item (WART with group-based dispatching)
            group.MapPost("/", (TestEntity entity) =>
            {
                Items.Add(entity);
                return Results.Ok(entity);
            })
            .UseWart()
            .WartGroup("SampleGroupName");

            // PATCH /api/test/{id} — partially updates an item
            group.MapPatch("/{id:int}", (int id, TestEntity entity) =>
            {
                var item = Items.FirstOrDefault(x => x.Id == id);
                if (item is null)
                {
                    return Results.NotFound();
                }
                item.Param = entity.Param;
                return Results.Ok(item);
            })
            .UseWart();

            // PUT /api/test/{id} — fully updates an item
            group.MapPut("/{id:int}", (int id, TestEntity entity) =>
            {
                var item = Items.FirstOrDefault(x => x.Id == id);
                if (item is null)
                {
                    return Results.NotFound();
                }
                item.Param = entity.Param;
                return Results.Ok(item);
            })
            .UseWart();

            // DELETE /api/test/{id} — deletes an item
            group.MapDelete("/{id:int}", (int id) =>
            {
                var item = Items.FirstOrDefault(x => x.Id == id);
                if (item is null)
                {
                    return Results.NotFound();
                }
                Items.Remove(item);
                return Results.Ok(item);
            })
            .UseWart();
        }
    }
}
