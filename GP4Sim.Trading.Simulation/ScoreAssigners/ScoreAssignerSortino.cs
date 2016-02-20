using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GP4Sim.Trading.Simulation
{
    public static class ScoreAssignerSortino
    {
        public static double AssignScore(List<double> eodNavSeries, List<double> eodInstrPriceSeries, bool inverted, double maxReturn, double startingNAV, double finalNAV, bool annualized)
        {
            List<double> navReturns = new List<double>();
            List<double> bhReturns = new List<double>();

            int annualizationCoefficient = 270;


            if (inverted)
                eodInstrPriceSeries = eodInstrPriceSeries.Select(x => 1 / x).ToList();

            for (int i = 1; i < eodNavSeries.Count; i++)
            {
                navReturns.Add((eodNavSeries[i] / eodNavSeries[i - 1]) - 1);
                bhReturns.Add((eodInstrPriceSeries[i] / eodInstrPriceSeries[i - 1]) - 1);
            }

            if (navReturns.Count < 3)
                return -100;

            //Sortino Calc
            List<double> negativeReturns = navReturns.Where(x => x < 0).ToList();
            double avgReturn = navReturns.Average();
            double sumsq = negativeReturns.Select(x => Math.Pow(x, 2)).Sum();

            long nDayPoints = navReturns.Count();
            double dailySortino = avgReturn / Math.Sqrt((sumsq / nDayPoints));
            double annualizedSortino = dailySortino * Math.Sqrt(annualizationCoefficient);

            double finalscore;

            if (annualized)
                finalscore = annualizedSortino;
            else
                finalscore = dailySortino;

            if (double.IsInfinity(finalscore) || double.IsNaN(finalscore))
                finalscore = -100;

            return finalscore;
            
        }
    }
}
