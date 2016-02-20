using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GP4Sim.Trading.Simulation
{
    public class ScoreAssignerRelative
    {
        public static double AssignScore(List<double> eodNavSeries, List<double> eodInstrPriceSeries, bool inverted, double maxReturn, double startingNAV, double finalNAV)
        {


            double finalReturn = (finalNAV / startingNAV) - 1;

            return (finalReturn / (maxReturn - 1)) * 100;
        }
    }
}
