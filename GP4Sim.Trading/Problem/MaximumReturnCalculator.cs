using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GP4Sim.Trading.Problem
{
    public static class MaximumReturnCalculator
    {
        public static double MaximumReturn(IEnumerable<double> priceSeries)
        {
            if (priceSeries.Count() > 3)
            {
                double[] prices = priceSeries.ToArray();
                double entryPrice = prices[0];
                bool trendDirection = true;
                double totalReturn = 1.0;
                double partialReturn;

                trendDirection = DirectionalChange(prices[1], entryPrice, trendDirection);

                for (int i = 2; i < prices.Count(); i++)
                {
                    bool curDir = DirectionalChange(prices[i], prices[i - 1], trendDirection);

                    if (trendDirection ^ curDir)
                    {
                        partialReturn = Math.Abs((prices[i - 1] / entryPrice) - 1) + 1;
                        totalReturn = totalReturn * partialReturn;

                        entryPrice = prices[i - 1];
                        trendDirection = curDir;
                    }

                    if (i == priceSeries.Count() - 1)
                    {
                        partialReturn = Math.Abs((prices[i - 1] / entryPrice) - 1) + 1;
                        totalReturn = totalReturn * partialReturn;
                    }
                }

                return Math.Round(totalReturn, 3, MidpointRounding.AwayFromZero);
            }
            else
                return 0.0;
        }


        private static bool DirectionalChange(double curPrice, double entryPrice, bool trendDirection)
        {
            if (curPrice - entryPrice > 0)
                return true;
            else if (curPrice - entryPrice < 0)
                return false;
            else
                return trendDirection;
        }
    }


}
