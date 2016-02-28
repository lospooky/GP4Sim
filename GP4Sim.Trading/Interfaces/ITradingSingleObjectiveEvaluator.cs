using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.Problems.DataAnalysis.Symbolic;
using HeuristicLab.Core;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using GP4Sim.SimulationFramework.Interfaces;
using GP4Sim.SymbolicTrees;
using GP4Sim.Trading.Simulation;

namespace GP4Sim.Trading.Interfaces
{
    public interface ITradingSingleObjectiveEvaluator : ITradingEvaluator, ISimulationSingleObjectiveEvaluator<ITradingProblemData>
    {
        ITradingEnvelope Analyze(SymbolicAbstractTreeInterpreter interpreter, ISymbolicExpressionGrammar grammar, ISymbolicExpressionTree tree, ITradingProblemData problem, IEnumerable<int> rows);
        string SimulationLog(SymbolicAbstractTreeInterpreter interpreter, ISymbolicExpressionGrammar grammar, ISymbolicExpressionTree tree, ITradingProblemData problem, IEnumerable<int> rows);
    }
}
