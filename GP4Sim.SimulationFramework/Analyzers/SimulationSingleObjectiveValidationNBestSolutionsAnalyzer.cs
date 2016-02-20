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
using GP4Sim.SymbolicTrees;

namespace GP4Sim.SimulationFramework.Analyzers
{
    [Item("SimulationSingleObjectiveValidationNBestSolutionsAnalyzer", "An operator that analyzes the validation best solution for single objective abstract problems.")]
    [StorableClass]
    public abstract class SimulationSingleObjectiveValidationNBestSolutionsAnalyzer<T, U, V, Z> : SymbolicDataAnalysisSingleObjectiveValidationAnalyzer<U, T>
        where T : class, ISimulationProblemData
        where U : class, ISimulationSingleObjectiveEvaluator<T>
        where V : class, ISimulationModel<T>
        where Z : class, ISimulationSolution<T, V>
    //where V : class, IAbstractSolution<T, IAbstractModel<T>>
    {

        protected const string NumTopSolutionsParameterName = "Number of Top Solutions to Show";
        private const string SymbolicExpressionGrammarParameterName = "SymbolicExpressionTreeGrammar";
        private const string ValidationBestSolutionsParameterName = "Best validation solution";
        private const string ValidationBestSolutionQualitiesParameterName = "Best validation solution quality";
        private const string UpdateAlwaysParameterName = "Always update best solution";
        private const string EstimationLimitsParameterName = "EstimationLimits";
        protected const string CompiledAgentsParameterName = "CompiledAgent";

        #region Parameter Properties
        public IValueParameter<IntValue> NumTopSolutionsParameter
        {
            get { return (IValueParameter<IntValue>)Parameters[NumTopSolutionsParameterName]; }
        }
        public ILookupParameter<SolutionList<Z>> ValidationBestSolutionsParameter
        {
            get { return (ILookupParameter<SolutionList<Z>>)Parameters[ValidationBestSolutionsParameterName]; }
        }
        public ILookupParameter<ItemList<DoubleValue>> ValidationBestSolutionQualitiesParameter
        {
            get { return (ILookupParameter<ItemList<DoubleValue>>)Parameters[ValidationBestSolutionQualitiesParameterName]; }
        }
        public IFixedValueParameter<BoolValue> UpdateAlwaysParameter
        {
            get { return (IFixedValueParameter<BoolValue>)Parameters[UpdateAlwaysParameterName]; }
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
        public ILookupParameter<ISymbolicExpressionGrammar> SymbolicExpressionGrammarParameter
        {
            get { return (ILookupParameter<ISymbolicExpressionGrammar>)Parameters[SymbolicExpressionGrammarParameterName]; }
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
        public SolutionList<Z> ValidationBestSolutions
        {
            get { return ValidationBestSolutionsParameter.ActualValue; }
            set { ValidationBestSolutionsParameter.ActualValue = value; }
        }
        public ItemList<DoubleValue> ValidationBestSolutionQualities
        {
            get { return ValidationBestSolutionQualitiesParameter.ActualValue; }
            set { ValidationBestSolutionQualitiesParameter.ActualValue = value; }
        }
        public BoolValue UpdateAlways
        {
            get { return UpdateAlwaysParameter.Value; }
        }
        #endregion

        #region Constructors
        [StorableConstructor]
        protected SimulationSingleObjectiveValidationNBestSolutionsAnalyzer(bool deserializing) : base(deserializing) { }
        protected SimulationSingleObjectiveValidationNBestSolutionsAnalyzer(SimulationSingleObjectiveValidationNBestSolutionsAnalyzer<T, U, V, Z> original, Cloner cloner) : base(original, cloner) { }
        public SimulationSingleObjectiveValidationNBestSolutionsAnalyzer()
            : base()
        {
            Parameters.Add(new ValueParameter<IntValue>(NumTopSolutionsParameterName, "Number of Top Solutions to Show."));
            Parameters.Add(new ScopeTreeLookupParameter<CompiledSymbolicExpressionTree>(CompiledAgentsParameterName, "The compiled symbolic data analysis solution"));
            Parameters.Add(new ValueLookupParameter<DoubleLimit>(EstimationLimitsParameterName, "The lower and upper limit for the estimated values produced by the symbolic Trading model."));
            Parameters.Add(new LookupParameter<SolutionList<Z>>(ValidationBestSolutionsParameterName, "The validation best symbolic data analyis solution."));
            Parameters.Add(new LookupParameter<ItemList<DoubleValue>>(ValidationBestSolutionQualitiesParameterName, "The quality of the validation best symbolic data analysis solution."));
            Parameters.Add(new FixedValueParameter<BoolValue>(UpdateAlwaysParameterName, "Determines if the best validation solution should always be updated regardless of its quality.", new BoolValue(false)));

            Parameters.Add(new ValueLookupParameter<ISymbolicExpressionGrammar>(SymbolicExpressionGrammarParameterName, "The tree-generating grammar"));

            SymbolicDataAnalysisTreeInterpreterParameter.ActualName = "SymbolicExpressionTreeInterpreter";
            UpdateAlwaysParameter.Hidden = true;
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

            if (ValidationBestSolutions == null)
            {
                ValidationBestSolutions = new SolutionList<Z>();
                ValidationBestSolutionQualities = new ItemList<DoubleValue>();
                results.Add(new Result(ValidationBestSolutionsParameter.Name, ValidationBestSolutionsParameter.Description, ValidationBestSolutions));
                results.Add(new Result(ValidationBestSolutionQualitiesParameter.Name, ValidationBestSolutionQualitiesParameter.Description, ValidationBestSolutionQualities));
            }

            var evaluator = EvaluatorParameter.ActualValue;
            var problemData = ProblemDataParameter.ActualValue;

            IEnumerable<int> rows = GenerateRowsToEvaluate();
            if (!rows.Any())
                return base.Apply();

            #region Find Best N Training Trees

            double[] qualities = Quality.Select(x => x.Value).ToArray();
            ISymbolicExpressionTree[] trees = SymbolicExpressionTree.ToArray();
            //CompiledSymbolicExpressionTree[] compiledAgents = CompiledAgents.ToArray();

            int topN = (int)Math.Max(trees.Length * PercentageOfBestSolutionsParameter.ActualValue.Value, 1);

            var topIndexes = (from q in qualities
                              orderby q descending
                              select Array.IndexOf(qualities, q));
            if (!Maximization.Value)
                topIndexes = topIndexes.Reverse();

            int[] topIndexesArray = topIndexes.Take(topN).ToArray();

            #endregion


            #region Validation Set Evaluation

            ISymbolicExpressionTree[] treesToValidate = new ISymbolicExpressionTree[topIndexesArray.Length];
            for (int i = 0; i < topIndexesArray.Length; i++)
                treesToValidate[i] = trees[topIndexesArray[i]];

            IExecutionContext childContext = (IExecutionContext)ExecutionContext.CreateChildOperation(evaluator);
            //double[] validationQualities = treesToValidate.AsParallel().Select(t => evaluator.Evaluate(childContext, t, problemData, rows)).ToArray();

            double[] validationQualities = treesToValidate.AsParallel().Select(t => evaluator.Evaluate(childContext, t, problemData, rows)).ToArray();

            #endregion


            #region Find Better Current Validation Trees
            Dictionary<int, double> Buffer = InitializeTempBuffer(ValidationBestSolutionQualities);

            for (int i = 0; i < treesToValidate.Length; i++)
            {
                if (IsBetter(validationQualities[i], Buffer))
                    UpdateBuffer(i, validationQualities[i], Buffer);
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
                    finalBestSolutions.Add(ValidationBestSolutions[k]);
                else
                    finalBestSolutions.Add(CreateSolution(treesToValidate[k - NumTopSolutions.Value], Buffer[k]));
            }

            ValidationBestSolutions = finalBestSolutions;
            ValidationBestSolutionQualities = finalBestSolutionQualities;

            if (!results.ContainsKey(ValidationBestSolutionsParameter.Name))
            {
                results.Add(new Result(ValidationBestSolutionsParameter.Name, ValidationBestSolutionsParameter.Description, ValidationBestSolutions));
                results.Add(new Result(ValidationBestSolutionQualitiesParameter.Name, ValidationBestSolutionQualitiesParameter.Description, ValidationBestSolutionQualities));
            }
            else
            {
                results[ValidationBestSolutionsParameter.Name].Value = ValidationBestSolutions;
                results[ValidationBestSolutionQualitiesParameter.Name].Value = ValidationBestSolutionQualities;
            }

            #endregion
            /*
            int bestValidationTreeIdx = Int32.MinValue;
            bestValidationTreeIdx = Array.IndexOf(validationQualities, BestElement(validationQualities));
            int originalIdx = Array.IndexOf(trees, treesToValidate[bestValidationTreeIdx]);

            #region Update Best Validation Solution
            var results = ResultCollection;
            if (UpdateAlways.Value || ValidationBestSolutionQualities == null ||
              IsBetter(validationQualities[bestValidationTreeIdx], ValidationBestSolutionQualities.Value))
            {
                ValidationBestSolutionQualities = new DoubleValue(qualities[bestValidationTreeIdx]);
                ValidationBestSolutions = CreateSolution(trees[originalIdx], validationQualities[bestValidationTreeIdx]);

                if (!results.ContainsKey(ValidationBestSolutionsParameter.Name))
                {
                    results.Add(new Result(ValidationBestSolutionsParameter.Name, ValidationBestSolutionsParameter.Description, ValidationBestSolutions));
                    results.Add(new Result(ValidationBestSolutionQualitiesParameter.Name, ValidationBestSolutionQualitiesParameter.Description, ValidationBestSolutionQualities));
                }
                else
                {
                    results[ValidationBestSolutionsParameter.Name].Value = ValidationBestSolutions;
                    results[ValidationBestSolutionQualitiesParameter.Name].Value = ValidationBestSolutionQualities;
                }
            }
            #endregion

            */

            return base.Apply();
        }

        protected abstract Z CreateSolution(ISymbolicExpressionTree bestTree, double bestQuality);

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
