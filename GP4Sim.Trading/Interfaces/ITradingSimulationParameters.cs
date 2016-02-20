using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GP4Sim.SimulationFramework.Interfaces;
using GP4Sim.Trading.Simulation;
using HeuristicLab.Problems.DataAnalysis;

namespace GP4Sim.Trading.Interfaces
{
    public interface ITradingSimulationParameters : ISimulationParameters
    {
        double StartingNAV { get; }
        int MinOrderSize { get; }
        double Slack { get; }
        DoubleLimit ExposureLimits { get; }
        int MinTrades { get; }
        CommissionFunctionValue CommissionFcn { get; }
    }
}
