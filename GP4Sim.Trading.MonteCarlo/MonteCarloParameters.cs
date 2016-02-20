using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace GP4Sim.Trading.MonteCarlo
{
    [StorableClass]
    public class MonteCarloParameters : ParameterizedNamedItem, INamedItem, IItem, IContent, IDeepCloneable, ICloneable
    {
        #region Parameter Names
        protected const string NEvaluationsParameterName = "Number of Monte Carlo Evaluations";
        protected const string NSamplesParameterName = "Length";
        protected const string LastSeedParameterName = "Last Seed";
        protected const string EnabledParameterName = "Enable Monte Carlo Evaluation";

        private static int lengthfactor = 3;

        

        [Storable]
        private bool regenerateData = true;

        #endregion

        #region Parameters
        public IValueParameter<IntValue> NEvaluationsParameter
        {
            get { return (IValueParameter<IntValue>)Parameters[NEvaluationsParameterName]; }
        }

        public IValueParameter<IntValue> NSamplesParameter
        {
            get { return (IValueParameter<IntValue>)Parameters[NSamplesParameterName]; }
        }
        
        public IValueParameter<IntValue> LastSeedParameter
        {
            get { return (IValueParameter<IntValue>)Parameters[LastSeedParameterName]; }
        }

        public IValueParameter<BoolValue> EnabledParameter
        {
            get { return (IValueParameter<BoolValue>)Parameters[EnabledParameterName]; }
        }

        #endregion

        #region Properties
        public int NEvaluations
        {
            get { return NEvaluationsParameter.Value.Value; }
            set { NEvaluationsParameter.Value.Value = value; }
        }

        public int NSamples
        {
            get { return NSamplesParameter.Value.Value; }
            set { NSamplesParameter.Value.Value = value/lengthfactor; }
        }

        public int LastSeed
        {
            get { return LastSeedParameter.Value.Value; }
            set { LastSeedParameter.Value.Value = value; }
        }

        public bool Enabled
        {
            get { return EnabledParameter.Value.Value; }
            set { EnabledParameter.Value.Value = value; }
        }

        #endregion

        #region Constructors
        public MonteCarloParameters(int nSamples=150, int nSets=3)
        {
            Parameters.Add(new ValueParameter<IntValue>(NEvaluationsParameterName, new IntValue(nSets)));
            Parameters.Add(new ValueParameter<IntValue>(NSamplesParameterName, "Length of Monte Carlo Evaluations, as fraction of training set length.", new IntValue(nSamples/lengthfactor)));
            Parameters.Add(new ValueParameter<IntValue>(LastSeedParameterName, new IntValue(0)));
            Parameters.Add(new ValueParameter<BoolValue>(EnabledParameterName, new BoolValue(false)));
            LastSeedParameter.Hidden = true;
        }

        protected MonteCarloParameters(MonteCarloParameters original, Cloner cloner)
            :base(original, cloner)
        {
            RegisterEventHandlers();
        }

        [StorableConstructor]
        protected MonteCarloParameters(bool deserializing) : base(deserializing) { }

        [StorableHook(HookType.AfterDeserialization)]
        private void AfterDeserialization()
        {
            RegisterEventHandlers();
            return;
        }

        public override IDeepCloneable Clone(Cloner cloner)
        {
            return new MonteCarloParameters(this, cloner);

        }

        #endregion

        #region Event Handlers

        public event EventHandler Changed;

        protected void RegisterEventHandlers()
        {
            NEvaluationsParameter.ValueChanged += new EventHandler(Parameter_ValueChanged);
            NSamplesParameter.ValueChanged += new EventHandler(Parameter_ValueChanged);
        }

        private void Parameter_ValueChanged(object sender, EventArgs e)
        {
            regenerateData = true;
            OnChanged(e);
        }

        protected virtual void OnChanged(EventArgs e)
        {
            if (Changed != null)
                Changed(this, e);
        }
        #endregion

    }
}
