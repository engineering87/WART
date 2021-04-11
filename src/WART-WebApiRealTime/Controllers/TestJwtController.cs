using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using WART_Api.Entity;
using WART_Core.Controllers;
using WART_Core.Hubs;

namespace WART_Api.Controllers
{
    /// <summary>
    /// A simple controller example extended by the WartController with JWT authentication.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class TestJwtController : WartControllerJwt
    {
        public TestJwtController(IHubContext<WartHubJwt> messageHubContext, ILogger<TestJwtController> logger) : base(messageHubContext, logger)
        {
        }

        [HttpGet]
        public IEnumerable<TestEntity> Get()
        {
            return Enumerable.Range(1, 2).Select(index => new TestEntity
            {
                Id = Guid.NewGuid(),
                Date = DateTime.Now.AddDays(index),
                Param = string.Empty
            }).ToArray();
        }

        [HttpGet("{id}", Name = "GetAuth")]
        public IEnumerable<TestEntity> Get(string id)
        {
            return Enumerable.Range(1, 2).Select(index => new TestEntity
            {
                Id = Guid.NewGuid(),
                Date = DateTime.Now.AddDays(index),
                Param = id
            }).ToArray();
        }

        [HttpPost]
        public TestEntity Post([FromBody] TestEntity entity)
        {
            if (entity != null)
            {
                entity.Id = Guid.NewGuid();
            }
            return entity;
        }
    }
}
