using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GP4Sim.SimulationFramework.Analyzers;
using GP4Sim.SymbolicTrees;
using GP4Sim.Trading.Interfaces;
using GP4Sim.Trading.Solutions;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace GP4Sim.Trading.Analyzers
{
    [Item("ConcreteSingleObjectiveValidationBestNSolutionsAnalyzer", "An operator that analyzes the validation best concrete solution for single objective concrete problems.")]
    [StorableClass]
    public sealed class TradingSingleObjectiveValidationNBestSolutionsAnalyzer : SimulationSingleObjectiveValidationNBestSolutionsAnalyzer<ITradingProblemData, ITradingSingleObjectiveEvaluator, ITradingModel, ITradingSolution>
    {
        private static string solName = "Validation Score: ";

        private const string SeedParameterName = "Seed";

        public IntValue SeedParameter
        {
            get { return (IntValue)Parameters[SeedParameterName].ActualValue; }
        }

        public int Seed
        {
            get { return SeedParameter.Value; }
        }

        #region Constructors
        [StorableConstructor]
        public TradingSingleObjectiveValidationNBestSolutionsAnalyzer(bool deserializing) : base(deserializing) { }
        public TradingSingleObjectiveValidationNBestSolutionsAnalyzer(TradingSingleObjectiveValidationNBestSolutionsAnalyzer original, Cloner cloner) : base(original, cloner) { }
        public TradingSingleObjectiveValidationNBestSolutionsAnalyzer()
            : base()
        {
            Parameters.Add(new LookupParameter<IntValue>(SeedParameterName));
        }
        public override IDeepCloneable Clone(Cloner cloner)
        {
            return new TradingSingleObjectiveValidationNBestSolutionsAnalyzer(this, cloner);
        }

        [StorableHook(HookType.AfterDeserialization)]
        private new void AfterDeserialization()
        {
            if (!Parameters.ContainsKey(SeedParameterName))
                Parameters.Add(new LookupParameter<IntValue>(SeedParameterName));

            base.AfterDeserialization();
        }
        #endregion

        protected override ITradingSolution CreateSolution(ISymbolicExpressionTree bestTree, double bestQuality)
        {

            ITradingModel model = new TradingModel(bestTree, SymbolicDataAnalysisTreeInterpreterParameter.ActualValue as SymbolicAbstractTreeInterpreter, SymbolicExpressionGrammarParameter.ActualValue, EvaluatorParameter.ActualValue, EstimationLimitsParameter.ActualValue.Lower, EstimationLimitsParameter.ActualValue.Upper);
            TradingSolution sol = new TradingSolution(model, ProblemDataParameter.ActualValue);
            if (ProblemDataParameter.ActualValue.MonteCarlo)
                DoMonteCarlo(sol);
            sol.Name = solName + bestQuality.ToString("F5");
            return sol;
        }

        private void DoMonteCarlo(TradingSolution sol)
        {
            sol.PerformMonteCarloEvaluation(ProblemDataParameter.ActualValue.MonteCarloSets(SeedParameter.Value));
        }
    }
}
