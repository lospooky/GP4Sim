using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Problems.DataAnalysis.Symbolic;

namespace GP4Sim.SymbolicTrees
{
    public interface ISymbolicAbstractTreeInterpreter : ISymbolicDataAnalysisExpressionTreeInterpreter
    {
        //Expression<Func<double[], double>> GetExpressionTree(ISymbolicExpressionTree tree, ISymbolicExpressionGrammar grammar);
        Expression<Func<int, double[][], double[], double>> GetExpressionTree(ISymbolicExpressionTree tree, ISymbolicExpressionGrammar grammar);
        Func<int, double[][], double[], double> GetCompiledFunction(ISymbolicExpressionTree tree, ISymbolicExpressionGrammar grammar);
    }
}
