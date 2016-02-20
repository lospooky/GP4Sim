using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using HeuristicLab.Common;
using System.Reflection;
using GP4Sim.SimulationFramework.Solutions;
using GP4Sim.Trading.Interfaces;
using GP4Sim.Data;
using GP4Sim.SymbolicTrees;
using HeuristicLab.Problems.DataAnalysis;

namespace GP4Sim.Trading.Solutions
{
    [StorableClass]
    public sealed class TradingSolution : SimulationSolution<ITradingProblemData, ITradingModel>, ITradingSolution
    {
        #region Time Ranges
        private const string TrainingPeriodName = "Training Period";
        private const string TestPeriodName = "Test Period";

        public DateTimeRange TrainingPeriod
        {
            get { return ((DateTimeRange)this[TrainingPeriodName].Value); }
        }

        public DateTimeRange TestPeriod
        {
            get { return ((DateTimeRange)this[TestPeriodName].Value); }
        }
        #endregion

        #region Monte Carlo Results
        private const string MonteCarloResultsName = "Monte Carlo Results";

        public ResultCollection MonteCarloResultCollection
        {
            get
            {
                if (this.ContainsKey(MonteCarloResultsName))
                    return (ResultCollection)this[MonteCarloResultsName].Value;
                else
                    return null;
            }
        }
        #endregion

        #region Constructors
        [StorableConstructor]
        private TradingSolution(bool deserializing) : base(deserializing) { }
        private TradingSolution(TradingSolution original, Cloner cloner)
            : base(original, cloner)
        {
        }
        public TradingSolution(ITradingModel model, ITradingProblemData problemData)
            : base(model, problemData)
        {
            string tpVariable = problemData.TimePointVariable;
            Add(new Result(TrainingPeriodName, "", new DateTimeRange(problemData.Dataset.GetDateTimeValue(tpVariable, problemData.TrainingPartition.Start), problemData.Dataset.GetDateTimeValue(tpVariable, problemData.TrainingPartition.End))));
            Add(new Result(TestPeriodName, "", new DateTimeRange(problemData.Dataset.GetDateTimeValue(tpVariable, problemData.TestPartition.Start), problemData.Dataset.GetDateTimeValue(tpVariable, problemData.TestPartition.End - 1))));

            this.name = FitnessName();

        }

        [StorableHook(HookType.AfterDeserialization)]
        private void AfterDeserialization()
        {
            if ((Model.Interpreter as SymbolicAbstractTreeInterpreter).StatesVarMap == null)
                (Model.Interpreter as SymbolicAbstractTreeInterpreter).UpdateInputsMap(ProblemData.AllowedInputVariables, ProblemData.AllowedInputStates);
        }

        public override IDeepCloneable Clone(Cloner cloner)
        {
            return new TradingSolution(this, cloner);
        }
        #endregion

        protected override void RecalculateResults()
        {
            base.RecalculateResults();
        }

        private static string CleanPriceVariableName(string vName)
        {
            if (vName.Contains('_'))
                return vName.Split(new char[] { '_' }, 1)[0];
            else
                return vName;
        }

        public string DescriptiveName
        {
            get
            {
                string pv = CleanPriceVariableName(ProblemData.PriceVariable);
                DateTime start = ProblemData.Dataset.GetDateTimeValue(ProblemData.TimePointVariable, ProblemData.TrainingPartition.Start);
                DateTime end = ProblemData.Dataset.GetDateTimeValue(ProblemData.TimePointVariable, ProblemData.TrainingPartition.End);

                return pv + "_" + start.ToString("MM-dd") + "_" + end.ToString("MM-dd");
            }
        }

        private string FitnessName()
        {
            return "Fitness: " + this.TrainingFitnessScore.ToString("F5");
        }

        public void PerformMonteCarloEvaluation(List<ITradingProblemData> mcSets)
        {
            ResultCollection mcResults;
            if (!this.ContainsKey(MonteCarloResultsName))
            {
                mcResults = new ResultCollection();
                Add(new Result(MonteCarloResultsName, mcResults));
            }
            else
                mcResults = MonteCarloResultCollection;


            foreach (ITradingProblemData mcProblem in mcSets)
                mcResults.Add(new Result(mcProblem.Name, Model.GetMCResult(mcProblem)));

        }
    }
}
