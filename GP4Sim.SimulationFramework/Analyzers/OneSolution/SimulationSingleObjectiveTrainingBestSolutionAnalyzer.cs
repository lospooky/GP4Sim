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
    //NOT TO BE REGISTERED
    //[Item("SimulationSingleObjectiveTrainingBestSolutionAnalyzer", "An operator that analyzes the training best abstract solution for single objective symbolic data analysis problems.")]
    [StorableClass]
    public abstract class SimulationSingleObjectiveTrainingBestSolutionAnalyzer<T, U, V, Z> : SymbolicDataAnalysisSingleObjectiveAnalyzer, ISymbolicDataAnalysisInterpreterOperator, ISymbolicDataAnalysisBoundedOperator
        where T : class, ISimulationProblemData
        where U : class, ISimulationSingleObjectiveEvaluator<T>
        where V : class, ISimulationModel<T>
        where Z : class, ISimulationSolution<T, V>
    {
        protected const string TrainingBestSolutionParameterName = "Best training solution";
        protected const string TrainingBestSolutionQualityParameterName = "Best training solution quality";
        protected const string UpdateAlwaysParameterName = "Always update best solution";
        protected const string ProblemDataParameterName = "ProblemData";
        protected const string SymbolicDataAnalysisTreeInterpreterParameterName = "SymbolicDataAnalysisTreeInterpreter";
        protected const string EstimationLimitsParameterName = "EstimationLimits";
        protected const string CompiledAgentsParameterName = "CompiledAgent";
        protected const string EvaluatorParameterName = "Evaluator";
        private const string SymbolicExpressionGrammarParameterName = "SymbolicExpressionTreeGrammar";

        #region Parameter Properties
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
        public ILookupParameter<Z> TrainingBestSolutionParameter
        {
            get { return (ILookupParameter<Z>)Parameters[TrainingBestSolutionParameterName]; }
        }
        public ILookupParameter<DoubleValue> TrainingBestSolutionQualityParameter
        {
            get { return (ILookupParameter<DoubleValue>)Parameters[TrainingBestSolutionQualityParameterName]; }
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
        public Z TrainingBestSolution
        {
            get { return TrainingBestSolutionParameter.ActualValue; }
            set { TrainingBestSolutionParameter.ActualValue = value; }
        }
        public DoubleValue TrainingBestSolutionQuality
        {
            get { return TrainingBestSolutionQualityParameter.ActualValue; }
            set { TrainingBestSolutionQualityParameter.ActualValue = value; }
        }
        public BoolValue UpdateAlways
        {
            get { return UpdateAlwaysParameter.Value; }
        }
        #endregion

        #region Constructors
        [StorableConstructor]
        protected SimulationSingleObjectiveTrainingBestSolutionAnalyzer(bool deserializing) : base(deserializing) { }
        protected SimulationSingleObjectiveTrainingBestSolutionAnalyzer(SimulationSingleObjectiveTrainingBestSolutionAnalyzer<T, U, V, Z> original, Cloner cloner) : base(original, cloner) { }
        protected SimulationSingleObjectiveTrainingBestSolutionAnalyzer()
            : base()
        {
            Parameters.Add(new LookupParameter<U>(EvaluatorParameterName, "The operator to use for fitness evaluation."));
            Parameters.Add(new LookupParameter<T>(ProblemDataParameterName, "The problem data for the symbolic Trading solution."));
            Parameters.Add(new LookupParameter<ISymbolicDataAnalysisExpressionTreeInterpreter>(SymbolicDataAnalysisTreeInterpreterParameterName, "The symbolic data analysis tree interpreter for the symbolic expression tree."));
            Parameters.Add(new ValueLookupParameter<DoubleLimit>(EstimationLimitsParameterName, "The lower and upper limit for the estimated values produced by the symbolic Trading model."));
            Parameters.Add(new ScopeTreeLookupParameter<CompiledSymbolicExpressionTree>(CompiledAgentsParameterName, "The compiled symbolic data analysis solution"));
            Parameters.Add(new LookupParameter<Z>(TrainingBestSolutionParameterName, "The training best symbolic data analyis solution."));
            Parameters.Add(new LookupParameter<DoubleValue>(TrainingBestSolutionQualityParameterName, "The quality of the training best symbolic data analysis solution."));
            Parameters.Add(new FixedValueParameter<BoolValue>(UpdateAlwaysParameterName, "Determines if the best training solution should always be updated regardless of its quality.", new BoolValue(false)));
            Parameters.Add(new ValueLookupParameter<ISymbolicExpressionGrammar>(SymbolicExpressionGrammarParameterName, "The tree-generating grammar"));
            UpdateAlwaysParameter.Hidden = true;
            SymbolicDataAnalysisTreeInterpreterParameter.ActualName = "SymbolicExpressionTreeInterpreter";
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
            #region Find Best Tree

            double bestQuality = Maximization.Value ? double.NegativeInfinity : double.PositiveInfinity;

            ISymbolicExpressionTree[] trees = SymbolicExpressionTree.ToArray();
            //CompiledSymbolicExpressionTree[] compiledAgents = CompiledAgents.ToArray();
            double[] qualities = Quality.Select(x => x.Value).ToArray();

            int bestTreeIdx = Int32.MinValue;
            bestTreeIdx = Array.IndexOf(qualities, BestElement(qualities));

            #endregion

            #region Update Solution Object

            var results = ResultCollection;
            if (bestTreeIdx != Int32.MinValue &&
                (UpdateAlways.Value || TrainingBestSolutionQuality == null || IsBetter(qualities[bestTreeIdx], TrainingBestSolutionQuality.Value)))
            {
                TrainingBestSolutionQuality = new DoubleValue(qualities[bestTreeIdx]);
                TrainingBestSolution = CreateSolution(trees[bestTreeIdx], qualities[bestTreeIdx]);

                if (!results.ContainsKey(TrainingBestSolutionParameter.Name))
                {
                    results.Add(new Result(TrainingBestSolutionParameter.Name, TrainingBestSolutionParameter.Description, TrainingBestSolution));
                    results.Add(new Result(TrainingBestSolutionQualityParameter.Name, TrainingBestSolutionQualityParameter.Description, TrainingBestSolutionQuality));
                }
                else
                {
                    results[TrainingBestSolutionParameter.Name].Value = TrainingBestSolution;
                    results[TrainingBestSolutionQualityParameter.Name].Value = TrainingBestSolutionQuality;
                }
            }

            #endregion

            return base.Apply();
        }

        //protected abstract Z CreateSolution(ISymbolicExpressionTree tree, CompiledSymbolicExpressionTree compiledTree, double bestQuality);
        protected abstract Z CreateSolution(ISymbolicExpressionTree tree, double bestQuality);

        private bool IsBetter(double lhs, double rhs)
        {
            if (Maximization.Value) return lhs > rhs;
            else return lhs < rhs;
        }

        private double BestElement(double[] arr)
        {
            if (Maximization.Value) return arr.Max();
            else return arr.Min();
        }
    }
}
