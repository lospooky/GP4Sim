using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GP4Sim.SimulationFramework.Solutions;
using HeuristicLab.Problems.DataAnalysis;
using HeuristicLab.Problems.DataAnalysis.Symbolic;

namespace GP4Sim.SimulationFramework.Interfaces
{
    public interface ISimulationModel<T> : IDataAnalysisModel, ISymbolicDataAnalysisModel
        where T : class, ISimulationProblemData
    {
        IEnumerable<double> GetRawOutputValues(T problemData, IEnumerable<int> rows);
        double GetFitnessScore(T problemData, IEnumerable<int> rows);
        //CompiledSymbolicExpressionTree CompiledTree { get; }
        ResultPropertiesDictionary ResultProperties { get; }
        dynamic GetResult(T problemData, string pName, IEnumerable<int> rows);
        //string GetSimulationLog(T problemData, IEnumerable<int> rows);



    }
}
