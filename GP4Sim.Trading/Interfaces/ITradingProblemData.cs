using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GP4Sim.SimulationFramework.Interfaces;
using GP4Sim.Trading.Parameters;

namespace GP4Sim.Trading.Interfaces
{
    public interface ITradingProblemData : ISimulationProblemData
    {

        TradingSimulationParameters SimulationParameters { get; }
        string PriceVariable { get; }
        string TimePointVariable { get; }
        bool InvertPrices { get; }
        double TrainingMaxReturn { get; }
        double TestMaxReturn { get; }

        bool MonteCarlo { get; }
        List<ITradingProblemData> MonteCarloSets(int seed, bool regenerate = false);
    }
}
