using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GP4Sim.SimulationFramework.Interfaces;
using GP4Sim.SymbolicTrees;
using HeuristicLab.Common;
using HeuristicLab.Data;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Optimization;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.DataAnalysis;
using HeuristicLab.Problems.DataAnalysis.Symbolic;

namespace GP4Sim.SimulationFramework.Solutions
{
    [StorableClass]
    public abstract class SimulationSolution<T, U> : DataAnalysisSolution, ISimulationSolution<T, U>
        where T : class, ISimulationProblemData
        where U : class, ISimulationModel<T>
    {
        //Collections
        public const string TrainingResultsName = "Training Results";
        public const string TestResultsName = "Test Results";


        //Abstract
        protected const string TrainingFitnessScoreResultName = "Fitness Score (Training)";
        protected const string TestFitnessScoreResultName = "Fitness Score (Test)";
        protected const string TrainingFitnessScoreResultDescription = "Fitness Score on the Training Partition";
        protected const string TestFitnessScoreResultDescription = "Fitness Score on the Test Partition";



        //Symbolic
        private const string ModelLengthResultName = "Model Length";
        private const string ModelDepthResultName = "Model Depth";


        public new U Model
        {
            get { return (U)base.Model; }
            protected set { base.Model = value; }
        }

        ISymbolicDataAnalysisModel ISymbolicDataAnalysisSolution.Model
        {
            get { return (ISymbolicDataAnalysisModel)base.Model; }
        }

        public new T ProblemData
        {
            get { return (T)base.ProblemData; }
            set { base.ProblemData = value; }
        }


        #region Results

        //Collections
        public ResultCollection TrainingResultCollection
        {
            get { return (ResultCollection)this[TrainingResultsName].Value; }
        }

        public ResultCollection TestResultCollection
        {
            get { return (ResultCollection)this[TestResultsName].Value; }
        }

        //Abstract
        public double TrainingFitnessScore
        {
            get { return ((DoubleValue)this[TrainingFitnessScoreResultName].Value).Value; }
            private set { ((DoubleValue)this[TrainingFitnessScoreResultName].Value).Value = value; }
        }
        public double TestFitnessScore
        {
            get { return ((DoubleValue)this[TestFitnessScoreResultName].Value).Value; }
            private set { ((DoubleValue)this[TestFitnessScoreResultName].Value).Value = value; }
        }


        //Symbolic
        public int ModelLength
        {
            get { return ((IntValue)this[ModelLengthResultName].Value).Value; }
            private set { ((IntValue)this[ModelLengthResultName].Value).Value = value; }
        }

        public int ModelDepth
        {
            get { return ((IntValue)this[ModelDepthResultName].Value).Value; }
            private set { ((IntValue)this[ModelDepthResultName].Value).Value = value; }
        }


        public List<string> FullInputVector
        {
            get
            {
                List<string> result = new List<string>();
                if (ProblemData.AllowedInputVariables != null)
                    result.AddRange(ProblemData.AllowedInputVariables);
                if (ProblemData.AllowedInputStates != null)
                    result.AddRange(ProblemData.AllowedInputStates);

                return result;
            }
        }

        public List<string> ActualInputVector
        {
            get
            {
                List<string> result = new List<string>();

                IEnumerable<ISymbolicExpressionTreeNode> nodes =  Model.SymbolicExpressionTree.IterateNodesBreadth();
                foreach (ISymbolicExpressionTreeNode node in nodes)
                {
                    if (node.Symbol is LaggedVariable)
                        result.Add("LV " + (node as LaggedVariableTreeNode).VariableName + " "+(node as LaggedVariableTreeNode).Lag);
                    else if (node.Symbol is Variable)
                        result.Add("V " + (node as VariableTreeNode).VariableName + " 0");
                    else if (node.Symbol is InternalState)
                        result.Add("IS " + (node as InternalStateTreeNode).VariableName);
                }

                return result.Distinct().ToList();
            }
        }
        #endregion

        #region Constructors
        [StorableConstructor]
        protected SimulationSolution(bool deserializing) : base(deserializing) { }
        protected SimulationSolution(SimulationSolution<T, U> original, Cloner cloner)
            : base(original, cloner)
        {

        }
        protected SimulationSolution(ISimulationModel<T> model, ISimulationProblemData problemData)
            : base(model, problemData)
        {
            //Abstract

            Add(new Result(TrainingFitnessScoreResultName, TrainingFitnessScoreResultDescription, new DoubleValue()));
            Add(new Result(TestFitnessScoreResultName, TestFitnessScoreResultDescription, new DoubleValue()));
            //Symbolic
            Add(new Result(ModelLengthResultName, "Length of the symbolic Abstract model.", new IntValue()));
            Add(new Result(ModelDepthResultName, "Depth of the symbolic Abstract model.", new IntValue()));

            AddResultCollection(ProblemData.TrainingIndices, TrainingResultsName);
            AddResultCollection(ProblemData.TestIndices, TestResultsName);

            CalculateResults();

        }


        [StorableHook(HookType.AfterDeserialization)]
        private void AfterDeserialization()
        {
            // BackwardsCompatibility3.4

            #region Backwards compatible code, remove with 3.5

            if (!ContainsKey(TrainingResultsName))
            {
                //AddResultCollection(ProblemData.TrainingIndices, TrainingResultsName);

                /*
                ResultCollection estimationLimitResults = new ResultCollection();
                estimationLimitResults.Add(new Result(EstimationLimitsResultName, "", new DoubleLimit()));
                estimationLimitResults.Add(new Result(TrainingUpperEstimationLimitHitsResultName, "", new IntValue()));
                estimationLimitResults.Add(new Result(TestUpperEstimationLimitHitsResultName, "", new IntValue()));
                estimationLimitResults.Add(new Result(TrainingLowerEstimationLimitHitsResultName, "", new IntValue()));
                estimationLimitResults.Add(new Result(TestLowerEstimationLimitHitsResultName, "", new IntValue()));
                estimationLimitResults.Add(new Result(TrainingNaNEvaluationsResultName, "", new IntValue()));
                estimationLimitResults.Add(new Result(TestNaNEvaluationsResultName, "", new IntValue()));
                Add(new Result(EstimationLimitsResultsResultName, "Results concerning the estimation limits of symbolic Trading solution", estimationLimitResults));
                 */
                //CalculateResults();
            }
            #endregion
        }


        #endregion

        protected override void RecalculateResults()
        {
            CalculateResults();
            CalculateTTResults();
        }

        protected void CalculateResults()
        {
            CalculateBaseResults();
            CalculateSymbolicResults();
        }

        protected void CalculateBaseResults()
        {
            TrainingFitnessScore = Math.Round(Model.GetFitnessScore(ProblemData, ProblemData.TrainingIndices), 6);
            TestFitnessScore = Math.Round(Model.GetFitnessScore(ProblemData, ProblemData.TestIndices), 6);
        }

        protected void CalculateSymbolicResults()
        {
            ModelLength = Model.SymbolicExpressionTree.Length;
            ModelDepth = Model.SymbolicExpressionTree.Depth;

        }

        protected void CalculateTTResults()
        {
            RecalculateResultsCollection(ProblemData.TrainingIndices, TrainingResultsName);
            RecalculateResultsCollection(ProblemData.TestIndices, TestResultsName);
        }

        protected void AddResultCollection(IEnumerable<int> indices, string name)
        {
            ResultCollection rc = new ResultCollection();

            foreach (string cat in Model.ResultProperties.Categories)
            {
                ResultCollection catResults = new ResultCollection();

                foreach (string val in Model.ResultProperties.CategoryValues(cat))
                {
                    Type t = Model.ResultProperties.Type(val);

                    dynamic value = Model.GetResult(ProblemData, val, indices);
                    //value = Convert.ChangeType(value, t);
                    if (value is double)
                        catResults.Add(new Result(Model.ResultProperties.VisibleName(val), Model.ResultProperties.Description(val), new DoubleValue(Math.Round(value, 4))));
                    else if (value is long || value is int)
                        catResults.Add(new Result(Model.ResultProperties.VisibleName(val), Model.ResultProperties.Description(val), new IntValue((int)value)));
                    else if (value is IEnumerable<double>)
                        catResults.Add(new Result(Model.ResultProperties.VisibleName(val), Model.ResultProperties.Description(val), new DoubleArray(((List<double>)value).ToArray())));
                }

                rc.Add(new Result(cat, catResults));
            }

            Add(new Result(name, rc));
        }

        protected void RecalculateResultsCollection(IEnumerable<int> indices, string name)
        {
            if (this[name] == null)
                return;
            else
            {
                ResultCollection rc = (ResultCollection)this[name].Value;
                foreach (IResult cat in rc)
                {
                    ResultCollection catResults = cat as ResultCollection;
                    foreach (string val in Model.ResultProperties.CategoryValues(cat.Name))
                    {
                        Type t = Model.ResultProperties.Type(val);
                        dynamic value = Model.GetResult(ProblemData, val, indices);
                        value = Convert.ChangeType(value, t);
                        if (value is double)
                            ((DoubleValue)catResults[Model.ResultProperties.VisibleName(val)].Value).Value = Math.Round(value, 4);
                        else if (value is long)
                            ((IntValue)catResults[Model.ResultProperties.VisibleName(val)].Value).Value = value;

                    }
                }
            }
        }

        //Event Handlers
        protected override void OnProblemDataChanged()
        {
            //evaluationCache.Clear();
            //base.OnProblemDataChanged();
        }

        protected override void OnModelChanged()
        {
            //evaluationCache.Clear();
            base.OnModelChanged();
        }
    }
}
