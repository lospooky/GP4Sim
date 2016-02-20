using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GP4Sim.SimulationFramework.Interfaces;

namespace GP4Sim.Trading.Interfaces
{
    public interface ITradingSolution : ISimulationSolution<ITradingProblemData, ITradingModel>
    //public interface IConcreteSolutionV2 : IAbstractSolutionV2<IConcreteProblemData, IAbstractModelV2<IConcreteProblemData>>
    {
        new ITradingModel Model { get; }
        new ITradingProblemData ProblemData { get; set; }
        string DescriptiveName { get; }
        void PerformMonteCarloEvaluation(List<ITradingProblemData> mcSets);
    }
}
