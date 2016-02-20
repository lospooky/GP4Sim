using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Core;
using HeuristicLab.Common;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Problems.DataAnalysis.Symbolic;
using GP4Sim.SimulationFramework.Analyzers;
using GP4Sim.Trading.Interfaces;
using GP4Sim.Trading.Solutions;
using GP4Sim.SymbolicTrees;

namespace HeuristicLab.GP4SimPlugins.Simulation.Inheritance_Tests.Tentative.Analyzers
{
    //NOT TO BE REGISTERED
    //[Item("ConcreteSingleObjectiveValidationBestSolutionAnalyzer", "An operator that analyzes the validation best concrete solution for single objective concrete problems.")]
    [StorableClass]
    public sealed class TradingSingleObjectiveValidationBestSolutionAnalyzer : SimulationSingleObjectiveValidationBestSolutionAnalyzer<ITradingProblemData, ITradingSingleObjectiveEvaluator, ITradingModel, ITradingSolution>
    {
        #region Constructors
        [StorableConstructor]
        public TradingSingleObjectiveValidationBestSolutionAnalyzer(bool deserializing) : base(deserializing) { }
        public TradingSingleObjectiveValidationBestSolutionAnalyzer(TradingSingleObjectiveValidationBestSolutionAnalyzer original, Cloner cloner) : base(original, cloner) { }
        public TradingSingleObjectiveValidationBestSolutionAnalyzer()
            : base()
        {

        }
        public override IDeepCloneable Clone(Cloner cloner)
        {
            return new TradingSingleObjectiveValidationBestSolutionAnalyzer(this, cloner);
        }
        #endregion

        protected override ITradingSolution CreateSolution(ISymbolicExpressionTree bestTree, double bestQuality)
        {

            ITradingModel model = new TradingModel(bestTree, SymbolicDataAnalysisTreeInterpreterParameter.ActualValue as SymbolicAbstractTreeInterpreter, SymbolicExpressionGrammarParameter.ActualValue, EvaluatorParameter.ActualValue, EstimationLimitsParameter.ActualValue.Lower, EstimationLimitsParameter.ActualValue.Upper);
            return new TradingSolution(model, ProblemDataParameter.ActualValue);
        }
    }
}
