using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GP4Sim.Collections;
using GP4Sim.SimulationFramework.Interfaces;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.DataAnalysis;
using HeuristicLab.Problems.DataAnalysis.Symbolic;

namespace GP4Sim.SimulationFramework.Analyzers
{
    [Item("SimulationSingleObjectiveTestNBestSolutionsAnalyzer", "An operator that analyzes the test best abstract solution for single objective symbolic data analysis problems.")]
    [StorableClass]
    public abstract class SimulationSingleObjectiveTestNBestSolutionsAnalyzer<T, U, V, Z> : SymbolicDataAnalysisSingleObjectiveAnalyzer, ISymbolicDataAnalysisInterpreterOperator, ISymbolicDataAnalysisBoundedOperator
        where T : class, ISimulationProblemData
        where U : class, ISimulationSingleObjectiveEvaluator<T>
        where V : class, ISimulationModel<T>
        where Z : class, ISimulationSolution<T, V>
    {
        protected const string NumTopSolutionsParameterName = "Number of Top Solutions to Show";
        protected const string TestBestSolutionsParameterName = "Best test solutions";
        protected const string TestBestSolutionQualitiesParameterName = "Best test solution qualities";
        protected const string UpdateAlwaysParameterName = "Always update best solution";
        protected const string ProblemDataParameterName = "ProblemData";
        protected const string SymbolicDataAnalysisTreeInterpreterParameterName = "SymbolicDataAnalysisTreeInterpreter";
        protected const string EstimationLimitsParameterName = "EstimationLimits";
        protected const string EvaluatorParameterName = "Evaluator";
        protected const string SymbolicExpressionGrammarParameterName = "SymbolicExpressionTreeGrammar";
        protected const string PercentageOfBestSolutionsParameterName = "Percentage Of Best Solutions";

        #region Parameter Properties
        public IValueParameter<IntValue> NumTopSolutionsParameter
        {
            get { return (IValueParameter<IntValue>)Parameters[NumTopSolutionsParameterName]; }
        }
        public ILookupParameter<U> EvaluatorParameter
        {
            get { return (ILookupParameter<U>)Parameters[EvaluatorParameterName]; }
        }
        public ILookupParameter<T> ProblemDataParameter
        {
            get { return (ILookupParameter<T>)Parameters[ProblemDataParameterName]; }
        }
        public ILookupParameter<ISymbolicDataAnalysisExpressionTreeInterpreter> SymbolicDataAnalysisTreeInterpreterParameter
        {
            get { return (ILookupParameter<ISymbolicDataAnalysisExpressionTreeInterpreter>)Parameters[SymbolicDataAnalysisTreeInterpreterParameterName]; }
        }
        public ILookupParameter<ISymbolicExpressionGrammar> SymbolicExpressionGrammarParameter
        {
            get { return (ILookupParameter<ISymbolicExpressionGrammar>)Parameters[SymbolicExpressionGrammarParameterName]; }
        }
        public IValueLookupParameter<DoubleLimit> EstimationLimitsParameter
        {
            get { return (IValueLookupParameter<DoubleLimit>)Parameters[EstimationLimitsParameterName]; }
        }
        /*
        public IScopeTreeLookupParameter<CompiledSymbolicExpressionTree> CompiledAgentsParameter
        {
            get { return (IScopeTreeLookupParameter<CompiledSymbolicExpressionTree>)Parameters[CompiledAgentsParameterName]; }
        }
        */
        public ILookupParameter<SolutionList<Z>> TestBestSolutionsParameter
        {
            get { return (ILookupParameter<SolutionList<Z>>)Parameters[TestBestSolutionsParameterName]; }
        }
        public ILookupParameter<ItemList<DoubleValue>> TestBestSolutionQualitiesParameter
        {
            get { return (ILookupParameter<ItemList<DoubleValue>>)Parameters[TestBestSolutionQualitiesParameterName]; }
        }
        public IFixedValueParameter<BoolValue> UpdateAlwaysParameter
        {
            get { return (IFixedValueParameter<BoolValue>)Parameters[UpdateAlwaysParameterName]; }
        }
        public IValueLookupParameter<PercentValue> PercentageOfBestSolutionsParameter
        {
            get { return (IValueLookupParameter<PercentValue>)Parameters[PercentageOfBestSolutionsParameterName]; }
        }
        #endregion

        #region Properties
        /*
        public ItemArray<CompiledSymbolicExpressionTree> CompiledAgents
        {
            get { return CompiledAgentsParameter.ActualValue; }
        }
        */
        public IntValue NumTopSolutions
        {
            get { return NumTopSolutionsParameter.Value; }
            set { NumTopSolutionsParameter.Value = value; }
        }
        public SolutionList<Z> TestBestSolutions
        {
            get { return TestBestSolutionsParameter.ActualValue; }
            set { TestBestSolutionsParameter.ActualValue = value; }
        }
        public ItemList<DoubleValue> TestBestSolutionQualities
        {
            get { return TestBestSolutionQualitiesParameter.ActualValue; }
            set { TestBestSolutionQualitiesParameter.ActualValue = value; }
        }
        public BoolValue UpdateAlways
        {
            get { return UpdateAlwaysParameter.Value; }
        }
        #endregion

        #region Constructors
        [StorableConstructor]
        protected SimulationSingleObjectiveTestNBestSolutionsAnalyzer(bool deserializing) : base(deserializing) { }
        protected SimulationSingleObjectiveTestNBestSolutionsAnalyzer(SimulationSingleObjectiveTestNBestSolutionsAnalyzer<T, U, V, Z> original, Cloner cloner) : base(original, cloner) { }
        protected SimulationSingleObjectiveTestNBestSolutionsAnalyzer()
            : base()
        {
            Parameters.Add(new ValueParameter<IntValue>(NumTopSolutionsParameterName, "Number of Top Solutions to Show", new IntValue(10)));
            Parameters.Add(new LookupParameter<U>(EvaluatorParameterName, "The operator to use for fitness evaluation."));
            Parameters.Add(new LookupParameter<T>(ProblemDataParameterName, "The problem data for the symbolic Trading solution."));
            Parameters.Add(new LookupParameter<ISymbolicDataAnalysisExpressionTreeInterpreter>(SymbolicDataAnalysisTreeInterpreterParameterName, "The symbolic data analysis tree interpreter for the symbolic expression tree."));
            Parameters.Add(new ValueLookupParameter<DoubleLimit>(EstimationLimitsParameterName, "The lower and upper limit for the estimated values produced by the symbolic Trading model."));
            Parameters.Add(new LookupParameter<SolutionList<Z>>(TestBestSolutionsParameterName, "The test best symbolic data analyis solution."));
            Parameters.Add(new LookupParameter<ItemList<DoubleValue>>(TestBestSolutionQualitiesParameterName, "The quality of the test best symbolic data analysis solution."));
            Parameters.Add(new FixedValueParameter<BoolValue>(UpdateAlwaysParameterName, "Determines if the best test solution should always be updated regardless of its quality.", new BoolValue(false)));
            Parameters.Add(new ValueLookupParameter<ISymbolicExpressionGrammar>(SymbolicExpressionGrammarParameterName, "The tree-generating grammar"));
            Parameters.Add(new ValueLookupParameter<PercentValue>(PercentageOfBestSolutionsParameterName, "The percentage of the top solutions which should be analyzed.", new PercentValue(0.1)));
            UpdateAlwaysParameter.Hidden = true;
            SymbolicDataAnalysisTreeInterpreterParameter.ActualName = "SymbolicExpressionTreeInterpreter";
        }

        [StorableHook(HookType.AfterDeserialization)]
        protected void AfterDeserialization()
        {
            if (!Parameters.ContainsKey(PercentageOfBestSolutionsParameterName))
                Parameters.Add(new ValueLookupParameter<PercentValue>(PercentageOfBestSolutionsParameterName, "The percentage of the top solutions which should be analyzed.", new PercentValue(0.1)));

            if (!Parameters.ContainsKey(UpdateAlwaysParameterName))
            {
                Parameters.Add(new FixedValueParameter<BoolValue>(UpdateAlwaysParameterName, "Determines if the best test solution should always be updated regardless of its quality.", new BoolValue(false)));
                UpdateAlwaysParameter.Hidden = true;
            }
        }
        #endregion

        public override IOperation Apply()
        {
            #region Initialization
            var results = ResultCollection;

            if (TestBestSolutions == null)
            {
                TestBestSolutions = new SolutionList<Z>();
                TestBestSolutionQualities = new ItemList<DoubleValue>();
                results.Add(new Result(TestBestSolutionsParameter.Name, TestBestSolutionsParameter.Description, TestBestSolutions));
                results.Add(new Result(TestBestSolutionQualitiesParameter.Name, TestBestSolutionQualitiesParameter.Description, TestBestSolutionQualities));
            }

            var evaluator = EvaluatorParameter.ActualValue;
            var problemData = ProblemDataParameter.ActualValue;

            //IEnumerable<int> rows = Enumerable.Range(problemData.TestPartition.Start, Math.Max(0, problemData.TestPartition.End - problemData.TestPartition.Start));
            IEnumerable<int> rows = problemData.TestIndices;
            if (!rows.Any())
                return base.Apply();
            #endregion

            #region Find Best N Training Trees

            double[] qualities = Quality.Select(x => x.Value).ToArray();
            ISymbolicExpressionTree[] trees = SymbolicExpressionTree.ToArray();

            int topN = (int)Math.Max(trees.Length * PercentageOfBestSolutionsParameter.ActualValue.Value, 1);

            var topIndexes = (from q in qualities
                              orderby q descending
                              select Array.IndexOf(qualities, q));
            if (!Maximization.Value)
                topIndexes = topIndexes.Reverse();

            int[] topIndexesArray = topIndexes.Take(topN).ToArray();

            #endregion

            #region Test Set Evaluation

            ISymbolicExpressionTree[] treesToTest = new ISymbolicExpressionTree[topIndexesArray.Length];
            for (int i = 0; i < topIndexesArray.Length; i++)
                treesToTest[i] = trees[topIndexesArray[i]];

            IExecutionContext childContext = (IExecutionContext)ExecutionContext.CreateChildOperation(evaluator);
            //double[] validationQualities = treesToValidate.AsParallel().Select(t => evaluator.Evaluate(childContext, t, problemData, rows)).ToArray();

            double[] testQualities = treesToTest.AsParallel().Select(t => evaluator.Evaluate(childContext, t, problemData, rows)).ToArray();

            #endregion


            #region Find Better Current Test Trees
            Dictionary<int, double> Buffer = InitializeTempBuffer(TestBestSolutionQualities);

            for (int i = 0; i < treesToTest.Length; i++)
            {
                if (IsBetter(testQualities[i], Buffer))
                    UpdateBuffer(i, testQualities[i], Buffer);
            }

            //order final indexes
            if (Maximization.Value)
                Buffer = Buffer.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
            else
                Buffer = Buffer.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);

            //update solution object
            SolutionList<Z> finalBestSolutions = new SolutionList<Z>();
            ItemList<DoubleValue> finalBestSolutionQualities = new ItemList<DoubleValue>();

            foreach (int k in Buffer.Keys)
            {
                finalBestSolutionQualities.Add(new DoubleValue(Buffer[k]));

                if (k < NumTopSolutions.Value)
                    finalBestSolutions.Add(TestBestSolutions[k]);
                else
                    finalBestSolutions.Add(CreateSolution(treesToTest[k - NumTopSolutions.Value], Buffer[k]));
            }

            TestBestSolutions = finalBestSolutions;
            TestBestSolutionQualities = finalBestSolutionQualities;

            if (!results.ContainsKey(TestBestSolutionsParameter.Name))
            {
                results.Add(new Result(TestBestSolutionsParameter.Name, TestBestSolutionsParameter.Description, TestBestSolutions));
                results.Add(new Result(TestBestSolutionQualitiesParameter.Name, TestBestSolutionQualitiesParameter.Description, TestBestSolutionQualities));
            }
            else
            {
                results[TestBestSolutionsParameter.Name].Value = TestBestSolutions;
                results[TestBestSolutionQualitiesParameter.Name].Value = TestBestSolutionQualities;
            }

            #endregion


            return base.Apply();
        }

        protected abstract Z CreateSolution(ISymbolicExpressionTree tree, double bestQuality);

        #region Private Methods
        private Dictionary<int, double> InitializeTempBuffer(ItemList<DoubleValue> oldBestSolutionQualities)
        {
            Dictionary<int, double> buffer = new Dictionary<int, double>();

            foreach (DoubleValue dv in oldBestSolutionQualities)
                buffer.Add(oldBestSolutionQualities.IndexOf(dv), dv.Value);

            return buffer;
        }

        private bool IsBetter(double quality, Dictionary<int, double> buffer)
        {

            if (buffer.Count < NumTopSolutions.Value)
                return true;
            else if (Maximization.Value && buffer.Values.Max() < quality)
                return true;
            else if (!Maximization.Value && buffer.Values.Min() > quality)
                return true;

            return false;
        }

        private void UpdateBuffer(int idx, double quality, Dictionary<int, double> buffer)
        {
            idx += NumTopSolutions.Value;

            if (buffer.Count < NumTopSolutions.Value)
                buffer.Add(idx, quality);
            else
            {

                if (Maximization.Value)
                {
                    buffer.Remove(buffer.Keys.Where(x => buffer[x] == buffer.Values.Min()).First());
                }
                else
                {
                    buffer.Remove(buffer.Keys.Where(x => buffer[x] == buffer.Values.Max()).First());
                }

                buffer.Add(idx, quality);
            }
        }
        #endregion
    }
}
