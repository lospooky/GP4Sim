using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GP4Sim.SimulationFramework.Interfaces;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace GP4Sim.SimulationFramework.Parameters
{
    [StorableClass]
    public abstract class SimulationParameters : ParameterizedNamedItem, ISimulationParameters
    {

        protected SimulationParameters()
        {

        }

        protected SimulationParameters(SimulationParameters original, Cloner cloner)
            : base(original, cloner)
        {
            RegisterEventHandlers();
        }

        [StorableConstructor]
        protected SimulationParameters(bool deserializing) : base(deserializing) { }

        [StorableHook(HookType.AfterDeserialization)]
        private void AfterDeserialization()
        {
            RegisterEventHandlers();
        }

        protected abstract void RegisterEventHandlers();

        protected void Parameter_ValueChanged(object sender, EventArgs e)
        {
            OnChanged();
        }

        public event EventHandler Changed;
        protected virtual void OnChanged()
        {
            var listeners = Changed;
            if (listeners != null) listeners(this, EventArgs.Empty);
        }
    }
}
