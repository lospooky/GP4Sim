using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.Common;
using HeuristicLab.Problems.DataAnalysis;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Parameters;
using HeuristicLab.Data;
using HeuristicLab.Core;
using HeuristicLab.Collections;

using GP4Sim.SimulationFramework.Interfaces;
using GP4Sim.Data;

namespace GP4Sim.SimulationFramework.Problem
{
    [StorableClass]
    public abstract class SimulationProblemData : DataAnalysisProblemData, ISimulationProblemData, IStorableContent
    {
        #region Internal States Properties


        protected const string InputStatesParameterName = "InternalStates";
        protected const string DataCacheParameterName = "DataCache";
        protected const string DataCacheParameterDescription = "Fast Access Data Cache";

        private int numInputVariables;
        private int numInputStates;

        public IFixedValueParameter<ReadOnlyCheckedItemList<StringValue>> InputStatesParameter
        {
            get { return (IFixedValueParameter<ReadOnlyCheckedItemList<StringValue>>)Parameters[InputStatesParameterName]; }
        }

        public ICheckedItemList<StringValue> InputStates
        {
            get { return InputStatesParameter.Value; }
        }
        public IEnumerable<string> AllowedInputStates
        {
            get { return InputStates.CheckedItems.Select(x => x.Value.Value); }
        }

        public IValueParameter<DataCache> DataCacheParameter
        {
            get { return (IValueParameter<DataCache>)Parameters[DataCacheParameterName]; }
        }

        public DataCache DataCache
        {
            get { return DataCacheParameter.Value; }
        }

        #endregion

        public string Filename { get; set; }

        #region Constructors

        [StorableConstructor]
        protected SimulationProblemData(bool deserializing) : base(deserializing) { }
        [StorableHook(HookType.AfterDeserialization)]
        private void AfterDeserialization()
        {
            if (!Parameters.ContainsKey(DataCacheParameterName))
            {
                Parameters.Add(new ValueParameter<DataCache>(DataCacheParameterName, DataCacheParameterDescription));
                DataCacheParameter.Hidden = true;
            }
            DataCache.Load(this.Dataset, this.AllowedInputVariables.ToList());
            RegisterParameterEvents();
        }

        protected SimulationProblemData(SimulationProblemData original, Cloner cloner)
            : base(original, cloner)
        {
            if (InputStates.Count == 0)
                InitializeInputStates();
            if (!Parameters.ContainsKey(DataCacheParameterName))
            {
                Parameters.Add(new ValueParameter<DataCache>(DataCacheParameterName, DataCacheParameterDescription));
                DataCacheParameter.Hidden = true;
            }
            DataCache.Load(this.Dataset, this.AllowedInputVariables.ToList());
            RegisterParameterEvents();
        }

        protected void SetPartitions(ISimulationProblemData AbstractProblemData)
        {
            TrainingPartition.Start = AbstractProblemData.TrainingPartition.Start;
            TrainingPartition.End = AbstractProblemData.TrainingPartition.End;
            TestPartition.Start = AbstractProblemData.TestPartition.Start;
            TestPartition.End = AbstractProblemData.TestPartition.End;
        }

        protected SimulationProblemData(Dataset dataset, IEnumerable<string> allowedInputVariables)
            : base(dataset, allowedInputVariables)
        {
            Parameters.Add(new ValueParameter<DataCache>(DataCacheParameterName, DataCacheParameterDescription));
            DataCache.Load(this.Dataset, this.AllowedInputVariables.ToList());
            DataCacheParameter.Hidden = true;
            InitializeInputStates();
            RegisterParameterEvents();
        }
        #endregion

        protected void RegisterParameterEvents()
        {
            InputVariables.CheckedItemsChanged += InputVariables_CheckedItemsChanged;
            //InputStates.CheckedItemsChanged +=new CollectionItemsChangedEventHandler<IndexedItem<StringValue>>(InputStates_CheckedItemsChanged);
            return;
        }

        protected void InputVariables_CheckedItemsChanged(object sender, CollectionItemsChangedEventArgs<IndexedItem<StringValue>> e)
        {
            //MessageBox.Show("Input Variables Changed", "YAY!", MessageBoxButtons.OK, MessageBoxIcon.Information);
            DataCache.Load(this.Dataset, this.AllowedInputVariables.ToList());
            OnChanged();
        }

        protected void InitializeInputStates()
        {
            var inputStates = new CheckedItemList<StringValue>(InternalStatusNames.Select(x => new StringValue(x)));
            foreach (StringValue x in inputStates)
                inputStates.SetItemCheckedState(x, true);
            Parameters.Add(new FixedValueParameter<ReadOnlyCheckedItemList<StringValue>>(InputStatesParameterName, "", inputStates.AsReadOnly()));
        }

        protected virtual List<string> InternalStatusNames
        {
            get { return new List<string>(); }

        }

        protected void InputStates_CheckedItemsChanged(object sender, CollectionItemsChangedEventArgs<IndexedItem<StringValue>> e)
        {
            OnChanged();
        }
    }
}
