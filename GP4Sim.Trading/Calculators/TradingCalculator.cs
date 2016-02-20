using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GP4Sim.CSharpAgents;
using GP4Sim.SimulationFramework.Interfaces;
using GP4Sim.Trading.Interfaces;
using GP4Sim.Trading.Simulation;


namespace HeuristicLab.GP4SimPlugins.Simulation.Inheritance_Tests.Concrete.Calculators
{
    public class TradingCalculator : ISimulationCalculator<ITradingProblemData, ITradingEnvelope>
    {

        public static double FitnessScore(AgentFunction agent, ITradingProblemData problem, IEnumerable<int> rows)
        {
            throw new NotImplementedException();
        }

        public static ITradingEnvelope AnalysisResults(AgentFunction agent, ITradingProblemData problem, IEnumerable<int> rows)
        {
            throw new NotImplementedException();
        }
    }
}
