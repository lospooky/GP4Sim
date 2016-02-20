using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Core;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Common;

namespace GP4Sim.SymbolicTrees
{
    [StorableClass]
    [Item("InternalState", "Represents an internal internalInternalState value")]
    public class InternalState : Symbol
    {
        #region Properties
        [Storable]
        private double weightMu;
        public double WeightMu
        {
            get { return weightMu; }
            set
            {
                if (value != weightMu)
                {
                    weightMu = value;
                    OnChanged(EventArgs.Empty);
                }
            }
        }
        [Storable]
        private double weightSigma;
        public double WeightSigma
        {
            get { return weightSigma; }
            set
            {
                if (weightSigma < 0.0) throw new ArgumentException("Negative sigma is not allowed.");
                if (value != weightSigma)
                {
                    weightSigma = value;
                    OnChanged(EventArgs.Empty);
                }
            }
        }
        [Storable]
        private double weightManipulatorMu;
        public double WeightManipulatorMu
        {
            get { return weightManipulatorMu; }
            set
            {
                if (value != weightManipulatorMu)
                {
                    weightManipulatorMu = value;
                    OnChanged(EventArgs.Empty);
                }
            }
        }
        [Storable]
        private double weightManipulatorSigma;
        public double WeightManipulatorSigma
        {
            get { return weightManipulatorSigma; }
            set
            {
                if (weightManipulatorSigma < 0.0) throw new ArgumentException("Negative sigma is not allowed.");
                if (value != weightManipulatorSigma)
                {
                    weightManipulatorSigma = value;
                    OnChanged(EventArgs.Empty);
                }
            }
        }
        [Storable(DefaultValue = 0.0)]
        private double multiplicativeWeightManipulatorSigma;
        public double MultiplicativeWeightManipulatorSigma
        {
            get { return multiplicativeWeightManipulatorSigma; }
            set
            {
                if (multiplicativeWeightManipulatorSigma < 0.0) throw new ArgumentException("Negative sigma is not allowed.");
                if (value != multiplicativeWeightManipulatorSigma)
                {
                    multiplicativeWeightManipulatorSigma = value;
                    OnChanged(EventArgs.Empty);
                }
            }
        }
        private List<string> internalStateNames;
        [Storable]
        public IEnumerable<string> InternalStateNames
        {
            get { return internalStateNames; }
            set
            {
                if (value == null) throw new ArgumentNullException();
                internalStateNames.Clear();
                internalStateNames.AddRange(value);
                OnChanged(EventArgs.Empty);
            }
        }

        private List<string> allInternalStateNames;
        [Storable]
        public IEnumerable<string> AllInternalStateNames
        {
            get { return allInternalStateNames; }
            set
            {
                if (value == null) throw new ArgumentNullException();
                allInternalStateNames.Clear();
                allInternalStateNames.AddRange(value);
            }
        }

        public override bool Enabled
        {
            get
            {
                if (internalStateNames.Count == 0) return false;
                return base.Enabled;
            }
            set
            {
                if (internalStateNames.Count == 0) base.Enabled = false;
                else base.Enabled = value;
            }
        }

        private const int minimumArity = 0;
        private const int maximumArity = 0;

        public override int MinimumArity
        {
            get { return minimumArity; }
        }
        public override int MaximumArity
        {
            get { return maximumArity; }
        }
        #endregion

        [StorableHook(HookType.AfterDeserialization)]
        private void AfterDeserialization()
        {
            if (allInternalStateNames == null || (allInternalStateNames.Count == 0 && internalStateNames.Count > 0))
            {
                allInternalStateNames = internalStateNames;
            }
        }

        [StorableConstructor]
        protected InternalState(bool deserializing)
            : base(deserializing)
        {
            internalStateNames = new List<string>();
            allInternalStateNames = new List<string>();
        }
        protected InternalState(InternalState original, Cloner cloner)
            : base(original, cloner)
        {
            weightMu = original.weightMu;
            weightSigma = original.weightSigma;
            internalStateNames = new List<string>(original.internalStateNames);
            allInternalStateNames = new List<string>(original.allInternalStateNames);
            weightManipulatorMu = original.weightManipulatorMu;
            weightManipulatorSigma = original.weightManipulatorSigma;
            multiplicativeWeightManipulatorSigma = original.multiplicativeWeightManipulatorSigma;
        }
        public InternalState() : this("InternalState", "Represents an internal internalInternalState value.") { }
        public InternalState(string name, string description)
            : base(name, description)
        {
            weightMu = 1.0;
            weightSigma = 1.0;
            weightManipulatorMu = 0.0;
            weightManipulatorSigma = 0.05;
            multiplicativeWeightManipulatorSigma = 0.03;
            internalStateNames = new List<string>();
            allInternalStateNames = new List<string>();
        }

        public override ISymbolicExpressionTreeNode CreateTreeNode()
        {
            return new InternalStateTreeNode(this);
        }

        public override IDeepCloneable Clone(Cloner cloner)
        {
            return new InternalState(this, cloner);
        }
    }
}
