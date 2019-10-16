using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NubankClient.Model.Events;

namespace Rice.NuBank.Domain.Metrics
{
    public class MonthlySummary
    {
        private readonly LinearRegression _linearModel;

        public int Month { get; set; }

        public DailySummary[] Days { get; set; }

        public DailySummary[] EstimatedDays =>
            Days.Select(d => new DailySummary
            {
                Day = d.Day,
                Total = (decimal) _linearModel.EstimateValue(d.Day)
            }).ToArray();

        public string LinearModel => _linearModel.ToString();
        
        public Decimal TotalAmount => Days.Sum(d => d.Total);

        public decimal EndOfDayEstimatedValue => (decimal) _linearModel.EstimateValue(31);

        public MonthlySummary(int month, IEnumerable<Event> events)
        {
            Month = month;
            Days = events
                .Where(e => e.Time.Month == Month)
                .GroupBy(e => e.Time.Day)
                .Select(g => new DailySummary(g.Key, g))
                .ToArray();

            _linearModel = LinearRegression.CreateFromData(
                Days.Select(a => (double) a.Day).ToArray(),
                Days.Select(a => (double) a.Total).ToArray());
        }
    }
}