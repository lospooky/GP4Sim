using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.Data;

namespace GP4Sim.Data
{
    public static partial class Utilities
    {
        public static string ToStringRange(this IEnumerable<int> en)
        {
            return en.First() + "-" + en.Last();
        }

        public static IntRange2 ToIntRange(this IEnumerable<int> en)
        {
            return new IntRange2(en.First(), en.Last());
        }
    }
}
