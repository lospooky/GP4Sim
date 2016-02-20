using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Common;
using HeuristicLab.Problems.DataAnalysis.Symbolic;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Data;
using GP4Sim.SimulationFramework.Interfaces;
using GP4Sim.CSharpAgents;
using GP4Sim.Trading.Interfaces;
using GP4Sim.Trading.Simulation;

namespace GP4Sim.Trading.Evaluators
{
    [Item("Trading NAV Evaluator", "42? 42!")]
    [StorableClass]
    public class TradingSingleObjectiveDummyEvaluator : TradingSingleObjectiveEvaluator, IConstantOptimizationEvaluator
    {

        public override bool Maximization { get { return false; } }


        [StorableConstructor]
        protected TradingSingleObjectiveDummyEvaluator(bool deserializing) : base(deserializing) { }
        protected TradingSingleObjectiveDummyEvaluator(TradingSingleObjectiveDummyEvaluator original, Cloner cloner)
            : base(original, cloner)
        {
        }

        public TradingSingleObjectiveDummyEvaluator() : base(TradingFitnessType.NAV) { }

        public override IDeepCloneable Clone(Cloner cloner)
        {
            return new TradingSingleObjectiveDummyEvaluator(this, cloner);
        }

        public override IOperation InstrumentedApply()
        {
            AgentFunction agent = CompileTree();

            //var tree = SymbolicExpressionTreeParameter.ActualValue;
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
