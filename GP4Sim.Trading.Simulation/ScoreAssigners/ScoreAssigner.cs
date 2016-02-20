using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GP4Sim.Trading.Simulation
{
    public static class ScoreAssigner
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

            //Beat BH, Nothing Else
            for (int i = 0; i < navReturns.Count; i++)
            {
                if (navReturns[i] > 0 && navReturns[i] > bhReturns[i])
                    score += 1;
            }

            double maxScore = navReturns.Count;

            //double finalReturn = finalNAV / startingNAV;

            //return (finalReturn/maxReturn) * 100;

            double finalScore = (score / maxScore) * 100;
            if (double.IsInfinity(finalScore) || double.IsNaN(finalScore))
                finalScore = -100;

            return finalScore;
        }
    }
}
