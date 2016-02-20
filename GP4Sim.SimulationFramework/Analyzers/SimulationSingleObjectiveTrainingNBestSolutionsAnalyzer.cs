using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.DataAnalysis.Symbolic;
using HeuristicLab.Data;
using HeuristicLab.Problems.DataAnalysis;
using HeuristicLab.Parameters;
using HeuristicLab.Common;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Optimization;
using GP4Sim.SimulationFramework.Interfaces;
using GP4Sim.Collections;

namespace GP4Sim.SimulationFramework.Analyzers
{
    [Item("SimulationSingleObjectiveTrainingNBestSolutionsAnalyzer", "An operator that analyzes the training best abstract solution for single objective symbolic data analysis problems.")]
    [StorableClass]
    public abstract class SimulationSingleObjectiveTrainingNBestSolutionsAnalyzer<T, U, V, Z> : SymbolicDataAnalysisSingleObjectiveAnalyzer, ISymbolicDataAnalysisInterpreterOperator, ISymbolicDataAnalysisBoundedOperator
        where T : class, ISimulationProblemData
        where U : class, ISimulationSingleObjectiveEvaluator<T>
        where V : class, ISimulationModel<T>
        where Z : class, ISimulationSolution<T, V>
    {
        protected const string NumTopSolutionsParameterName = "Number of Top Solutions to Show";
        protected const string TrainingBestSolutionsParameterName = "Best training solutions";
        protected const string TrainingBestSolutionQualitiesParameterName = "Best training solution qualities";
        protected const string UpdateAlwaysParameterName = "Always update best solution";
        protected const string ProblemDataParameterName = "ProblemData";
        protected const string SymbolicDataAnalysisTreeInterpreterParameterName = "SymbolicDataAnalysisTreeInterpreter";
        protected const string EstimationLimitsParameterName = "EstimationLimits";
        //protected const string CompiledAgentsParameterName = "CompiledAgent";
        protected const string EvaluatorParameterName = "Evaluator";
        private const string SymbolicExpressionGrammarParameterName = "SymbolicExpressionTreeGrammar";


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
        public ILookupParameter<SolutionList<Z>> TrainingBestSolutionsParameter
        {
            get { return (ILookupParameter<SolutionList<Z>>)Parameters[TrainingBestSolutionsParameterName]; }
        }
        public ILookupParameter<ItemList<DoubleValue>> TrainingBestSolutionQualitiesParameter
        {
            get { return (ILookupParameter<ItemList<DoubleValue>>)Parameters[TrainingBestSolutionQualitiesParameterName]; }
        }
        public IFixedValueParameter<BoolValue> UpdateAlwaysParameter
        {
            get { return (IFixedValueParameter<BoolValue>)Parameters[UpdateAlwaysParameterName]; }
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
        public SolutionList<Z> TrainingBestSolutions
        {
            get { return TrainingBestSolutionsParameter.ActualValue; }
            set { TrainingBestSolutionsParameter.ActualValue = value; }
        }
        public ItemList<DoubleValue> TrainingBestSolutionQualities
        {
            get { return TrainingBestSolutionQualitiesParameter.ActualValue; }
            set { TrainingBestSolutionQualitiesParameter.ActualValue = value; }
        }
        public BoolValue UpdateAlways
        {
            get { return UpdateAlwaysParameter.Value; }
        }
        public T ProblemData
        {
            get { return ProblemDataParameter.ActualValue; }
            set { ProblemDataParameter.ActualValue = value; }
        }
        #endregion

        #region Constructors
        [StorableConstructor]
        protected SimulationSingleObjectiveTrainingNBestSolutionsAnalyzer(bool deserializing) : base(deserializing) { }
        protected SimulationSingleObjectiveTrainingNBestSolutionsAnalyzer(SimulationSingleObjectiveTrainingNBestSolutionsAnalyzer<T, U, V, Z> original, Cloner cloner) : base(original, cloner) { }
        protected SimulationSingleObjectiveTrainingNBestSolutionsAnalyzer()
            : base()
        {
            Parameters.Add(new ValueParameter<IntValue>(NumTopSolutionsParameterName, "Number of Top Solutions to Show", new IntValue(10)));
            Parameters.Add(new LookupParameter<U>(EvaluatorParameterName, "The operator to use for fitness evaluation."));
            Parameters.Add(new LookupParameter<T>(ProblemDataParameterName, "The problem data for the symbolic Trading solution."));
            Parameters.Add(new LookupParameter<ISymbolicDataAnalysisExpressionTreeInterpreter>(SymbolicDataAnalysisTreeInterpreterParameterName, "The symbolic data analysis tree interpreter for the symbolic expression tree."));
            Parameters.Add(new ValueLookupParameter<DoubleLimit>(EstimationLimitsParameterName, "The lower and upper limit for the estimated values produced by the symbolic Trading model."));
            //Parameters.Add(new ScopeTreeLookupParameter<CompiledSymbolicExpressionTree>(CompiledAgentsParameterName, "The compiled symbolic data analysis solution"));
            Parameters.Add(new LookupParameter<SolutionList<Z>>(TrainingBestSolutionsParameterName, "The training best symbolic data analyis solution."));
            Parameters.Add(new LookupParameter<ItemList<DoubleValue>>(TrainingBestSolutionQualitiesParameterName, "The quality of the training best symbolic data analysis solution."));
            Parameters.Add(new FixedValueParameter<BoolValue>(UpdateAlwaysParameterName, "Determines if the best training solution should always be updated regardless of its quality.", new BoolValue(false)));
            Parameters.Add(new ValueLookupParameter<ISymbolicExpressionGrammar>(SymbolicExpressionGrammarParameterName, "The tree-generating grammar"));
            UpdateAlwaysParameter.Hidden = true;
            SymbolicDataAnalysisTreeInterpreterParameter.ActualName = "SymbolicExpressionTreeInterpreter";
        }

        [StorableHook(HookType.AfterDeserialization)]
        protected void AfterDeserialization()
        {
            if (!Parameters.ContainsKey(UpdateAlwaysParameterName))
            {
                Parameters.Add(new FixedValueParameter<BoolValue>(UpdateAlwaysParameterName, "Determines if the best training solution should always be updated regardless of its quality.", new BoolValue(false)));
                UpdateAlwaysParameter.Hidden = true;
            }
        }
        #endregion

        public override IOperation Apply()
        {
            var results = ResultCollection;

            // create empty parameter and result values
            if (TrainingBestSolutions == null)
            {
                TrainingBestSolutions = new SolutionList<Z>();
                TrainingBestSolutionQualities = new ItemList<DoubleValue>();
                results.Add(new Result(TrainingBestSolutionsParameter.Name, TrainingBestSolutionsParameter.Description, TrainingBestSolutions));
                results.Add(new Result(TrainingBestSolutionQualitiesParameter.Name, TrainingBestSolutionQualitiesParameter.Description, TrainingBestSolutionQualities));
            }

            //find  current better trees
            ISymbolicExpressionTree[] trees = SymbolicExpressionTree.ToArray();
            List<double> qualities = Quality.Select(x => x.Value).ToList();

            Dictionary<int, double> Buffer = InitializeTempBuffer(TrainingBestSolutionQualities);

            for (int i = 0; i < trees.Length; i++)
            {
                if (IsBetter(qualities[i], Buffer))
                    UpdateBuffer(i, qualities[i], Buffer);
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
                    finalBestSolutions.Add(TrainingBestSolutions[k]);
                else
                    finalBestSolutions.Add(CreateSolution(trees[k - NumTopSolutions.Value], Buffer[k]));
            }

            TrainingBestSolutions = finalBestSolutions;
            TrainingBestSolutionQualities = finalBestSolutionQualities;

            if (!results.ContainsKey(TrainingBestSolutionsParameter.Name))
            {
                results.Add(new Result(TrainingBestSolutionsParameter.Name, TrainingBestSolutionsParameter.Description, TrainingBestSolutions));
                results.Add(new Result(TrainingBestSolutionQualitiesParameter.Name, TrainingBestSolutionQualitiesParameter.Description, TrainingBestSolutionQualities));
            }
            else
            {
                results[TrainingBestSolutionsParameter.Name].Value = TrainingBestSolutions;
                results[TrainingBestSolutionQualitiesParameter.Name].Value = TrainingBestSolutionQualities;
            }


            return base.Apply();
        }

        //protected abstract Z CreateSolution(ISymbolicExpressionTree tree, CompiledSymbolicExpressionTree compiledTree, double bestQuality);
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


        private double BestElement(double[] arr)
        {
            if (Maximization.Value) return arr.Max();
            else return arr.Min();
        }
        #endregion
    }
}
