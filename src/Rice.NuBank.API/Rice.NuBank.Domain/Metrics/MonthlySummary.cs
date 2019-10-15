using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NubankClient.Model.Events;

namespace Rice.NuBank.Domain.Metrics
{
    public class MonthlySummary
    {
        public int Month { get; set; }
        public Decimal TotalAmount { get; set; }
        public DailySummary[] Days { get; set; }

        public MonthlySummary(int month, IEnumerable<Event> events)
        {
            Month = month;
            Days = events
                .Where(e => e.Time.Month == Month)
                .GroupBy(e => e.Time.Day)
                .Select(g => new DailySummary(g.Key, g))
                .ToArray();

            TotalAmount = Days.Sum(d => d.Total);
        }
    }
}