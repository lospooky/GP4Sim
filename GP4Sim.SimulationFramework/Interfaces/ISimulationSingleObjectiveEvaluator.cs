using HeuristicLab.Problems.DataAnalysis.Symbolic;

namespace GP4Sim.SimulationFramework.Interfaces
{
    public interface ISimulationSingleObjectiveEvaluator<T> : ISimulationEvaluator<T>, ISymbolicDataAnalysisSingleObjectiveEvaluator<T>
        where T : class, ISimulationProblemData
    {
        //double AnalyzeSolution(AgentFunction agent, double lowerEstimationLimit, double upperEstimationLimit, T problemData, IEnumerable<int> rows);
    }
}
