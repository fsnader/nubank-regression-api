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

        public decimal EndOfMonthEstimatedValue => (decimal) _linearModel.EstimateValue(31);

        public MonthlySummary(int month, IEnumerable<Event> events)
        {
            Month = month;
            Days = events
                .Where(e => e.Time.Month == Month &&
                            // TODO: Ver como pegar parcelas e jogar para dia 0
                            (e.Category == EventCategory.Transaction || e.Category == EventCategory.Unknown))
                .GroupBy(e => e.Time.Day)
                .Select(g => new DailySummary(g.Key, g))
                .OrderBy(e => e.Day)
                .ToArray();

            var yValues = new List<double> {0};
            var xValues = new List<double> {0};
            foreach (var day in Days)
            {
                xValues.Add((double) day.Day);
                yValues.Add((double) day.Total + yValues.LastOrDefault());
            }

            _linearModel = LinearRegression.CreateFromData(xValues.ToArray(), yValues.ToArray());
        }
    }
}