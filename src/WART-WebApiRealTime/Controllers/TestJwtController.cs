// (c) 2019 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using System.Collections.Generic;
using System.Linq;
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
    /// A simple controller example extended by the WartController with JWT authentication.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class TestJwtController : WartControllerJwt
    {
        private static List<TestEntity> Items = new List<TestEntity>
            {
                new TestEntity { Id = 1, Param = "Item1" },
                new TestEntity { Id = 2, Param = "Item2" },
                new TestEntity { Id = 3, Param = "Item3" }
            };

        public TestJwtController(IHubContext<WartHubJwt> messageHubContext, ILogger<TestJwtController> logger) : base(messageHubContext, logger)
        {
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
