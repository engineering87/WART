// (c) 2024 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using Microsoft.AspNetCore.Mvc.Filters;
using System.Collections.Generic;

namespace WART_Core.Entity
{
    /// <summary>
    /// Represents an event that contains additional filter metadata.
    /// </summary>
    public class WartEventWithFilters
    {
        /// <summary>
        /// The main WartEvent object.
        /// </summary>
        public WartEvent WartEvent { get; set; }

        /// <summary>
        /// A list of filters applied to the event.
        /// </summary>
        public List<IFilterMetadata> Filters { get; set; }

        /// <summary>
        /// Initializes a new instance of the WartEventWithFilters class.
        /// </summary>
        /// <param name="wartEvent">The WartEvent to associate with the filters.</param>
        /// <param name="filters">The list of filters applied to the event.</param>
        public WartEventWithFilters(WartEvent wartEvent, List<IFilterMetadata> filters)
        {
            // Initialize the WartEvent and Filters properties
            WartEvent = wartEvent;
            Filters = filters;
        }
    }
}