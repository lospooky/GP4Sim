using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using GP4Sim.SimulationFramework.Solutions;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace GP4Sim.Trading.Simulation
{
    [StorableClass]
    [Item("MonteCarlo Envelope", "Represents Monte Carlo results envelope")]
    public class MCEnvelope : Item
    {
        public static new Image StaticItemImage
        {
            get { return HeuristicLab.Common.Resources.VSImageLibrary.Function; }
        }

        #region Result Properties
        [Storable]
        private double lastdayNAV;

        [VisibleName("Last Day NAV")]
        [Description("NAV at the end of the last complete trading day")]
        public double LastDayNAV
        {
            get { return lastdayNAV; }
            set { lastdayNAV = value; }
        }


        #region Daily Values
        [Storable]
        private IEnumerable<double> dailyNavPoints;
        [VisibleName("Daily NAV Points")]
        [Description("End-of-Day NAV Values")]
        [Category("Daily")]
        public IEnumerable<double> DailyNavPoints
        {
            get { return dailyNavPoints; }
            set { dailyNavPoints = value; }
        }

        [Storable]
        private IEnumerable<double> dailyInstrPoints;
        [VisibleName("Daily Instrument Points")]
        [Description("End-of-Day Instrument Prices")]
        [Category("Daily")]
        public IEnumerable<double> DailyInstrPoints
        {
            get { return dailyInstrPoints; }
            set { dailyInstrPoints = value; }
        }

        [Storable]
        private IEnumerable<DateTime> dayPoints;
        public IEnumerable<DateTime> DayPoints
        {
            get { return dayPoints; }
            set { dayPoints = value; }
        }
        #endregion
        #endregion


        #region Constructors
        [StorableConstructor]
        protected MCEnvelope(bool deserializing) : base(deserializing) { }
        public MCEnvelope(MCEnvelope original, Cloner cloner)
            : base(original, cloner)
        {

        }
        public MCEnvelope()
            : base()
        {
            dailyNavPoints = new List<double>();
            dailyInstrPoints = new List<double>();
            dayPoints = new List<DateTime>();
        }

        

        public override IDeepCloneable Clone(Cloner cloner)
        {
            return new MCEnvelope(this, cloner);
        }
        #endregion
    }
}
