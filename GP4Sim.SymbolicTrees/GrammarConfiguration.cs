using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.Problems.DataAnalysis.Symbolic;

namespace GP4Sim.SymbolicTrees
{
    public static partial class Utilities
    {
        private const string TrigonometricFunctionsName = "Trigonometric Functions";
        private const string PowerFunctionsName = "Power Functions";
        private const string SpecialFunctionsName = "Special Functions";
        private const string ConditionalSymbolsName = "ConditionalSymbols";
        private const string TimeSeriesSymbolsName = "Time Series Symbols";

        public static void ConfigureAsDefaultSimulationGrammar(this TypeCoherentStateExpressionGrammar g)
        {
            g.Symbols.First(s => s is Average).Enabled = false;
            g.Symbols.First(s => s.Name == TrigonometricFunctionsName).Enabled = false;
            g.Symbols.First(s => s.Name == PowerFunctionsName).Enabled = false;
            g.Symbols.First(s => s.Name == SpecialFunctionsName).Enabled = false;
            g.Symbols.First(s => s.Name == ConditionalSymbolsName).Enabled = false;
            //g.Symbols.First(s => s.Name == TimeSeriesSymbolsName).Enabled = false;

        }
    }
}
