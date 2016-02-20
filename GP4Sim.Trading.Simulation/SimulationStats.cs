using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GP4Sim.Trading.Simulation
{
    public class SimulationStats
    {
        public OnlineMVSCalculator Instrument;
        public ExposureStats Exposure;
        public BuyHoldStats BuyHold;
        public FullTradingStats Trading;

        private double snapshotNav;
        private double lastDayNav;
        private List<double> dailyNavPoints;
        private List<double> dailyInstrPoints;
        private List<int> dailyTradingActivity;
        private bool invertedPrices = false;
        private List<DateTime> dayPoints;

        public SimulationStats(double initialNAV, bool invertedInstrumentPrices)
        {
            Instrument = new OnlineMVSCalculator();
            Exposure = new ExposureStats();
            BuyHold = new BuyHoldStats();
            Trading = new FullTradingStats(initialNAV);


            snapshotNav = 0;
            lastDayNav = 0;
            dailyNavPoints = new List<double>();
            dailyInstrPoints = new List<double>();
            dailyTradingActivity = new List<int>();
            this.invertedPrices = invertedInstrumentPrices;
            dayPoints = new List<DateTime>();
        }

        public double SnapshotNAV
        {
            get { return snapshotNav; }
            set { snapshotNav = value; }
        }

        public double LastDayNAV
        {
            get { return lastDayNav; }
            set { lastDayNav = value; }
        }

        public List<double> DailyNavPoints
        {
            get
            {
                if (dailyNavPoints.Count > 0)
                    return dailyNavPoints;
                else
                    return new List<double> { 0 };
            }
        }

        public void AddNavPoint(double nav)
        {
            dailyNavPoints.Add(nav);
        }

        public List<double> DailyInstrPoints
        {
            get
            {
                if (dailyInstrPoints.Count > 0)
                    return dailyInstrPoints;
                else
                    return new List<double> { 0 };
            }
        }

        public void AddInstrPoint(double price)
        {
            if (!invertedPrices)
                dailyInstrPoints.Add(price);
            else
                dailyInstrPoints.Add(1 / price);
        }

        public List<DateTime> DayPoints
        {
            get
            {
                if (dayPoints.Count > 0)
                    return dayPoints;
                else
                    return new List<DateTime> { DateTime.MinValue };
            }
        }

        public void AddDayPoint(DateTime dt)
        {
            dayPoints.Add(dt.Date);
        }

        public List<int> DailyTrades
        {
            get
            {
                if (dailyTradingActivity.Count > 0)
                    return dailyTradingActivity;
                else
                    return new List<int> { int.MinValue };
            }
        }

        public void AddDailyTrades()
        {
            dailyTradingActivity.Add(Math.Min(this.Trading.LongTrades.NPositionsEntered, this.Trading.ShortTrades.NPositionsEntered));
        }

        public void Teardown()
        {
            Instrument = null;
            Exposure = null;
            Trading = null;
            BuyHold = null;
        }
    }
}
