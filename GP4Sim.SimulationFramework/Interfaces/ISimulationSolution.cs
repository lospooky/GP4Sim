using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.Problems.DataAnalysis;
using HeuristicLab.Problems.DataAnalysis.Symbolic;

namespace GP4Sim.SimulationFramework.Interfaces
{
    public interface ISimulationSolution<T, U> : IDataAnalysisSolution, ISymbolicDataAnalysisSolution
        where T : class, ISimulationProblemData
        where U : class, ISimulationModel<T>
    {

        new U Model { get; }
        new T ProblemData { get; set; }

        List<string> FullInputVector { get; }
        List<string> ActualInputVector { get; }
        //IEnumerable<double> RawOutputValues { get; }
        //IEnumerable<double> RawOutputTrainingValues { get; }
        //IEnumerable<double> RawOutputTestValues { get; }
        //IEnumerable<double> GetRawOutputValues(IEnumerable<int> rows);
    }
}
