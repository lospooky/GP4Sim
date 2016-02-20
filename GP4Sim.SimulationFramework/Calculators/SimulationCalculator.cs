using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GP4Sim.CSharpAgents;
using GP4Sim.SimulationFramework.Interfaces;

namespace GP4Sim.SimulationFramework.Calculators
{
    public abstract class SimulationCalculator<T, U>
        where T : class, ISimulationEnvelope
        where U : class, ISimulationProblemData
    {
        protected abstract double FitnessScore();

        protected abstract T AnalysisResults(AgentFunction agent, U problemData, IEnumerable<int> rows);
    }
}
