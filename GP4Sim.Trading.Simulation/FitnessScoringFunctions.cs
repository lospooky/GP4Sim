using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GP4Sim.Trading.Simulation
{
    public enum FitnessScoringFunctionsEnum
    {
        NEGEXP = 0, ARCTAN = 1
    }

    public delegate double FitnessScoringFunction(double n);

    public static class FitnessScoring
    {
        public static double NEGEXP(double n)
        {
            return Math.Exp(-n);
        }

        public static double ARCTAN(double n)
        {
            return Math.Atan(-n) + Math.PI / 2;
        }
    }
}
