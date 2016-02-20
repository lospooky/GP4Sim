using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.DataAnalysis;
using HeuristicLab.Problems.DataAnalysis.Symbolic;

namespace GP4Sim.SymbolicTrees
{
    [StorableClass]
    [Item("SymbolicStateExpressionTreeInterpreter", "Interpreter for symbolic expression tree with state nodes")]
    public sealed class SymbolicAbstractTreeInterpreter : ParameterizedNamedItem, ISymbolicDataAnalysisExpressionTreeInterpreter, ISymbolicAbstractTreeInterpreter
    {


        internal delegate double CompiledAgent(IList<double>[] inputs);

        private const string CheckExpressionsWithIntervalArithmeticParameterName = "CheckExpressionsWithIntervalArithmetic";
        private const string EvaluatedSolutionsParameterName = "EvaluatedSolutions";

        private static Type[] doubleSignature = { typeof(double) };
        private static Type[] doubleDoubleSignature = { typeof(double), typeof(double) };
        private static Type[] doubleIntSignature = { typeof(double), typeof(Int32) };

        private Dictionary<string, Expression> paramVarMap = null;
        private Dictionary<string, Expression> inputsVarMap = null;
        private Dictionary<string, Expression> statesVarMap = null;

       

        public override bool CanChangeName
        {
            get { return false; }
        }

        public override bool CanChangeDescription
        {
            get { return false; }
        }

        public Dictionary<string, Expression> StatesVarMap
        { get { return statesVarMap; } }

        #region Parameter Properties

        public IValueParameter<BoolValue> CheckExpressionsWithIntervalArithmeticParameter
        {
            get { return (IValueParameter<BoolValue>)Parameters[CheckExpressionsWithIntervalArithmeticParameterName]; }
        }

        public IValueParameter<IntValue> EvaluatedSolutionsParameter
        {
            get { return (IValueParameter<IntValue>)Parameters[EvaluatedSolutionsParameterName]; }
        }


        #endregion

        #region Properties

        public BoolValue CheckExpressionsWithIntervalArithmetic
        {
            get { return CheckExpressionsWithIntervalArithmeticParameter.Value; }
            set { CheckExpressionsWithIntervalArithmeticParameter.Value = value; }
        }

        public IntValue EvaluatedSolutions
        {
            get { return EvaluatedSolutionsParameter.Value; }
            set { EvaluatedSolutionsParameter.Value = value; }
        }

        #endregion

        #region IStatefulItem

        public void InitializeState()
        {
            EvaluatedSolutions.Value = 0;
        }

        public void ClearState()
        {
            EvaluatedSolutions.Value = 0;
        }

        #endregion

        #region Constructors
        [StorableConstructor]
        private SymbolicAbstractTreeInterpreter(bool deserializing) : base(deserializing) { }

        private SymbolicAbstractTreeInterpreter(SymbolicAbstractTreeInterpreter original, Cloner cloner)
            : base(original, cloner)
        {

        }
        public override IDeepCloneable Clone(Cloner cloner)
        {
            return new SymbolicAbstractTreeInterpreter(this, cloner);
        }

        public SymbolicAbstractTreeInterpreter()
            : base("SymbolicStateExpressionTreeInterpreter", "Interpreter for symbolic expression trees.")
        {
            Parameters.Add(new ValueParameter<BoolValue>(CheckExpressionsWithIntervalArithmeticParameterName, "Switch that determines if the interpreter checks the validity of expressions with interval arithmetic before evaluating the expression.", new BoolValue(false)));
            Parameters.Add(new ValueParameter<IntValue>(EvaluatedSolutionsParameterName, "A counter for the total number of solutions the interpreter has evaluated", new IntValue(0)));
        }

        [StorableHook(HookType.AfterDeserialization)]
        private void AfterDeserialization()
        {
            if (!Parameters.ContainsKey(EvaluatedSolutionsParameterName))
                Parameters.Add(new ValueParameter<IntValue>(EvaluatedSolutionsParameterName, "A counter for the total number of solutions the interpreter has evaluated", new IntValue(0)));
        }
        #endregion


        #region Public Methods
        public IEnumerable<double> GetSymbolicExpressionTreeValues(ISymbolicExpressionTree tree, Dataset dataset, IEnumerable<int> rows)
        {
            throw new NotSupportedException("Unsupported Interpreter Usage");
        }

        public Expression<Func<int, double[][], double[], double>> GetExpressionTree(ISymbolicExpressionTree tree, ISymbolicExpressionGrammar grammar)
        {
            ISymbolicExpressionTreeNode node = tree.Root.GetSubtree(0).GetSubtree(0);
            Expression treeExpression = RecursiveParser(node);

            lock (EvaluatedSolutions)
            {
                EvaluatedSolutions.Value++;
            }

            return Expression.Lambda<Func<int, double[][], double[], double>>(treeExpression, "Agent", true, new ParameterExpression[] { paramVarMap["rowIndex"] as ParameterExpression, paramVarMap["variables"] as ParameterExpression, paramVarMap["states"] as ParameterExpression });
        }

        public Func<int, double[][], double[], double> GetCompiledFunction(ISymbolicExpressionTree tree, ISymbolicExpressionGrammar grammar)
        {
            ISymbolicExpressionTreeNode node = tree.Root.GetSubtree(0).GetSubtree(0);
            Expression treeExpression = RecursiveParser(node);

            lock (EvaluatedSolutions)
            {
                EvaluatedSolutions.Value++;
            }

            return Expression.Lambda<Func<int, double[][], double[], double>>(treeExpression, "Agent", true, new ParameterExpression[] {paramVarMap["rowIndex"] as ParameterExpression, paramVarMap["variables"] as ParameterExpression, paramVarMap["states"] as ParameterExpression }).Compile();
        }

        public void UpdateInputsMap(IEnumerable<string> vars, IEnumerable<string> states)
        {
            //Separate for Variables and States
            paramVarMap = new Dictionary<string, Expression>();
            inputsVarMap = new Dictionary<string, Expression>();
            statesVarMap = new Dictionary<string, Expression>();

            ParameterExpression varParam = Expression.Parameter(typeof(double[][]), "variables");
            ParameterExpression statesParam = Expression.Parameter(typeof(double[]), "states");
            ParameterExpression rowIndexParam = Expression.Parameter(typeof(int), "rowIndex");
            paramVarMap.Add("variables", varParam);
            paramVarMap.Add("states", statesParam);
            paramVarMap.Add("rowIndex", rowIndexParam);

            //[][] form does not work with double indexing;
            List<string> varNames = vars.ToList();
            for (int i = 0; i < vars.Count(); i++)
                inputsVarMap.Add(varNames[i], Expression.Constant(i));
                //varMap.Add(varNames[i], Expression.ArrayIndex(varParam, Expression.Constant(i)));

            List<string> stateNames = states.ToList();
            for (int i = 0; i < states.Count(); i++)
                statesVarMap.Add(stateNames[i], Expression.ArrayIndex(statesParam, Expression.Constant(i)));
        }

        #endregion

        #region Private Methods

        private Expression RecursiveParser(ISymbolicExpressionTreeNode node)
        {
            ISymbol s = node.Symbol;
            Expression expr = null;

            if (s is Addition)
            {
                Expression left = RecursiveParser(node.GetSubtree(0));
                Expression right = RecursiveParser(node.GetSubtree(1));
                expr = Expression.Add(left, right);
            }
            else if (s is Subtraction)
            {
                Expression left = RecursiveParser(node.GetSubtree(0));
                Expression right = RecursiveParser(node.GetSubtree(1));
                expr = Expression.Subtract(left, right);
            }
            else if (s is Multiplication)
            {
                Expression left = RecursiveParser(node.GetSubtree(0));
                Expression right = RecursiveParser(node.GetSubtree(1));
                expr = Expression.Multiply(left, right);
            }
            else if (s is Division)
            {
                Expression left = RecursiveParser(node.GetSubtree(0));
                Expression right = RecursiveParser(node.GetSubtree(1));
                expr = Expression.Divide(left, right);
            }
            else if (s is Average)
            {
                Expression[] subs = new Expression[node.SubtreeCount];
                for (int i = 0; i < node.SubtreeCount; i++)
                {
                    subs[i] = RecursiveParser(node.GetSubtree(i));
                }

                expr = Expression.Call(null, typeof(Expression[]).GetMethod("Average"), subs);
            }
            else if (s is Sine)
            {
                Expression sub = RecursiveParser(node.GetSubtree(0));
                expr = Expression.Call(null, typeof(Math).GetMethod("Sin"), sub);
            }
            else if (s is Cosine)
            {
                Expression sub = RecursiveParser(node.GetSubtree(0));
                expr = Expression.Call(null, typeof(Math).GetMethod("Cos"), sub);
            }
            else if (s is Tangent)
            {
                Expression sub = RecursiveParser(node.GetSubtree(0));
                expr = Expression.Call(null, typeof(Math).GetMethod("Tan"), sub);
            }
            else if (s is Square)
            {
                Expression sub = RecursiveParser(node.GetSubtree(0));
                expr = Expression.Call(null, typeof(Math).GetMethod("Pow", doubleDoubleSignature), sub, Expression.Constant(2d));
            }
            else if (s is Power)
            {
                Expression powbase = RecursiveParser(node.GetSubtree(0));
                Expression rawexponent = RecursiveParser(node.GetSubtree(1));
                Expression exponent = Expression.Call(null, typeof(Math).GetMethod("Round", doubleSignature), rawexponent);
                expr = Expression.Call(null, typeof(Math).GetMethod("Pow", doubleIntSignature), powbase, exponent);
            }
            else if (s is SquareRoot)
            {
                Expression sub = RecursiveParser(node.GetSubtree(0));
                expr = Expression.Call(null, typeof(Math).GetMethod("Sqrt"), sub);
            }
            else if (s is Root)
            {
                Expression radicand = RecursiveParser(node.GetSubtree(0));
                Expression rawindex = RecursiveParser(node.GetSubtree(1));
                Expression index = Expression.Call(null, typeof(Math).GetMethod("Round", doubleSignature), rawindex);
                Expression exponent = Expression.Divide(Expression.Constant(1d, typeof(double)), index);
                expr = Expression.Call(null, typeof(Math).GetMethod("Pow", doubleDoubleSignature), radicand, exponent);
            }
            else if (s is Logarithm)
            {
                Expression sub = RecursiveParser(node.GetSubtree(0));
                expr = Expression.Call(null, typeof(Math).GetMethod("Log", doubleSignature), sub);
            }
            else if (s is Exponential)
            {
                Expression sub = RecursiveParser(node.GetSubtree(0));
                expr = Expression.Call(null, typeof(Math).GetMethod("Exp"), sub);
            }
            else if (s is IfThenElse)
            {
                Expression condition = RecursiveParser(node.GetSubtree(0));
                Expression trueBranch = RecursiveParser(node.GetSubtree(1));
                Expression falseBranch = RecursiveParser(node.GetSubtree(2));
                expr = Expression.Condition(condition, trueBranch, falseBranch, typeof(double));
            }
            else if (s is GreaterThan)
            {
                Expression left = RecursiveParser(node.GetSubtree(0));
                Expression right = RecursiveParser(node.GetSubtree(1));
                expr = Expression.GreaterThan(left, right);
            }
            else if (s is LessThan)
            {
                Expression left = RecursiveParser(node.GetSubtree(0));
                Expression right = RecursiveParser(node.GetSubtree(1));
                expr = Expression.LessThan(left, right);
            }
            else if (s is And)
            {
                Expression left = RecursiveParser(node.GetSubtree(0));
                Expression right = RecursiveParser(node.GetSubtree(1));
                expr = Expression.And(left, right);
            }
            else if (s is Or)
            {
                Expression left = RecursiveParser(node.GetSubtree(0));
                Expression right = RecursiveParser(node.GetSubtree(1));
                expr = Expression.And(left, right);
            }
            else if (s is Not)
            {
                Expression sub = RecursiveParser(node.GetSubtree(0));
                expr = Expression.Not(sub);
            }
            else if (s is Constant)
            {
                expr = Expression.Constant((node as ConstantTreeNode).Value);
            }
            else if (s is LaggedVariable)
            {
                LaggedVariableTreeNode vn = node as LaggedVariableTreeNode;

                //LAG Parameter is already negative
                Expression offSet = Expression.Add(paramVarMap["rowIndex"], Expression.Constant(vn.Lag));
                //Expression ve = Expression.Lambda(varMap[vn.VariableName], offSet as ParameterExpression);
                Expression ve = Expression.ArrayIndex(Expression.ArrayIndex(paramVarMap["variables"], offSet), inputsVarMap[vn.VariableName]);
                
                if (vn.Weight != 1)
                {
                    Expression we = Expression.Constant(vn.Weight);
                    expr = Expression.Multiply(ve, we);
                }
                else
                    expr = ve;

            }
            else if (s is HeuristicLab.Problems.DataAnalysis.Symbolic.Variable)
            {
                VariableTreeNode vn = node as VariableTreeNode;

                Expression ve = Expression.ArrayIndex(Expression.ArrayIndex(paramVarMap["variables"], paramVarMap["rowIndex"]), inputsVarMap[vn.VariableName]);

                if (vn.Weight != 1)
                {
                    Expression we = Expression.Constant(vn.Weight);
                    expr = Expression.Multiply(ve, we);
                }
                else
                    expr = ve;
            }
            else if (s is InternalState)
            {
                InternalStateTreeNode sn = node as InternalStateTreeNode;
                Expression se = statesVarMap[sn.VariableName];
                if (sn.Weight != 1)
                {
                    Expression we = Expression.Constant(sn.Weight);
                    expr = Expression.Multiply(se, we);
                }
                else
                    expr = se;
            }
            else if (s is AiryA)
            {
                Expression sub = RecursiveParser(node.GetSubtree(0));
                expr = Expression.Call(null, FunctionLibrary.AiryA, sub);
            }
            else if (s is AiryB)
            {
                Expression sub = RecursiveParser(node.GetSubtree(0));
                expr = Expression.Call(null, FunctionLibrary.AiryB, sub);
            }
            else if (s is Gamma)
            {
                Expression sub = RecursiveParser(node.GetSubtree(0));
                expr = Expression.Call(null, FunctionLibrary.Gamma, sub);
            }
            else if (s is Psi)
            {
                Expression sub = RecursiveParser(node.GetSubtree(0));
                expr = Expression.Call(null, FunctionLibrary.Psi, sub);
            }
            else if (s is Dawson)
            {
                Expression sub = RecursiveParser(node.GetSubtree(0));
                expr = Expression.Call(null, FunctionLibrary.Dawson, sub);
            }
            else if (s is ExponentialIntegralEi)
            {
                Expression sub = RecursiveParser(node.GetSubtree(0));
                expr = Expression.Call(null, FunctionLibrary.ExpIntegralEi, sub);
            }
            else if (s is SineIntegral)
            {
                Expression sub = RecursiveParser(node.GetSubtree(0));
                expr = Expression.Call(null, FunctionLibrary.SinIntegral, sub);
            }
            else if (s is CosineIntegral)
            {
                Expression sub = RecursiveParser(node.GetSubtree(0));
                expr = Expression.Call(null, FunctionLibrary.CosIntegral, sub);
            }
            else if (s is HyperbolicSineIntegral)
            {
                Expression sub = RecursiveParser(node.GetSubtree(0));
                expr = Expression.Call(null, FunctionLibrary.HypSinIntegral, sub);
            }
            else if (s is HyperbolicCosineIntegral)
            {
                Expression sub = RecursiveParser(node.GetSubtree(0));
                expr = Expression.Call(null, FunctionLibrary.HypCosIntegral, sub);
            }
            else if (s is FresnelSineIntegral)
            {
                Expression sub = RecursiveParser(node.GetSubtree(0));
                expr = Expression.Call(null, FunctionLibrary.FresnelSinIntegral, sub);
            }
            else if (s is FresnelCosineIntegral)
            {
                Expression sub = RecursiveParser(node.GetSubtree(0));
                expr = Expression.Call(null, FunctionLibrary.FresnelCosIntegral, sub);
            }
            else if (s is Norm)
            {
                Expression sub = RecursiveParser(node.GetSubtree(0));
                expr = Expression.Call(null, FunctionLibrary.Norm, sub);
            }
            else if (s is Erf)
            {
                Expression sub = RecursiveParser(node.GetSubtree(0));
                expr = Expression.Call(null, FunctionLibrary.Erf, sub);
            }
            else if (s is Bessel)
            {
                Expression sub = RecursiveParser(node.GetSubtree(0));
                expr = Expression.Call(null, FunctionLibrary.Bessel, sub);
            }
            return expr;
        }

        public IEnumerable<double> GetSymbolicExpressionTreeValues(ISymbolicExpressionTree tree, IDataset dataset, IEnumerable<int> rows)
        {
            throw new NotImplementedException();
        }



        #endregion
    }

}
