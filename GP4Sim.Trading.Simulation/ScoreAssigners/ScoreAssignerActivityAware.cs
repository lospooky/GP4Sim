using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GP4Sim.Trading.Simulation
{
    public static class ScoreAssignerActivityAware
    {
        public static double AssignScore(List<double> eodNavSeries, List<double> eodInstrPriceSeries, List<int> eodTradingActivity, bool inverted, double maxReturn, double startingNAV, double finalNAV, double rmdd)
        {
            List<double> navReturns = new List<double>();
            List<double> bhReturns = new List<double>();
            List<int> dailyTrades = new List<int>();

            int activitythreshold = 3;
            int nDays = 7;

            if (eodTradingActivity.Count() < 7)
                return SignedExponential(-3);

            /*
            if (inverted)
                eodInstrPriceSeries = eodInstrPriceSeries.Select(x => 1 / x).ToList();

            for (int i = 1; i < eodNavSeries.Count; i++)
            {
                navReturns.Add((eodNavSeries[i] / eodNavSeries[i - 1]) - 1);
                bhReturns.Add((eodInstrPriceSeries[i] / eodInstrPriceSeries[i - 1]) - 1);
               
            }

             */
            for (int i = 1; i < eodTradingActivity.Count; i++)
                dailyTrades.Add(eodTradingActivity[i] - eodTradingActivity[i - 1]);
            

            List<int> slidingweeklytrades = new List<int>();

            for (int i = 0; i < dailyTrades.Count - 6; i++)
                slidingweeklytrades.Add(dailyTrades.GetRange(i, 7).Sum());

            double wta = slidingweeklytrades.Average();

            double finalReturn = (finalNAV / startingNAV) - 1;
            double relativeReturn =  (finalReturn / (maxReturn - 1)) * 100;

            double score = CalculateScore(relativeReturn, wta, activitythreshold);

            return score;
        }

        private static double CalculateScore(double relativeReturn, double wta, int t)
        {
            if (wta < 1)
                return SignedExponential(-1);
            else
                return SignedExponential(relativeReturn) * FrequencyBonus(wta, t);
        }

        private static double SignedExponential(double x)
        {
            if (x > 0)
                return Math.Exp(x) - 1;
            else if (x < 0)
                return -Math.Exp(-x) + 1;
            else
                return 0;
        }

        private static double FrequencyBonus(double f, int t)
        {
            if (f >= t)
                return 1;
            else
                return f / t;
        }

    }
}
