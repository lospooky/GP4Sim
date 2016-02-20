using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GP4Sim.Trading.Simulation
{
    public enum TradingFitnessType
    {
        NAV = 0, Score = 1, ScorePenalties = 2, ScoreWeighted = 3, Relative = 4, ActivityAware = 5, Sortino=6, AnnualizedSortino=7
    }
}
