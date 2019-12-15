// (c) 2019 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using WART_Api.Entity;
using WART_Core.Controllers;
using WART_Core.Hubs;

namespace WART_Api.Controllers
{
    /// <summary>
    /// A simple controller example extended by the WartController
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : WartController
    {
        public TestController(IHubContext<WartHub> messageHubContext, ILogger<WartController> logger) : base(messageHubContext, logger)
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
            })
            .ToArray();
        }

        [HttpGet("{id}", Name = "Get")]
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
