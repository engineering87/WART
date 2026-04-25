// (c) 2025 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using Microsoft.AspNetCore.Mvc.Filters;
using WART_Core.Entity;
using WART_Core.Filters;

namespace WART_Tests.Entity
{
    public class WartEventWithFiltersTests
    {
        [Fact]
        public void Constructor_SetsProperties()
        {
            var evt = new WartEvent("POST", "/api/items", "10.0.0.1");
            var filters = new List<IFilterMetadata> { new GroupWartAttribute("g1") };

            var item = new WartEventWithFilters(evt, filters);

            Assert.Same(evt, item.WartEvent);
            Assert.Same(filters, item.Filters);
            Assert.Equal(0, item.RetryCount);
        }

        [Fact]
        public void Constructor_NullWartEvent_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new WartEventWithFilters(null!, []));
        }

        [Fact]
        public void Constructor_NullFilters_DoesNotThrow()
        {
            var evt = new WartEvent("GET", "/", "::1");

            var item = new WartEventWithFilters(evt, null!);

            Assert.Null(item.Filters);
        }

        [Fact]
        public void RetryCount_DefaultsToZero()
        {
            var item = new WartEventWithFilters(new WartEvent("GET", "/", "::1"), []);

            Assert.Equal(0, item.RetryCount);
        }

        [Fact]
        public void RetryCount_CanBeIncremented()
        {
            var item = new WartEventWithFilters(new WartEvent("GET", "/", "::1"), []);

            item.RetryCount++;
            item.RetryCount++;

            Assert.Equal(2, item.RetryCount);
        }
    }
}
