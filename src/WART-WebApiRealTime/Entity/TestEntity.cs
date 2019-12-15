// (c) 2019 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using System;

namespace WART_Api.Entity
{
    [Serializable]
    public class TestEntity
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public string Param { get; set; }
    }
}
