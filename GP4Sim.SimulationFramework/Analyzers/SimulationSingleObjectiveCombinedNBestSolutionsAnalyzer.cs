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
    [Item("SimulationSingleObjectiveTrTeNBestSolutionsAnalyzer", "An operator that analyzes the training + test best abstract solution for single objective symbolic data analysis problems.")]
    [StorableClass]
    public abstract class SimulationSingleObjectiveCombinedNBestSolutionsAnalyzer<T, U, V, Z> : SymbolicDataAnalysisSingleObjectiveAnalyzer, ISymbolicDataAnalysisInterpreterOperator, ISymbolicDataAnalysisBoundedOperator
        where T : class, ISimulationProblemData
        where U : class, ISimulationSingleObjectiveEvaluator<T>
        where V : class, ISimulationModel<T>
        where Z : class, ISimulationSolution<T, V>
    {
        protected const string NumTopSolutionsParameterName = "Number of Top Solutions to Show";
        protected const string TrTeBestSolutionsParameterName = "Best Training+Test profitable solutions";
        protected const string TrTeBestSolutionQualitiesParameterName = "Best Training+Test profitable solution qualities";
        protected const string UpdateAlwaysParameterName = "Always update best solutions";
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
        public ILookupParameter<SolutionList<Z>> TrTeBestSolutionsParameter
        {
            get { return (ILookupParameter<SolutionList<Z>>)Parameters[TrTeBestSolutionsParameterName]; }
        }
        public ILookupParameter<ItemList<DoubleValue>> TrTeBestSolutionQualitiesParameter
        {
            get { return (ILookupParameter<ItemList<DoubleValue>>)Parameters[TrTeBestSolutionQualitiesParameterName]; }
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
        public SolutionList<Z> TrTeBestSolutions
        {
            get { return TrTeBestSolutionsParameter.ActualValue; }
            set { TrTeBestSolutionsParameter.ActualValue = value; }
        }
        public ItemList<DoubleValue> TrTeBestSolutionQualities
        {
            get { return TrTeBestSolutionQualitiesParameter.ActualValue; }
            set { TrTeBestSolutionQualitiesParameter.ActualValue = value; }
        }
        public BoolValue UpdateAlways
        {
            get { return UpdateAlwaysParameter.Value; }
        }
        #endregion

        #region Constructors
        [StorableConstructor]
        protected SimulationSingleObjectiveCombinedNBestSolutionsAnalyzer(bool deserializing) : base(deserializing) { }
        protected SimulationSingleObjectiveCombinedNBestSolutionsAnalyzer(SimulationSingleObjectiveCombinedNBestSolutionsAnalyzer<T, U, V, Z> original, Cloner cloner) : base(original, cloner) { }
        protected SimulationSingleObjectiveCombinedNBestSolutionsAnalyzer()
            : base()
        {
            Parameters.Add(new ValueParameter<IntValue>(NumTopSolutionsParameterName, "Number of Top Solutions to Show", new IntValue(10)));
            Parameters.Add(new LookupParameter<U>(EvaluatorParameterName, "The operator to use for fitness evaluation."));
            Parameters.Add(new LookupParameter<T>(ProblemDataParameterName, "The problem data for the symbolic Trading solution."));
            Parameters.Add(new LookupParameter<ISymbolicDataAnalysisExpressionTreeInterpreter>(SymbolicDataAnalysisTreeInterpreterParameterName, "The symbolic data analysis tree interpreter for the symbolic expression tree."));
            Parameters.Add(new ValueLookupParameter<DoubleLimit>(EstimationLimitsParameterName, "The lower and upper limit for the estimated values produced by the symbolic Trading model."));
            Parameters.Add(new LookupParameter<SolutionList<Z>>(TrTeBestSolutionsParameterName, "The training + test best symbolic data analyis solutions."));
            Parameters.Add(new LookupParameter<ItemList<DoubleValue>>(TrTeBestSolutionQualitiesParameterName, "The quality of the training+test best symbolic data analysis solutions."));
            Parameters.Add(new FixedValueParameter<BoolValue>(UpdateAlwaysParameterName, "Determines if the best test solution should always be updated regardless of its quality.", new BoolValue(false)));
            Parameters.Add(new ValueLookupParameter<ISymbolicExpressionGrammar>(SymbolicExpressionGrammarParameterName, "The tree-generating grammar"));
            Parameters.Add(new ValueLookupParameter<PercentValue>(PercentageOfBestSolutionsParameterName, "The percentage of the top training solutions which should be analyzed.", new PercentValue(0.1)));
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

            if (TrTeBestSolutions == null)
            {
                TrTeBestSolutions = new SolutionList<Z>();
                TrTeBestSolutionQualities = new ItemList<DoubleValue>();
                results.Add(new Result(TrTeBestSolutionsParameter.Name, TrTeBestSolutionsParameter.Description, TrTeBestSolutions));
                results.Add(new Result(TrTeBestSolutionQualitiesParameter.Name, TrTeBestSolutionQualitiesParameter.Description, TrTeBestSolutionQualities));
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
            double[] trainQualities = topIndexesArray.Select(x => qualities[x]).ToArray();

            childContext = null;

            int[] okTrainIdx = trainQualities.Where(x => x < 1).Select(x => Array.IndexOf(trainQualities, x)).ToArray();
            int[] okTestIdx = testQualities.Where(x => x < 1).Select(x => Array.IndexOf(testQualities, x)).ToArray();

            int[] okIdx = okTrainIdx.Intersect(okTestIdx).ToArray();

            //double[] combiQualities = okIdx.Select(x => Math.Exp(-(-Math.Log(trainQualities[x]) + -Math.Log(testQualities[x])))).ToArray();
            //SPOOOOORCA!!
            //double[] combiQualities = okIdx.Select(x => testQualities[x]).ToArray();
            double[] combiQualities = okIdx.Select(x => CombiScore(trainQualities[x], testQualities[x])).ToArray();

            #endregion


            #region Find Better Current Test Trees
            Dictionary<int, double> Buffer = InitializeTempBuffer(TrTeBestSolutionQualities);

            for (int i = 0; i < combiQualities.Length; i++)
            {
                if (IsBetter(combiQualities[i], Buffer))
                    UpdateBuffer(okIdx[i], combiQualities[i], Buffer);
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
                    finalBestSolutions.Add(TrTeBestSolutions[k]);
                else
                    finalBestSolutions.Add(CreateSolution(treesToTest[k - NumTopSolutions.Value], Buffer[k]));
            }

            TrTeBestSolutions = finalBestSolutions;
            TrTeBestSolutionQualities = finalBestSolutionQualities;

            if (!results.ContainsKey(TrTeBestSolutionsParameter.Name))
            {
                results.Add(new Result(TrTeBestSolutionsParameter.Name, TrTeBestSolutionsParameter.Description, TrTeBestSolutions));
                results.Add(new Result(TrTeBestSolutionQualitiesParameter.Name, TrTeBestSolutionQualitiesParameter.Description, TrTeBestSolutionQualities));
            }
            else
            {
                results[TrTeBestSolutionsParameter.Name].Value = TrTeBestSolutions;
                results[TrTeBestSolutionQualitiesParameter.Name].Value = TrTeBestSolutionQualities;
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

        //Detailed in HL Ranking.mat
        private static double CombiScore(double trainScore, double testScore)
        {
            double rad = 1 - Math.Sqrt(Math.Pow((1 - trainScore), 2) + Math.Pow((1 - testScore), 2));
            double plane = Math.Abs(Math.Sqrt(2) * trainScore - Math.Sqrt(2) * testScore);

            double rawScore = rad - (1 - plane);

            double prettyScore = (rawScore + 1) / 1.25;

            return prettyScore;

        }
        #endregion
    }
}
