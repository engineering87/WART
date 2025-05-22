// (c) 2025 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using WART_Api.Entity;
using WART_Core.Controllers;
using WART_Core.Filters;
using WART_Core.Hubs;

namespace WART_Api.Controllers
{
    /// <summary>
    /// A simple controller example extended by the WartController with Cookie authentication.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class TestCookieController : WartControllerCookie
    {
        private static List<TestEntity> Items = new List<TestEntity>
            {
                new TestEntity { Id = 1, Param = "Item1" },
                new TestEntity { Id = 2, Param = "Item2" },
                new TestEntity { Id = 3, Param = "Item3" }
            };

        public TestCookieController(IHubContext<WartHubCookie> messageHubContext, ILogger<TestCookieController> logger) : base(messageHubContext, logger)
        {
        }

        // Login endpoint: issues authentication cookie using "WartCookieAuth" scheme
        [HttpPost("login")]
        [ExcludeWart] // Exclude from event interception if using custom filters
        public async Task<IActionResult> Login([FromForm] string username, [FromForm] string password)
        {
            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, username)
                };

                var identity = new ClaimsIdentity(claims, "WartCookieAuth");
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync("WartCookieAuth", principal, new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTime.UtcNow.AddHours(1)
                });

                return Ok("Login successful.");
            }

            return Unauthorized("Invalid credentials.");
        }

        [HttpGet]
        public IEnumerable<TestEntity> Get()
        {
            return Items;
        }

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

        [HttpPost]
        [GroupWart("SampleGroupName")]
        public ActionResult<TestEntity> Post([FromBody] TestEntity entity)
        {
            Items.Add(entity);
            return entity;
        }

        [HttpPatch("{id}")]
        public ActionResult<TestEntity> Patch(int id, [FromBody] TestEntity entity)
        {
            var item = Items.FirstOrDefault(x => x.Id == id);
            if (item == null)
            {
                return NotFound();
            }
            item.Param = entity.Param;
            return item;
        }

        [HttpPut("{id}")]
        public ActionResult<TestEntity> Put(int id, [FromBody] TestEntity entity)
        {
            var item = Items.FirstOrDefault(x => x.Id == id);
            if (item == null)
            {
                return NotFound();
            }
            item.Param = entity.Param;
            return item;
        }

        [HttpDelete("{id}")]
        public ActionResult<TestEntity> Delete(int id)
        {
            var item = Items.FirstOrDefault(x => x.Id == id);
            if (item == null)
            {
                return NotFound();
            }
            Items.Remove(item);
            return item;
        }
    }
}