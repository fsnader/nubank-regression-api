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
        
        public DailySummary[] EstimatedDays { get; set; }
        
        public string LinearModel { get; set; }
        
        public decimal EndOfDayEstimatedValue { get; set; }

        public MonthlySummary(int month, IEnumerable<Event> events)
        {
            Month = month;
            Days = events
                .Where(e => e.Time.Month == Month)
                .GroupBy(e => e.Time.Day)
                .Select(g => new DailySummary(g.Key, g))
                .ToArray();

            TotalAmount = Days.Sum(d => d.Total);

            var xValues = Days.Select(a => (double) a.Day).ToArray();
            var yValues = Days.Select(a => (double) a.Total).ToArray();

            var linearModel = LinearRegression.CreateFromData(xValues, yValues);
            
            LinearModel = linearModel.ToString();
            EndOfDayEstimatedValue = (decimal) linearModel.EstimateValue(31);
            EstimatedDays = Days.Select(d => new DailySummary
            {
                Day = d.Day,
                Total = (decimal) linearModel.EstimateValue(d.Day)
            }).ToArray();
        }
    }
}