using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Core;
using HeuristicLab.Common;
using HeuristicLab.Problems.DataAnalysis;
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HeuristicLab.Collections;
using System.Collections;
using GP4Sim.SimulationFramework.Problem;
using GP4Sim.Data;
using GP4Sim.Trading.Interfaces;
using GP4Sim.Trading.Parameters;
using GP4Sim.Trading.MonteCarlo;
using System.Windows.Forms;
using System.Threading.Tasks;

namespace GP4Sim.Trading.Problem
{
    [StorableClass]
    [Item("Trading Problem Data", "Represents an item containing all data defining a Trading problem.")]
    public class TradingProblemData : SimulationProblemData, ITradingProblemData
    {

        #region Parameter Names

        protected const string TimePointVariableName = "TimePoint Variable";
        protected const string PriceVariableParameterName = "Price Variable";
        protected const string SimulationParametersParameterName = "Simulation Parameters";
        protected const string InvertedPriceVariableName = "Invert Price";
        protected const string MonteCarloSettingsParameterName = "Monte Carlo Settings";

        protected const string TrainingMaxReturnParameterName = "Maximum Training Return";
        protected const string TestMaxReturnParameterName = "Maximum Test Return";

        private const string LastSeedParameterName = "Last Seed";

        private object lockobject = new object();

        [NonSerialized]
        private List<ITradingProblemData> mcSets;

        #endregion

        #region Parameters

        public IValueParameter<TradingSimulationParameters> SimulationParametersParameter
        {
            get { return (IValueParameter<TradingSimulationParameters>)Parameters[SimulationParametersParameterName]; }
        }

        public IValueParameter<StringValue> PriceVariableParameter
        {
            get { return (IValueParameter<StringValue>)Parameters[PriceVariableParameterName]; }
        }

        public IValueParameter<StringValue> TimePointVariableParameter
        {
            get { return (IValueParameter<StringValue>)Parameters[TimePointVariableName]; }
        }

        public IValueParameter<BoolValue> InvertPriceParameter
        {
            get { return (IValueParameter<BoolValue>)Parameters[InvertedPriceVariableName]; }
        }

        public IValueParameter<MonteCarloParameters> MonteCarloSettingsParameter
        {
            get { return (IValueParameter<MonteCarloParameters>)Parameters[MonteCarloSettingsParameterName]; }
        }

        public IValueParameter<IntValue> LastSeedParameter
        {
            get { return (IValueParameter<IntValue>)Parameters[LastSeedParameterName]; }
        }

        
        public IValueParameter<DoubleValue> TrainingMaxReturnParameter
        {
            get { return (IValueParameter<DoubleValue>)Parameters[TrainingMaxReturnParameterName]; }
        }

        public IValueParameter<DoubleValue> TestMaxReturnParameter
        {
            get { return (IValueParameter<DoubleValue>)Parameters[TestMaxReturnParameterName]; }
        }
        


        #endregion

        #region Properties
        public TradingSimulationParameters SimulationParameters
        {
            get { return SimulationParametersParameter.Value; }
            set { SimulationParametersParameter.Value = value; }
        }

        public string PriceVariable
        {
            get { return PriceVariableParameter.Value.Value; }
        }

        public string TimePointVariable
        {
            get { return TimePointVariableParameter.Value.Value; }
        }

        public bool InvertPrices
        {
            get { return InvertPriceParameter.Value.Value; }
            set { InvertPriceParameter.Value.Value = value; }
        }

        public MonteCarloParameters MonteCarloSettings
        {
            get { return MonteCarloSettingsParameter.Value; }
            set { MonteCarloSettingsParameter.Value = value; }
        }

        public int LastSeed
        {
            get { return LastSeedParameter.Value.Value; }
            set { LastSeedParameter.Value.Value = value; }
        }

        public bool MonteCarlo
        {
            get { return MonteCarloSettings.Enabled; }
        }

        public double TrainingMaxReturn
        {
            get {


                lock (lockobject)
                {
                    if (double.IsNaN(TrainingMaxReturnParameter.Value.Value))
                        CalculateMaxReturn(TrainingPartition);
                }
                    return TrainingMaxReturnParameter.Value.Value;
                }

            set { TrainingMaxReturnParameter.Value.Value = value; }
        }

        public double TestMaxReturn
        {
            get {
                    if(double.IsNaN(TestMaxReturnParameter.Value.Value))
                        CalculateMaxReturn(TestPartition);
                    return TestMaxReturnParameter.Value.Value;
                }
            set { TestMaxReturnParameter.Value.Value = value; }
        }

        #endregion

        #region Default Data
        private static List<IList> DefaultData
        {
            get
            {

                List<IList> dataList = new List<IList>();
                List<double> xList = new List<double> { 1.0, 1.5, 2.0, 2.5, 3.0, 3.5, 4.0 };
                List<double> yList = new List<double> { 10.0, 20.0, 30.0, 40.0, 50.0, 60.0, 70.0 };
                List<DateTime> dateList = new List<DateTime> { DateTime.Today, DateTime.Today, DateTime.Today, DateTime.Today, DateTime.Today, DateTime.Today, DateTime.Today };
                dataList.Add(xList);
                dataList.Add(yList);
                dataList.Add(dateList);

                return dataList;
            }
        }

        protected static readonly Dataset defaultDataset;
        protected static readonly IEnumerable<string> defaultAllowedInputVariables;
        protected static readonly string defaultPriceVariable;
        protected static readonly string defaultTimePointVariable;

        protected static readonly TradingProblemData emptyProblemData;

        protected override List<string> InternalStatusNames
        {
            get
            {
                return new List<string> { "PosAvgCost", "PosAsNavPerc", "PosQty" };
            }
        }

        static TradingProblemData()
        {
            defaultDataset = new Dataset(new string[] { "y", "x", "DATE" }, DefaultData);
            defaultDataset.Name = "Fourth-order Polynomial Function Benchmark Dataset";
            defaultDataset.Description = "f(x) = x^4 + x^3 + x^2 + x^1";
            defaultAllowedInputVariables = new List<string>() { "x" };
            defaultPriceVariable = "x";
            defaultTimePointVariable = "Date";


            var problemData = new TradingProblemData();
            problemData.Parameters.Clear();
            problemData.Name = "Empty Trading ProblemData";
            problemData.Description = "This ProblemData acts as place holder before the correct problem data is loaded.";
            problemData.isEmpty = true;

            problemData.Parameters.Add(new FixedValueParameter<Dataset>(DatasetParameterName, "", new Dataset()));
            problemData.Parameters.Add(new FixedValueParameter<ReadOnlyCheckedItemList<StringValue>>(InputVariablesParameterName, ""));
            problemData.Parameters.Add(new FixedValueParameter<IntRange>(TrainingPartitionParameterName, "", (IntRange)new IntRange(0, 0).AsReadOnly()));
            problemData.Parameters.Add(new FixedValueParameter<IntRange>(TestPartitionParameterName, "", (IntRange)new IntRange(0, 0).AsReadOnly()));
            problemData.Parameters.Add(new ConstrainedValueParameter<StringValue>(PriceVariableParameterName, new ItemSet<StringValue>()));
            problemData.Parameters.Add(new ConstrainedValueParameter<StringValue>(TimePointVariableName, new ItemSet<StringValue>()));
            problemData.Parameters.Add(new ValueParameter<BoolValue>(InvertedPriceVariableName, new BoolValue(false)));
            problemData.Parameters.Add(new ValueParameter<MonteCarloParameters>(MonteCarloSettingsParameterName, new MonteCarloParameters()));
            problemData.Parameters.Add(new ValueParameter<IntValue>(LastSeedParameterName, new IntValue(0)));
            problemData.Parameters.Add(new ValueParameter<DoubleValue>(TrainingMaxReturnParameterName, new DoubleValue(double.NaN)));
            problemData.Parameters.Add(new ValueParameter<DoubleValue>(TestMaxReturnParameterName, new DoubleValue(double.NaN)));

            

            emptyProblemData = problemData;
        }

        #endregion

        #region Constructors

        [StorableConstructor]
        protected TradingProblemData(bool deserializing) : base(deserializing) { }
        [StorableHook(HookType.AfterDeserialization)]
        private void AfterDeserialization()
        {
            if (!Parameters.ContainsKey(InvertedPriceVariableName))
                Parameters.Add(new ValueParameter<BoolValue>(InvertedPriceVariableName, new BoolValue(false)));

            if (!Parameters.ContainsKey(MonteCarloSettingsParameterName))
                Parameters.Add(new ValueParameter<MonteCarloParameters>(MonteCarloSettingsParameterName, new MonteCarloParameters(TrainingPartition.Size)));


            if (!Parameters.ContainsKey(LastSeedParameterName))
            {
                Parameters.Add(new ValueParameter<IntValue>(LastSeedParameterName, new IntValue(0)));
                
            }

            if (!Parameters.ContainsKey(TrainingMaxReturnParameterName))
            {
                Parameters.Add(new ValueParameter<DoubleValue>(TrainingMaxReturnParameterName, new DoubleValue(double.NaN)));
                
            }

            if (!Parameters.ContainsKey(TestMaxReturnParameterName))
            {
                Parameters.Add(new ValueParameter<DoubleValue>(TestMaxReturnParameterName, new DoubleValue(double.NaN)));
                
            }

            if (lockobject == null)
                lockobject = new object();

            LastSeedParameter.Hidden = true;
            TrainingMaxReturnParameter.Hidden = true;
            TestMaxReturnParameter.Hidden = true;
            RegisterParameterEvents();
        }
        protected TradingProblemData(TradingProblemData original, Cloner cloner)
            : base(original, cloner)
        {
            RegisterParameterEvents();
        }

        public override IDeepCloneable Clone(Cloner cloner)
        {
            if (this == emptyProblemData) return emptyProblemData;
            return new TradingProblemData(this, cloner);
        }

        public TradingProblemData()
            : this(defaultDataset, defaultAllowedInputVariables, defaultPriceVariable, defaultTimePointVariable)
        {

            if (lockobject == null)
                lockobject = new object();
        }

        public TradingProblemData(ITradingProblemData ConcreteProblemData)
            : this((Dataset)ConcreteProblemData.Dataset, ConcreteProblemData.AllowedInputVariables, ConcreteProblemData.PriceVariable, ConcreteProblemData.TimePointVariable)
        {
            base.SetPartitions(ConcreteProblemData);
            if (!Parameters.ContainsKey(InvertedPriceVariableName))
                Parameters.Add(new ValueParameter<BoolValue>(InvertedPriceVariableName, new BoolValue(false)));

            if (!Parameters.ContainsKey(MonteCarloSettingsParameterName))
                Parameters.Add(new ValueParameter<MonteCarloParameters>(MonteCarloSettingsParameterName, new MonteCarloParameters(TrainingPartition.Size)));

            if (!Parameters.ContainsKey(LastSeedParameterName))
            {
                Parameters.Add(new ValueParameter<IntValue>(LastSeedParameterName, new IntValue(0)));
                LastSeedParameter.Hidden = true;
            }

            if (!Parameters.ContainsKey(TrainingMaxReturnParameterName))
            {
                Parameters.Add(new ValueParameter<DoubleValue>(TrainingMaxReturnParameterName, new DoubleValue(double.NaN)));
                TrainingMaxReturnParameter.Hidden = true;
            }
            if (!Parameters.ContainsKey(TestMaxReturnParameterName))
            {
                Parameters.Add(new ValueParameter<DoubleValue>(TestMaxReturnParameterName, new DoubleValue(double.NaN)));
                TestMaxReturnParameter.Hidden = true;
            }

            if (lockobject == null)
                lockobject = new object();

            RegisterParameterEvents();
        }

        public TradingProblemData(Dataset dataset, IEnumerable<string> allowedInputVariables, string priceVariable, string timePointVariable)
            : base(dataset, allowedInputVariables)
        {

            //This Already Selects Double Variables ONLY!, In DataAnalysisProblemData.cs
            //Input Variables
            var doubleVariables = InputVariables.Select(x => x.AsReadOnly()).ToList();
            Parameters.Add(new ConstrainedValueParameter<StringValue>(PriceVariableParameterName, new ItemSet<StringValue>(doubleVariables), doubleVariables.Where(x => x.Value == priceVariable).First()));
            //DateTime Variables
            List<string> dtv = ((Dataset)Dataset).DateTimeVariables().ToList();
            var dateTimeVariables =dtv.Select(x => new StringValue(x));

            

            Parameters.Add(new ConstrainedValueParameter<StringValue>(TimePointVariableName, new ItemSet<StringValue>(dateTimeVariables), dateTimeVariables.First()));
            Parameters.Add(new ValueParameter<TradingSimulationParameters>(SimulationParametersParameterName, new TradingSimulationParameters()));
            Parameters.Add(new ValueParameter<BoolValue>(InvertedPriceVariableName, new BoolValue(false)));

            Parameters.Add(new ValueParameter<MonteCarloParameters>(MonteCarloSettingsParameterName, new MonteCarloParameters(TrainingPartition.Size)));
            Parameters.Add(new ValueParameter<IntValue>(LastSeedParameterName, new IntValue(0)));
            Parameters.Add(new ValueParameter<DoubleValue>(TrainingMaxReturnParameterName, new DoubleValue(double.NaN)));
            Parameters.Add(new ValueParameter<DoubleValue>(TestMaxReturnParameterName, new DoubleValue(double.NaN)));
            LastSeedParameter.Hidden = true;
            TrainingMaxReturnParameter.Hidden = true;
            TestMaxReturnParameter.Hidden = true;

            if (lockobject == null)
                lockobject = new object();

            RegisterParameterEvents();

            
        }

        #endregion

        #region Event Handlers
        private void RegisterParameterEvents()
        {
            PriceVariableParameter.ValueChanged += new EventHandler(PriceVariableParameter_ValueChanged);
            TimePointVariableParameter.ValueChanged += new EventHandler(TimePointVariableParameter_ValueChanged);
            InputStates.CheckedItemsChanged += new CollectionItemsChangedEventHandler<IndexedItem<StringValue>>(InputStates_CheckedItemsChanged);
            //LastSeedParameter.Value.ValueChanged += new EventHandler(SeedParameter_ValueChanged);
            DatasetParameter.ValueChanged += new EventHandler(Dataset_ValueChanged);
            TrainingPartitionParameter.Value.ValueChanged += new EventHandler(TrainingPartition_ValueChanged);
            TestPartitionParameter.Value.ValueChanged += new EventHandler(TestPartition_ValueChanged);
            //    base.RegisterParameterEvents();

           
            
        }
        private void PriceVariableParameter_ValueChanged(object sender, EventArgs e)
        {

            MaxReturnZero(TrainingPartition);
            MaxReturnZero(TestPartition);
            OnChanged();
        }
        private void TimePointVariableParameter_ValueChanged(object sender, EventArgs e)
        {
            OnChanged();
        }

        private void SeedParameter_ValueChanged(object sender, EventArgs e)
        {
            MessageBox.Show("Seed Change!");
        }

        private void Dataset_ValueChanged(object sender, EventArgs e)
        {
            MaxReturnZero(TrainingPartition);
            MaxReturnZero(TestPartition);
        }

        private void TrainingPartition_ValueChanged(object sender, EventArgs e)
        {
            MaxReturnZero(TrainingPartition);
            MonteCarloSettings.NSamples = TrainingPartition.Size;
        }

        private void TestPartition_ValueChanged(object sender, EventArgs e)
        {
            MaxReturnZero(TestPartition);
        }

        #endregion

        private bool SeedCheck(int curSeed)
        {
            if (curSeed != LastSeed)
            {
                LastSeed = curSeed;
                return false;
            }
            return true;
        }

        public List<ITradingProblemData> MonteCarloSets(int seed, bool regenerate=false)
        {
            if (mcSets == null || SeedCheck(seed) == false || regenerate == true)
            {
                if (seed == 0)
                    seed = new Random().Next(0, 10000000);

                List<Dataset> sets = MonteCarloDataFactory.GenerateMonteCarloData(this.Dataset, this.TrainingPartition, this.MonteCarloSettings.NSamples, this.MonteCarloSettings.NEvaluations, this.TimePointVariable);
                List<ITradingProblemData> problems = new List<ITradingProblemData>();


                for (int i = 0; i < sets.Count(); i++)
                {
                    //Parallel.For(0, sets.Count, i=>
                    //{
                    TradingProblemData p = new TradingProblemData(sets[i], this.AllowedInputVariables, this.PriceVariable, this.TimePointVariable);
                    p.Name = seed.ToString() + " - " + (i + 1).ToString();
                    p.SimulationParameters = this.SimulationParameters;
                    p.InvertPrices = this.InvertPrices;
                    p.TrainingPartition.Start = 0;
                    p.TrainingPartition.End = this.MonteCarloSettings.NSamples;
                    p.TestPartition.Start = 0;
                    p.TestPartition.End = 0;
                    problems.Add(p);
                    //});
                }

                mcSets = problems;                 
            }
            return mcSets;
        }

        private void CalculateMaxReturn(IntRange partition)
        {
            IEnumerable<double> priceSeries = Dataset.GetDoubleValues(PriceVariable, Enumerable.Range(partition.Start, partition.Size));

            if (partition == TrainingPartition)
            {
                TrainingMaxReturn = MaximumReturnCalculator.MaximumReturn(priceSeries);
                //MessageBox.Show("Training Max Return Calculation!");
                return;
            }
            if (partition == TestPartition)
            {
                TestMaxReturn = MaximumReturnCalculator.MaximumReturn(priceSeries);
                //MessageBox.Show("Test Max Return Calculation!");
                return;
            }
        }

        private void MaxReturnZero(IntRange partition)
        {
            if (partition == TrainingPartition)
                TrainingMaxReturn = double.NaN;
            else if (partition == TestPartition)
                TestMaxReturn = double.NaN;
        }
    }
}
