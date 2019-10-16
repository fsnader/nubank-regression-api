using System;

namespace Rice.NuBank.Domain.Metrics
{
    /// <summary>
    /// Based in NikolayIT linear regression implementation:
    /// https://gist.github.com/NikolayIT/d86118a3a0cb3f5ed63d674a350d75f2
    /// </summary>
    public class LinearRegression
    {
        /// <summary>
        /// The y-intercept value of the line (i.e. y = ax + b, yIntercept is b).
        /// </summary>
        public double Constant { get; private set; }

        /// <summary>
        /// The slop of the line (i.e. y = ax + b, slope is a).
        /// </summary>
        public double Slope { get; private set; }

        /// <summary>
        /// Creates a linear regression model based on x/y values
        /// </summary>
        /// <param name="xValues">The x-axis values.</param>
        /// <param name="yValues">The y-axis values.</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static LinearRegression CreateFromData(
            double[] xValues,
            double[] yValues)
        {
            if (xValues.Length != yValues.Length)
            {
                throw new Exception("Input values should be with the same length.");
            }

            double sumOfX = 0;
            double sumOfY = 0;
            double sumOfXSq = 0;
            double sumOfYSq = 0;
            double sumCodeviates = 0;

            for (var i = 0; i < xValues.Length; i++)
            {
                var x = xValues[i];
                var y = yValues[i];
                sumCodeviates += x * y;
                sumOfX += x;
                sumOfY += y;
                sumOfXSq += x * x;
                sumOfYSq += y * y;
            }

            var count = xValues.Length;
            var ssX = sumOfXSq - ((sumOfX * sumOfX) / count);
            var ssY = sumOfYSq - ((sumOfY * sumOfY) / count);

            var rNumerator = (count * sumCodeviates) - (sumOfX * sumOfY);
            var rDenom = (count * sumOfXSq - (sumOfX * sumOfX)) * (count * sumOfYSq - (sumOfY * sumOfY));
            var sCo = sumCodeviates - ((sumOfX * sumOfY) / count);

            var meanX = sumOfX / count;
            var meanY = sumOfY / count;
            var dblR = rNumerator / Math.Sqrt(rDenom);

            return new LinearRegression
            {
                Constant = meanY - ((sCo / ssX) * meanX),
                Slope = sCo / ssX
            };
        }
        
        private LinearRegression()
        {
        }

        /// <summary>
        /// Estimates a value based on model
        /// </summary>
        /// <param name="xValue"></param>
        /// <returns></returns>
        public double EstimateValue(double xValue)
        {
            return (Slope * xValue) + Constant;
        }

        public override string ToString()
        {
            return $"{Slope}*x + {Constant}";
        }
    }
}