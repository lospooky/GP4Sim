using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.Problems.DataAnalysis.Symbolic;

namespace GP4Sim.SimulationFramework.Interfaces
{
    public interface ISimulationEvaluator<T> : ISymbolicDataAnalysisEvaluator<T>
        where T : class, ISimulationProblemData
    {

    }
}
