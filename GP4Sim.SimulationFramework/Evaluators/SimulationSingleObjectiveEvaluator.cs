using System;
using System.Collections.Generic;
using GP4Sim.CSharpAgents;
using GP4Sim.SimulationFramework.Interfaces;
using GP4Sim.SymbolicTrees;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.DataAnalysis.Symbolic;

namespace GP4Sim.SimulationFramework.Evaluators
{
    [StorableClass]
    public abstract class SimulationSingleObjectiveEvaluator<T, U> : SymbolicDataAnalysisSingleObjectiveEvaluator<T>, ISimulationSingleObjectiveEvaluator<T>
        where T : class, ISimulationProblemData
        where U : class, ISimulationEnvelope
    {
        private const string SymbolicExpressionGrammarParameterName = "SymbolicExpressionTreeGrammar";

        public ILookupParameter<ISymbolicExpressionGrammar> SymbolicExpressionGrammarParameter
        {
            get { return (ILookupParameter<ISymbolicExpressionGrammar>)Parameters[SymbolicExpressionGrammarParameterName]; }
        }



        [StorableConstructor]
        protected SimulationSingleObjectiveEvaluator(bool deserializing) : base(deserializing) { }
        protected SimulationSingleObjectiveEvaluator(SimulationSingleObjectiveEvaluator<T, U> original, Cloner cloner) : base(original, cloner) { }
        protected SimulationSingleObjectiveEvaluator()
            : base()
        {
            Parameters.Add(new ValueLookupParameter<ISymbolicExpressionGrammar>(SymbolicExpressionGrammarParameterName, "The tree-generating grammar"));
            SymbolicExpressionGrammarParameter.Hidden = false;
            SymbolicDataAnalysisTreeInterpreterParameter.Hidden = false;
            return;
        }


        protected AgentFunction CompileTree()
        {
            ISymbolicExpressionTree tree = SymbolicExpressionTreeParameter.ActualValue;

            return CompileTree(tree);
        }


        protected AgentFunction CompileTree(ISymbolicExpressionTree tree)
        {
            SymbolicAbstractTreeInterpreter interpreter;
            if (SymbolicDataAnalysisTreeInterpreterParameter.ActualValue is SymbolicAbstractTreeInterpreter)
                interpreter = SymbolicDataAnalysisTreeInterpreterParameter.ActualValue as SymbolicAbstractTreeInterpreter;
            else
                throw new NotSupportedException("Interpreter Type Not Supported");

            //return new CompiledSymbolicExpressionTree(interpreter.GetCompiledFunction(tree, SymbolicExpressionGrammarParameter.ActualValue));

            return new AgentFunction(interpreter.GetCompiledFunction(tree, SymbolicExpressionGrammarParameter.ActualValue));
        }

        protected AgentFunction CompileTree(ISymbolicExpressionTree tree, SymbolicAbstractTreeInterpreter interpreter, ISymbolicExpressionGrammar grammar)
        {
            return new AgentFunction(interpreter.GetCompiledFunction(tree, grammar));
        }

        //Training Entry Point
        protected abstract double CalculateFitness(AgentFunction agent, double lowerEstimationLimit, double upperEstimationLimit, T problemData, IEnumerable<int> rows);

        //Analyzers' Entry Point
        //public abstract U Analyze(IExecutionContext context, ISymbolicExpressionTree tree, T problem, IEnumerable<int> rows);

        //TOFIX
        protected int MinLag
        {
            get
            {
                if (SymbolicExpressionGrammarParameter.ActualValue != null)
                    return LagLimit(SymbolicExpressionGrammarParameter.ActualValue);
                else
                    throw new ArgumentNullException("Grammar is Null");
            }
        }

        protected int LagLimit(ISymbolicExpressionGrammar grammar)
        {
            if (grammar.ContainsSymbol(new LaggedVariable()))
                return ((grammar.GetSymbol("LaggedVariable")) as LaggedVariable).MinLag;
            else
                return 0;
        }

    }
}
