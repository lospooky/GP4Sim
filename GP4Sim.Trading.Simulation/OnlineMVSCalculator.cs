using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.Problems.DataAnalysis;

namespace GP4Sim.Trading.Simulation
{
    public class OnlineMVSCalculator : OnlineMeanAndVarianceCalculator
    {

        public OnlineMVSCalculator()
            : base()
        {

        }

        public double StdDev
        {
            get
            {
                return Math.Sqrt(Variance);
            }
        }

        public double RelStdDev
        {
            get
            {
                return StdDev / Mean;
            }
        }

        public double PercRelStdDev
        {
            get
            {
                return RelStdDev * 100;
            }
        }
    }
}
