using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HeuristicLab.Random;

namespace GP4Sim.Trading.Instances
{
    public static class ValueGenerator
    {
        private static FastRandom rand = new FastRandom();

        public static IEnumerable<double> GenerateUniformDistributedValues(int n, double start, double end)
        {
            for (int i = 0; i < n; i++)
            {
                // we need to return a random value including end.
                // so we cannot use rand.NextDouble() as it returns a value strictly smaller than 1.
                double r = rand.NextUInt() / (double)uint.MaxValue;    // r \in [0,1]
                yield return r * (end - start) + start;
            }
        }
    }
}
