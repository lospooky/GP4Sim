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
using GP4Sim.SymbolicTrees;

namespace GP4Sim.SimulationFramework.Analyzers
{
    //[Item("SimulationSingleObjectiveValidationBestSolutionAnalyzer", "An operator that analyzes the validation best solution for single objective abstract problems.")]
    [StorableClass]
    public abstract class SimulationSingleObjectiveValidationBestSolutionAnalyzer<T, U, V, Z> : SymbolicDataAnalysisSingleObjectiveValidationAnalyzer<U, T>
        where T : class, ISimulationProblemData
        where U : class, ISimulationSingleObjectiveEvaluator<T>
        where V : class, ISimulationModel<T>
        where Z : class, ISimulationSolution<T, V>
    //where V : class, IAbstractSolution<T, IAbstractModel<T>>
    {

        private const string SymbolicExpressionGrammarParameterName = "SymbolicExpressionTreeGrammar";

        private const string ValidationBestSolutionParameterName = "Best validation solution";
        private const string ValidationBestSolutionQualityParameterName = "Best validation solution quality";
        private const string UpdateAlwaysParameterName = "Always update best solution";
        private const string EstimationLimitsParameterName = "EstimationLimits";
        protected const string CompiledAgentsParameterName = "CompiledAgent";

        #region Parameter Properties
        public ILookupParameter<Z> ValidationBestSolutionParameter
        {
            get { return (ILookupParameter<Z>)Parameters[ValidationBestSolutionParameterName]; }
        }
        public ILookupParameter<DoubleValue> ValidationBestSolutionQualityParameter
        {
            get { return (ILookupParameter<DoubleValue>)Parameters[ValidationBestSolutionQualityParameterName]; }
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
        public Z ValidationBestSolution
        {
            get { return ValidationBestSolutionParameter.ActualValue; }
            set { ValidationBestSolutionParameter.ActualValue = value; }
        }
        public DoubleValue ValidationBestSolutionQuality
        {
            get { return ValidationBestSolutionQualityParameter.ActualValue; }
            set { ValidationBestSolutionQualityParameter.ActualValue = value; }
        }
        public BoolValue UpdateAlways
        {
            get { return UpdateAlwaysParameter.Value; }
        }
        #endregion

        #region Constructors
        [StorableConstructor]
        protected SimulationSingleObjectiveValidationBestSolutionAnalyzer(bool deserializing) : base(deserializing) { }
        protected SimulationSingleObjectiveValidationBestSolutionAnalyzer(SimulationSingleObjectiveValidationBestSolutionAnalyzer<T, U, V, Z> original, Cloner cloner) : base(original, cloner) { }
        public SimulationSingleObjectiveValidationBestSolutionAnalyzer()
            : base()
        {
            Parameters.Add(new ScopeTreeLookupParameter<CompiledSymbolicExpressionTree>(CompiledAgentsParameterName, "The compiled symbolic data analysis solution"));
            Parameters.Add(new ValueLookupParameter<DoubleLimit>(EstimationLimitsParameterName, "The lower and upper limit for the estimated values produced by the symbolic Trading model."));
            Parameters.Add(new LookupParameter<Z>(ValidationBestSolutionParameterName, "The validation best symbolic data analyis solution."));
            Parameters.Add(new LookupParameter<DoubleValue>(ValidationBestSolutionQualityParameterName, "The quality of the validation best symbolic data analysis solution."));
            Parameters.Add(new FixedValueParameter<BoolValue>(UpdateAlwaysParameterName, "Determines if the best validation solution should always be updated regardless of its quality.", new BoolValue(false)));

            Parameters.Add(new ValueLookupParameter<ISymbolicExpressionGrammar>(SymbolicExpressionGrammarParameterName, "The tree-generating grammar"));

            SymbolicDataAnalysisTreeInterpreterParameter.ActualName = "SymbolicExpressionTreeInterpreter";
            UpdateAlwaysParameter.Hidden = true;
        }

        [StorableHook(HookType.AfterDeserialization)]
        private void AfterDeserialization()
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
            var evaluator = EvaluatorParameter.ActualValue;
            var problemData = ProblemDataParameter.ActualValue;

            IEnumerable<int> rows = GenerateRowsToEvaluate();
            if (rows.Any())
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

            int bestValidationTreeIdx = Int32.MinValue;
            bestValidationTreeIdx = Array.IndexOf(validationQualities, BestElement(validationQualities));
            int originalIdx = Array.IndexOf(trees, treesToValidate[bestValidationTreeIdx]);

            #region Update Best Validation Solution
            var results = ResultCollection;
            if (UpdateAlways.Value || ValidationBestSolutionQuality == null ||
              IsBetter(validationQualities[bestValidationTreeIdx], ValidationBestSolutionQuality.Value))
            {
                ValidationBestSolutionQuality = new DoubleValue(qualities[bestValidationTreeIdx]);
                ValidationBestSolution = CreateSolution(trees[originalIdx], validationQualities[bestValidationTreeIdx]);

                if (!results.ContainsKey(ValidationBestSolutionParameter.Name))
                {
                    results.Add(new Result(ValidationBestSolutionParameter.Name, ValidationBestSolutionParameter.Description, ValidationBestSolution));
                    results.Add(new Result(ValidationBestSolutionQualityParameter.Name, ValidationBestSolutionQualityParameter.Description, ValidationBestSolutionQuality));
                }
                else
                {
                    results[ValidationBestSolutionParameter.Name].Value = ValidationBestSolution;
                    results[ValidationBestSolutionQualityParameter.Name].Value = ValidationBestSolutionQuality;
                }
            }
            #endregion

            return base.Apply();
        }

        protected abstract Z CreateSolution(ISymbolicExpressionTree bestTree, double bestQuality);

        private double BestElement(double[] arr)
        {
            if (Maximization.Value) return arr.Max();
            else return arr.Min();
        }

        private bool IsBetter(double lhs, double rhs)
        {
            if (Maximization.Value) return lhs > rhs;
            else return lhs < rhs;
        }
    }
}
