using System;
using System.Collections.Generic;
using System.Linq;
using GP4Sim.SimulationFramework.Interfaces;
using GP4Sim.SymbolicTrees;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.DataAnalysis;
using HeuristicLab.Problems.DataAnalysis.Symbolic;

namespace HeuristicLab.GP4SimPlugins.Simulation.Inheritance_Tests
{
    [StorableClass]
    public abstract class SimulationProblem<T, U, V> : SymbolicDataAnalysisSingleObjectiveProblem<T, U, V>
        where T : class, ISimulationProblemData
        where U : class, ISimulationEvaluator<T>, ISymbolicDataAnalysisSingleObjectiveEvaluator<T>
        where V : class, ISymbolicDataAnalysisSolutionCreator
    {
        #region Constants

        protected const int InitialMaximumTreeDepth = 8;
        protected const int InitialMaximumTreeLength = 50;
        protected const string EstimationLimitsParameterName = "EstimationLimits";
        protected const string EstimationLimitsParameterDescription = "The upper and lower limits for the value returned by the agent.";
        protected const double InitialEstimationLimit = 100;



        //Voodoo
        private const string ModelCreatorParameterName = "ModelCreator";

        #endregion

        #region Parameter Properties
        public IFixedValueParameter<DoubleLimit> EstimationLimitsParameter
        {
            get { return (IFixedValueParameter<DoubleLimit>)Parameters[EstimationLimitsParameterName]; }
        }

        #endregion

        #region Properties
        public DoubleLimit EstimationLimits
        {
            get { return EstimationLimitsParameter.Value; }
        }

        #endregion

        #region Constructors

        [StorableConstructor]
        protected SimulationProblem(bool deserializing) : base(deserializing) { }


        protected SimulationProblem(SimulationProblem<T, U, V> original, Cloner cloner)
            : base(original, cloner)
        {
            this.RegisterEventHandlers();
            EstimationLimitsParameter.Hidden = false;
            MaximizationParameter.Hidden = false;
            Maximization.Value = false;
            InitializeInterpreterVarMap();
        }

        public SimulationProblem(T problemData, U evaluator, V solutionCreator)
            : base(problemData, evaluator, solutionCreator)
        {
            Parameters.Add(new FixedValueParameter<DoubleLimit>(EstimationLimitsParameterName, EstimationLimitsParameterDescription));

            EstimationLimitsParameter.Hidden = false;
            MaximizationParameter.Hidden = false;
            Maximization.Value = false;
            MaximumSymbolicExpressionTreeDepth.Value = InitialMaximumTreeDepth;
            MaximumSymbolicExpressionTreeLength.Value = InitialMaximumTreeLength;
            EstimationLimits.Upper = InitialEstimationLimit;
            EstimationLimits.Lower = 0;
            SymbolicExpressionTreeGrammar = new TypeCoherentStateExpressionGrammar();
            SymbolicExpressionTreeInterpreter = new SymbolicAbstractTreeInterpreter();
            SymbolicExpressionTreeInterpreterParameter.Hidden = false;

            this.InitializeOperators();
            UpdateGrammar();
            ConfigureGrammarSymbols();
            InitializeInterpreterVarMap();
            this.RegisterEventHandlers();
        }

        //Voodoo
        [StorableHook(HookType.AfterDeserialization)]
        private void AfterDeserialization()
        {
            RegisterEventHandlers();
            if ((SymbolicExpressionTreeInterpreter as SymbolicAbstractTreeInterpreter).StatesVarMap == null)
                InitializeInterpreterVarMap();
        }

        #endregion

        #region Protected Methods

        protected override void OnProblemDataChanged()
        {
            base.OnProblemDataChanged();
        }



        //Voodoo
        protected override void ParameterizeOperators()
        {
            base.ParameterizeOperators();
            if (Parameters.ContainsKey(EstimationLimitsParameterName))
            {
                IEnumerable<IItem> operators = Parameters.OfType<IValueParameter>().Select(p => p.Value).OfType<IOperator>().Union(Operators);
                foreach (ISymbolicDataAnalysisBoundedOperator op in operators.OfType<ISymbolicDataAnalysisBoundedOperator>())
                {
                    op.EstimationLimitsParameter.ActualName = EstimationLimitsParameter.Name;
                }
            }
        }

        protected void RegisterEventHandlers()
        {
            SymbolicExpressionTreeGrammarParameter.ValueChanged += (o, e) => ConfigureGrammarSymbols();
            ProblemDataParameter.Value.Changed += AbstractProblemDataChanged;
            ProblemDataChanged += AbstractProblemDataChanged;
        }

        void AbstractProblemDataChanged(object sender, EventArgs e)
        {
            InitializeInterpreterVarMap();
        }

        protected abstract void ConfigureGrammarSymbols();

        protected override void UpdateGrammar()
        {
            foreach (var stateSymbol in SymbolicExpressionTreeGrammar.Symbols.OfType<InternalState>())
            {
                stateSymbol.AllInternalStateNames = ProblemData.InputStates.Select(x => x.Value);
                stateSymbol.InternalStateNames = ProblemData.AllowedInputStates;
            }

            base.UpdateGrammar();
        }

        protected void InitializeInterpreterVarMap()
        {
            (SymbolicExpressionTreeInterpreter as SymbolicAbstractTreeInterpreter).UpdateInputsMap(ProblemData.AllowedInputVariables, ProblemData.AllowedInputStates);
        }

        private void InitializeOperators()
        {
            ParameterizeOperators();
        }



        #endregion
    }
}
