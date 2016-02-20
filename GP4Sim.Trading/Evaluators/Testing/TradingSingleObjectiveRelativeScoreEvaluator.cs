using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GP4Sim.CSharpAgents;
using GP4Sim.Trading.Interfaces;
using GP4Sim.Trading.Simulation;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace GP4Sim.Trading.Evaluators
{
    [Item("Trading Relative Score Evaluator", "Trading Relative Score Evaluator")]
    [StorableClass]
    public class TradingSingleObjectiveRelativeScoreEvaluator : TradingSingleObjectiveEvaluator
    {
        public override bool Maximization { get { return true; } }

        #region Constructors
        [StorableConstructor]
        protected TradingSingleObjectiveRelativeScoreEvaluator(bool deserializing) : base(deserializing) { }
        protected TradingSingleObjectiveRelativeScoreEvaluator(TradingSingleObjectiveRelativeScoreEvaluator original, Cloner cloner)
            : base(original, cloner) { }

        public TradingSingleObjectiveRelativeScoreEvaluator() : base(TradingFitnessType.Relative) { }

        [StorableHook(HookType.AfterDeserialization)]
        private void AfterDeserialization()
        {
            evalType = TradingFitnessType.Relative;
        }


        public override IDeepCloneable Clone(Cloner cloner)
        {
            return new TradingSingleObjectiveRelativeScoreEvaluator(this, cloner);
        }
        #endregion

        public override IOperation InstrumentedApply()
        {
            AgentFunction agent = CompileTree();
            IEnumerable<int> rows = GenerateRowsToEvaluate();

            double quality = CalculateFitness(agent, EstimationLimitsParameter.ActualValue.Lower, EstimationLimitsParameter.ActualValue.Upper, ProblemDataParameter.ActualValue, rows);

            QualityParameter.ActualValue = new DoubleValue(quality);

            return base.InstrumentedApply();
        }


        //Validation/OOS Entry Point
        public override double Evaluate(IExecutionContext context, ISymbolicExpressionTree tree, ITradingProblemData problemData, IEnumerable<int> rows)
        {
            SymbolicDataAnalysisTreeInterpreterParameter.ExecutionContext = context;
            SymbolicExpressionGrammarParameter.ExecutionContext = context;
            EstimationLimitsParameter.ExecutionContext = context;
            ApplyLinearScalingParameter.ExecutionContext = context;


            AgentFunction agent = CompileTree(tree);

            double result = CalculateFitness(agent, EstimationLimitsParameter.ActualValue.Lower, EstimationLimitsParameter.ActualValue.Upper, problemData, rows);

            SymbolicDataAnalysisTreeInterpreterParameter.ExecutionContext = null;
            SymbolicExpressionGrammarParameter.ExecutionContext = null;
            EstimationLimitsParameter.ExecutionContext = null;
            ApplyLinearScalingParameter.ExecutionContext = null;

            return result;
        }
    }
}
