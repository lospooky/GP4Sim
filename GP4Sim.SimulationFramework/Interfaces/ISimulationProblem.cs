using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.Problems.DataAnalysis;

namespace GP4Sim.SimulationFramework.Interfaces
{
    public interface ISimulationProblem<T> : IDataAnalysisProblem<T>
        where T : class, ISimulationProblemData
    {

    }
}
