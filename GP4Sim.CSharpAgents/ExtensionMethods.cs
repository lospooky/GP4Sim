using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GP4Sim.SymbolicTrees;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Problems.DataAnalysis.Symbolic;

namespace GP4Sim.CSharpAgents
{
    public static class ExtensionMethods
    {
        public static bool IsTerminal(this ISymbolicExpressionTreeNode node)
        {
            if (node.Symbol is Constant || node.Symbol is LaggedVariable || node.Symbol is HeuristicLab.Problems.DataAnalysis.Symbolic.Variable || node.Symbol is InternalState)
                return true;
            return false;
        }
    }
}
