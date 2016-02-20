using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GP4Sim.Trading.Simulation
{
    public static class ScoreAssignerWeighted
    {
        public static double AssignScore(List<double> eodNavSeries, List<double> eodInstrPriceSeries, bool inverted, double maxReturn, double startingNAV, double finalNAV)
        {
            List<double> navReturns = new List<double>();
            List<double> bhReturns = new List<double>();

            if (eodNavSeries.Count() < 3)
                return -200;

            if (inverted)
                eodInstrPriceSeries = eodInstrPriceSeries.Select(x => 1 / x).ToList();

            for (int i = 1; i < eodNavSeries.Count; i++)
            {
                navReturns.Add((eodNavSeries[i] / eodNavSeries[i - 1]) - 1);
                bhReturns.Add((eodInstrPriceSeries[i] / eodInstrPriceSeries[i - 1]) - 1);
            }

            double score = 0;

            //Beat BH, Else Penalties
            for (int i = 0; i < navReturns.Count; i++)
            {
                if (navReturns[i] > 0)
                {
                    if (bhReturns[i] > 0 && navReturns[i] > bhReturns[i])
                        score += 5;
                    else if (navReturns[i] <= bhReturns[i]) //nav > 0, bh > 0, nav < bh
                        score += 1;
                    else //nav > 0, bh < 0
                        score += 2;
                }
                else if (navReturns[i] <= 0)
                {
                    if (navReturns[i] == 0)
                        score -= 1;
                    else if (bhReturns[i] > 0)
                        score -= 10;
                    else if (navReturns[i] < bhReturns[i])
                        score -= 5;
                    else //nav < 0, bh < 0, nav > bh
                        score -= 3;
                }
            }

            double maxScore = navReturns.Count * 5;

            //double finalReturn = finalNAV / startingNAV;

            //return (finalReturn/maxReturn) * 100;

            double finalScore = (score / maxScore) * 100;
            if(double.IsInfinity(finalScore) || double.IsNaN(finalScore))
                finalScore = -200;

            return finalScore;
        }
    }
}
