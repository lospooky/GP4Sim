using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GP4Sim.SimulationFramework.Interfaces;
using HeuristicLab.Problems.Instances.DataAnalysis;

namespace GP4Sim.SimulationFramework.Instances
{
    public abstract class SimulationInstanceProvider<T, U> : DataAnalysisInstanceProvider<T, U>
        where T : class, ISimulationProblemData
        where U : SimulationImportType
    {

    }
}
