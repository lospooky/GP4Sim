using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Core;
using HeuristicLab.Common;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Data;
using HeuristicLab.Problems.DataAnalysis.Symbolic;
using GP4Sim.SimulationFramework.Solutions;
using GP4Sim.Trading.Interfaces;
using GP4Sim.SymbolicTrees;
using GP4Sim.Data;
using GP4Sim.Trading.Problem;
using GP4Sim.Trading.Simulation;

namespace GP4Sim.Trading.Solutions
{
    [StorableClass]
    [Item(Name = "Symbolic Trading Model", Description = "Represents a symbolic concrete model.")]
    public class TradingModel : SimulationModel<ITradingProblemData, TradingEnvelope>, ITradingModel
    {

        #region Constructors
        [StorableConstructor]
        protected TradingModel(bool deserializing) : base(deserializing) { }
        protected TradingModel(TradingModel original, Cloner cloner)
            : base(original, cloner)
        {
        }

        public TradingModel(ISymbolicExpressionTree tree, ISymbolicAbstractTreeInterpreter interpreter, ISymbolicExpressionGrammar grammar,
          ITradingSingleObjectiveEvaluator evaluator, double lowerEstimationLimit = double.MinValue, double upperEstimationLimit = double.MaxValue)
            : base(tree, interpreter, grammar, evaluator, lowerEstimationLimit, upperEstimationLimit)
        {

        }

        public override IDeepCloneable Clone(Cloner cloner)
        {
            return new TradingModel(this, cloner);
        }

        public ITradingSingleObjectiveEvaluator Evaluator { get { return (ITradingSingleObjectiveEvaluator)evaluator; } }

        #endregion

        public double[] GetDailyNavPoints(ITradingProblemData problemData, IEnumerable<int> rows)
        {
            if (!isCached(rows))
                Evaluate(problemData, rows);

            return Cache[rows.ToIntRange()].DailyNavPoints.ToArray();
        }

        public double[] GetDailyInstrPoints(ITradingProblemData problemData, IEnumerable<int> rows)
        {
            if (!isCached(rows))
                Evaluate(problemData, rows);

            return Cache[rows.ToIntRange()].DailyInstrPoints.ToArray();
        }

        public DateTime[] GetDayPoints(ITradingProblemData problemData, IEnumerable<int> rows)
        {
            if (!isCached(rows))
                Evaluate(problemData, rows);

            return Cache[rows.ToIntRange()].DayPoints.ToArray();
        }

        public ITradingSolution CreateSolution(ITradingProblemData problemData)
        {
            return new TradingSolution(this, new TradingProblemData(problemData));
        }

        protected override void Evaluate(ITradingProblemData problemData, IEnumerable<int> rows)
        {

            TradingEnvelope resultsEnvelope = (TradingEnvelope)Evaluator.Analyze(Interpreter as SymbolicAbstractTreeInterpreter, Grammar, SymbolicExpressionTree, problemData, rows);
            Cache.Add(new IntRange2(rows.First(), rows.Last()), resultsEnvelope);
        }


        public string GetSimulationLog(ITradingProblemData problemData, IEnumerable<int> rows)
        {
            return Evaluator.SimulationLog(Interpreter as SymbolicAbstractTreeInterpreter, Grammar, SymbolicExpressionTree, problemData, rows);
        }
    }
}
