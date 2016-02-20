using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.DataAnalysis.Symbolic;
using HeuristicLab.Common;
using HeuristicLab.GP4SimPlugins.Simulation.Inheritance_Tests.Concrete.Calculators;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using GP4Sim.SimulationFramework.Evaluators;
using GP4Sim.Trading.Interfaces;
using GP4Sim.CSharpAgents;
using GP4Sim.SymbolicTrees;
using GP4Sim.Trading.Simulation;

namespace GP4Sim.Trading.Evaluators
{
    [StorableClass]
    //public abstract class ConcreteSingleObjectiveEvaluator : SymbolicDataAnalysisSingleObjectiveEvaluator<IConcreteProblemData>, IConcreteSingleObjectiveEvaluator
    public abstract class TradingSingleObjectiveEvaluator : SimulationSingleObjectiveEvaluator<ITradingProblemData, ITradingEnvelope>, ITradingSingleObjectiveEvaluator
    {
        protected TradingFitnessType evalType;

        [StorableConstructor]
        protected TradingSingleObjectiveEvaluator(bool deserializing) : base(deserializing) { }
        protected TradingSingleObjectiveEvaluator(TradingSingleObjectiveEvaluator original, Cloner cloner) : base(original, cloner) { }
        protected TradingSingleObjectiveEvaluator(TradingFitnessType type) : base()
        {
            evalType = type;
        }

        protected override double CalculateFitness(AgentFunction agent, double lowerEstimationLimit, double upperEstimationLimit, ITradingProblemData problemData, IEnumerable<int> rows)
        {
            double maxReturn = 0;

            if (rows.First() == problemData.TrainingPartition.Start)
                maxReturn = problemData.TrainingMaxReturn;
            else if (rows.First() == problemData.TestPartition.Start)
                maxReturn = problemData.TestMaxReturn;

            SimulationParameters_Internal simParameters = new SimulationParameters_Internal(problemData.SimulationParameters.MinOrderSize, problemData.SimulationParameters.StartingNAV, problemData.SimulationParameters.Slack, problemData.SimulationParameters.MinTrades, problemData.SimulationParameters.ExposureLimits, problemData.SimulationParameters.CommissionFcn.Function);
            TradingSimulationRunner simRunner = new TradingSimulationRunner(agent, problemData, problemData.DataCache, simParameters, problemData.TimePointVariable, problemData.PriceVariable, problemData.AllowedInputVariables.Count(), problemData.AllowedInputStates.Count(), MinLag, problemData.InvertPrices, rows, TradingSimulationRunMode.EVALUATION, evalType, maxReturn);
            simRunner.Run();

            double result = simRunner.FitnessScore;
            simRunner.TearDown();
            simRunner = null;

            return result;
        }

        public ITradingEnvelope Analyze(SymbolicAbstractTreeInterpreter interpreter, ISymbolicExpressionGrammar grammar, ISymbolicExpressionTree tree, ITradingProblemData problemData, IEnumerable<int> rows)
        {
            double maxReturn = 0;
            if (rows.First() == problemData.TrainingPartition.Start)
                maxReturn = problemData.TrainingMaxReturn;
            else if (rows.First() == problemData.TestPartition.Start)
                maxReturn = problemData.TestMaxReturn;

            AgentFunction agent = CompileTree(tree, interpreter, grammar);

            SimulationParameters_Internal simParameters = new SimulationParameters_Internal(problemData.SimulationParameters.MinOrderSize, problemData.SimulationParameters.StartingNAV, problemData.SimulationParameters.Slack, problemData.SimulationParameters.MinTrades, problemData.SimulationParameters.ExposureLimits, problemData.SimulationParameters.CommissionFcn.Function);
            TradingSimulationRunner simRunner = new TradingSimulationRunner(agent, problemData, problemData.DataCache, simParameters, problemData.TimePointVariable, problemData.PriceVariable, problemData.AllowedInputVariables.Count(), problemData.AllowedInputStates.Count(), LagLimit(grammar), problemData.InvertPrices, rows, TradingSimulationRunMode.ANALYSIS, evalType, problemData.TrainingMaxReturn);
            simRunner.Run();

            TradingEnvelope results = (TradingEnvelope)simRunner.Results;

            agent = null;
            simParameters = null;
            simRunner.TearDown();
            simRunner = null;
            return results;
        }

        public MCEnvelope MCAnalyze(SymbolicAbstractTreeInterpreter interpreter, ISymbolicExpressionGrammar grammar, ISymbolicExpressionTree tree, ITradingProblemData problemData)
        {

            AgentFunction agent = CompileTree(tree, interpreter, grammar);
            SimulationParameters_Internal simParameters = new SimulationParameters_Internal(problemData.SimulationParameters.MinOrderSize, problemData.SimulationParameters.StartingNAV, problemData.SimulationParameters.Slack, problemData.SimulationParameters.MinTrades, problemData.SimulationParameters.ExposureLimits, problemData.SimulationParameters.CommissionFcn.Function);
            TradingSimulationRunner simRunner = new TradingSimulationRunner(agent, problemData, problemData.DataCache, simParameters, problemData.TimePointVariable, problemData.PriceVariable, problemData.AllowedInputVariables.Count(), problemData.AllowedInputStates.Count(), LagLimit(grammar), problemData.InvertPrices, problemData.TrainingIndices, TradingSimulationRunMode.MONTECARLO, evalType, problemData.TrainingMaxReturn, false);

            simRunner.Run();
            MCEnvelope results = simRunner.MCResults;

            agent = null;
            simParameters = null;
            simRunner.TearDown();
            simRunner = null;

            return results;
        }

        public string SimulationLog(SymbolicAbstractTreeInterpreter interpreter, ISymbolicExpressionGrammar grammar, ISymbolicExpressionTree tree, ITradingProblemData problemData, IEnumerable<int> rows)
        {
            double maxReturn = 0;
            if (rows.First() == problemData.TrainingPartition.Start)
                maxReturn = problemData.TrainingMaxReturn;
            else if (rows.First() == problemData.TestPartition.Start)
                maxReturn = problemData.TestMaxReturn;

            AgentFunction agent = CompileTree(tree, interpreter, grammar);

            SimulationParameters_Internal simParameters = new SimulationParameters_Internal(problemData.SimulationParameters.MinOrderSize, problemData.SimulationParameters.StartingNAV, problemData.SimulationParameters.Slack, problemData.SimulationParameters.MinTrades, problemData.SimulationParameters.ExposureLimits, problemData.SimulationParameters.CommissionFcn.Function);
            TradingSimulationRunner simRunner = new TradingSimulationRunner(agent, problemData, problemData.DataCache, simParameters, problemData.TimePointVariable, problemData.PriceVariable, problemData.AllowedInputVariables.Count(), problemData.AllowedInputStates.Count(), LagLimit(grammar), problemData.InvertPrices, rows, TradingSimulationRunMode.ANALYSIS, evalType, problemData.TrainingMaxReturn, true);
            simRunner.Run();

            agent = null;
            simParameters = null;
            string logText = String.Copy(simRunner.LogText);
            simRunner.TearDown();
            simRunner = null;

            return logText;
        }

    }
}
