using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using GP4Sim.SimulationFramework.Interfaces;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace GP4Sim.SimulationFramework.Solutions
{
    [StorableClass]
    [Item("SimulationEnvelope", "Represents an abstract results envelope")]
    public abstract class SimulationEnvelope : Item, ISimulationEnvelope
    {
        public static new Image StaticItemImage
        {
            get { return HeuristicLab.Common.Resources.VSImageLibrary.Function; }
        }


        #region Properties
        [Storable]
        private IEnumerable<double> rawOutput;
        public IEnumerable<double> RawOutput
        {
            get { return rawOutput; }
            set { rawOutput = value; }
        }

        [Storable]
        private double fitnessScore;

        [VisibleName("Fitness Score ")]
        [Description("Fitness Score")]
        [Category("General")]
        public double FitnessScore
        {
            get { return fitnessScore; }
            set { fitnessScore = value; }
        }
        #endregion

        #region Constructors
        [StorableConstructor]
        protected SimulationEnvelope(bool deserializing) : base(deserializing) { }
        protected SimulationEnvelope(SimulationEnvelope original, Cloner cloner)
            : base(original, cloner)
        {

        }
        public SimulationEnvelope()
            : base()
        {

        }

        public SimulationEnvelope(string test)
            : base()
        {
            if (test.Equals("test"))
            {
                rawOutput = new double[] { 0, 0, 0, -100, 100, double.NaN };
                fitnessScore = 42;
            }
        }

        #endregion
    }
}
