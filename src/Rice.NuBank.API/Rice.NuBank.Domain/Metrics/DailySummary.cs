using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NubankClient.Model.Events;

namespace Rice.NuBank.Domain.Metrics
{
    public class DailySummary
    {
        public int Day { get; set; }
        public Decimal Total { get; set; }
        public Event[] Events { get; set; }

        public DailySummary(int day, IEnumerable<Event> events)
        {
            Day = day;
            Events = events.Where(e => e.Time.Day == day).ToArray();
            Total = Events.Sum(e => e.CurrencyAmount);
        }
    }
}