using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.Problems.DataAnalysis.Symbolic;
using HeuristicLab.Data;
using HeuristicLab.Core;
using HeuristicLab.Analysis;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Common;
using HeuristicLab.Parameters;
using HeuristicLab.Optimization;
using GP4Sim.SimulationFramework.Interfaces;

namespace GP4Sim.SimulationFramework.Analyzers
{
    [StorableClass]
    public abstract class SimulationSingleObjectiveOverfittingAnalyzer<T, U> : SymbolicDataAnalysisSingleObjectiveValidationAnalyzer<T, U>
        where T : class, ISimulationSingleObjectiveEvaluator<U>
        where U : class, ISimulationProblemData
    {
        protected const string TrainingValidationCorrelationParameterName = "Training and validation fitness correlation";
        protected const string TrainingValidationCorrelationTableParameterName = "Training and validation fitness correlation table";
        protected const string LowerCorrelationThresholdParameterName = "LowerCorrelationThreshold";
        protected const string UpperCorrelationThresholdParameterName = "UpperCorrelationThreshold";
        protected const string OverfittingParameterName = "IsOverfitting";

        #region Parameter Properties
        public ILookupParameter<DoubleValue> TrainingValidationQualityCorrelationParameter
        {
            get { return (ILookupParameter<DoubleValue>)Parameters[TrainingValidationCorrelationParameterName]; }
        }
        public ILookupParameter<DataTable> TrainingValidationQualityCorrelationTableParameter
        {
            get { return (ILookupParameter<DataTable>)Parameters[TrainingValidationCorrelationTableParameterName]; }
        }
        public IValueLookupParameter<DoubleValue> LowerCorrelationThresholdParameter
        {
            get { return (IValueLookupParameter<DoubleValue>)Parameters[LowerCorrelationThresholdParameterName]; }
        }
        public IValueLookupParameter<DoubleValue> UpperCorrelationThresholdParameter
        {
            get { return (IValueLookupParameter<DoubleValue>)Parameters[UpperCorrelationThresholdParameterName]; }
        }
        public ILookupParameter<BoolValue> OverfittingParameter
        {
            get { return (ILookupParameter<BoolValue>)Parameters[OverfittingParameterName]; }
        }
        #endregion

        [StorableConstructor]
        protected SimulationSingleObjectiveOverfittingAnalyzer(bool deserializing) : base(deserializing) { }
        protected SimulationSingleObjectiveOverfittingAnalyzer(SimulationSingleObjectiveOverfittingAnalyzer<T, U> original, Cloner cloner) : base(original, cloner) { }
        protected SimulationSingleObjectiveOverfittingAnalyzer()
            : base()
        {
            Parameters.Add(new LookupParameter<DoubleValue>(TrainingValidationCorrelationParameterName, "Correlation of training and validation fitnesses"));
            Parameters.Add(new LookupParameter<DataTable>(TrainingValidationCorrelationTableParameterName, "Data table of training and validation fitness correlation values over the whole run."));
            Parameters.Add(new ValueLookupParameter<DoubleValue>(LowerCorrelationThresholdParameterName, "Lower threshold for correlation value that marks the boundary from non-overfitting to overfitting.", new DoubleValue(0.65)));
            Parameters.Add(new ValueLookupParameter<DoubleValue>(UpperCorrelationThresholdParameterName, "Upper threshold for correlation value that marks the boundary from overfitting to non-overfitting.", new DoubleValue(0.75)));
            Parameters.Add(new LookupParameter<BoolValue>(OverfittingParameterName, "Boolean indicator for overfitting."));
        }


        public override IOperation Apply()
        {
            IEnumerable<int> rows = GenerateRowsToEvaluate();
            if (!rows.Any()) return base.Apply();

            double[] trainingQuality = QualityParameter.ActualValue.Select(x => x.Value).ToArray();
            var problemData = ProblemDataParameter.ActualValue;
            var evaluator = EvaluatorParameter.ActualValue;
            // evaluate on validation partition
            IExecutionContext childContext = (IExecutionContext)ExecutionContext.CreateChildOperation(evaluator);
            double[] validationQuality = SymbolicExpressionTree
              .AsParallel()
              .Select(t => evaluator.Evaluate(childContext, t, problemData, rows))
              .ToArray();
            double r = 0.0;
            try
            {
                r = alglib.spearmancorr2(trainingQuality, validationQuality);
            }
            catch (alglib.alglibexception)
            {
                r = 0.0;
            }

            TrainingValidationQualityCorrelationParameter.ActualValue = new DoubleValue(r);

            if (TrainingValidationQualityCorrelationTableParameter.ActualValue == null)
            {
                var dataTable = new DataTable(TrainingValidationQualityCorrelationTableParameter.Name, TrainingValidationQualityCorrelationTableParameter.Description);
                dataTable.Rows.Add(new DataRow(TrainingValidationQualityCorrelationParameter.Name, TrainingValidationQualityCorrelationParameter.Description));
                dataTable.Rows[TrainingValidationQualityCorrelationParameter.Name].VisualProperties.StartIndexZero = true;
                TrainingValidationQualityCorrelationTableParameter.ActualValue = dataTable;
                ResultCollectionParameter.ActualValue.Add(new Result(TrainingValidationQualityCorrelationTableParameter.Name, dataTable));
            }

            TrainingValidationQualityCorrelationTableParameter.ActualValue.Rows[TrainingValidationQualityCorrelationParameter.Name].Values.Add(r);

            if (OverfittingParameter.ActualValue != null && OverfittingParameter.ActualValue.Value)
            {
                // overfitting == true
                // => r must reach the upper threshold to switch back to non-overfitting state
                OverfittingParameter.ActualValue = new BoolValue(r < UpperCorrelationThresholdParameter.ActualValue.Value);
            }
            else
            {
                // overfitting == false 
                // => r must drop below lower threshold to switch to overfitting state
                OverfittingParameter.ActualValue = new BoolValue(r < LowerCorrelationThresholdParameter.ActualValue.Value);
            }

            return base.Apply();
        }
    }
}
