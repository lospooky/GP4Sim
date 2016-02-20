using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Common;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Problems.DataAnalysis.Symbolic;
using GP4Sim.SimulationFramework.Analyzers;
using GP4Sim.Trading.Interfaces;
using GP4Sim.Trading.Solutions;
using GP4Sim.SymbolicTrees;

namespace GP4Sim.Trading.Analyzers
{
    //NOT TO BE REGISTERED
    //[Item("ConcreteSingleObjectiveTrainingBestSolutionAnalyzer", "An operator that analyzes the training best concrete solution for single objective concrete problems.")]
    [StorableClass]
    public sealed class TradingSingleObjectiveTrainingBestSolutionAnalyzer :
        SimulationSingleObjectiveTrainingBestSolutionAnalyzer<ITradingProblemData, ITradingSingleObjectiveEvaluator, ITradingModel, ITradingSolution>
    {
        #region Constructors
        [StorableConstructor]
        public TradingSingleObjectiveTrainingBestSolutionAnalyzer(bool deserializing) : base(deserializing) { }
        public TradingSingleObjectiveTrainingBestSolutionAnalyzer(TradingSingleObjectiveTrainingBestSolutionAnalyzer original, Cloner cloner) : base(original, cloner) { }
        public TradingSingleObjectiveTrainingBestSolutionAnalyzer()
            : base()
        {

        }
        public override IDeepCloneable Clone(Cloner cloner)
        {
            return new TradingSingleObjectiveTrainingBestSolutionAnalyzer(this, cloner);
        }
        #endregion

        protected override ITradingSolution CreateSolution(ISymbolicExpressionTree tree, double bestQuality)
        {

            ITradingModel model = new TradingModel(tree, SymbolicDataAnalysisTreeInterpreterParameter.ActualValue as SymbolicAbstractTreeInterpreter, SymbolicExpressionGrammarParameter.ActualValue, EvaluatorParameter.ActualValue, EstimationLimitsParameter.ActualValue.Lower, EstimationLimitsParameter.ActualValue.Upper);
            return new TradingSolution(model, ProblemDataParameter.ActualValue);
        }
    }
}
