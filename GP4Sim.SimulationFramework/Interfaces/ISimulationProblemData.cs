using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GP4Sim.Data;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Problems.DataAnalysis;

namespace GP4Sim.SimulationFramework.Interfaces
{
    public interface ISimulationProblemData : IDataAnalysisProblemData
    {
        ICheckedItemList<StringValue> InputStates { get; }
        IEnumerable<string> AllowedInputStates { get; }
        DataCache DataCache { get; }
    }
}
