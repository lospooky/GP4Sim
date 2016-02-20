using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Common;
using HeuristicLab.Problems.DataAnalysis.Symbolic;
using HeuristicLab.Parameters;
using HeuristicLab.Problems.DataAnalysis;
using HeuristicLab.GP4SimPlugins.Simulation.Inheritance_Tests;
using GP4Sim.Trading.Interfaces;
using GP4Sim.SymbolicTrees;
using GP4Sim.Trading.Evaluators;
using GP4Sim.Trading.Analyzers;

namespace GP4Sim.Trading.Problem
{
    [Item("Symbolic Trading Problem (single objective)", "Represents a single objective symbolic trading problem.")]
    [StorableClass]
    [Creatable("Problems")]
    public class TradingProblem : SimulationProblem<ITradingProblemData, ITradingSingleObjectiveEvaluator, ISymbolicDataAnalysisSolutionCreator>, ITradingProblem
    {


        #region Constructors
        [StorableConstructor]
        protected TradingProblem(bool deserializing) : base(deserializing) { }
        protected TradingProblem(TradingProblem original, Cloner cloner)
            : base(original, cloner)
        {
            RegisterEventHandlers();
        }
        public override IDeepCloneable Clone(Cloner cloner) { return new TradingProblem(this, cloner); }

        protected override void ConfigureGrammarSymbols()
        {
            var grammar = SymbolicExpressionTreeGrammar as TypeCoherentStateExpressionGrammar;
            if (grammar != null) grammar.ConfigureAsDefaultSimulationGrammar();
        }

        public TradingProblem()
            : base(new TradingProblemData(), new TradingSingleObjectiveDummyEvaluator(), new SymbolicDataAnalysisExpressionTreeCreator())
        {
            ApplyLinearScalingParameter.Value.Value = true;
            Maximization.Value = false;
            MaximumSymbolicExpressionTreeDepth.Value = InitialMaximumTreeDepth;
            MaximumSymbolicExpressionTreeLength.Value = InitialMaximumTreeLength;

            RegisterEventHandlers();
            ConfigureGrammarSymbols();
            InitializeOperators();
        }
        #endregion

        [StorableHook(HookType.AfterDeserialization)]
        private void AfterDeserialization()
        {
            RegisterEventHandlers();
            // compatibility
            bool changed = false;
            //Default Analyzers
            if (!Operators.OfType<TradingSingleObjectiveTrainingNBestSolutionsAnalyzer>().Any())
            {
                Operators.Add(new TradingSingleObjectiveTrainingNBestSolutionsAnalyzer());
                changed = true;
            }
            if (changed)
            {
                ParameterizeOperators();
            }
        }

        private void InitializeOperators()
        {
            Operators.Add(new TradingSingleObjectiveTrainingNBestSolutionsAnalyzer());
            ParameterizeOperators();
        }


    }
}
