using System;
using System.Collections.Generic;
using GP4Sim.SimulationFramework.Solutions;
using GP4Sim.Trading.Simulation;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace GP4Sim.Trading.Simulation
{
    [StorableClass]
    [Item("Trading Envelope", "Represents an abstract results envelope")]
    public class TradingEnvelope : SimulationEnvelope, ITradingEnvelope
    {


        #region Liquidity
        [Storable]
        private double initialNAV;

        [VisibleName("Initial NAV")]
        [Description("NAV at the start of the trading period")]
        [Category("Liquidity")]
        public double InitialNAV
        {
            get { return initialNAV; }
            set { initialNAV = value; }
        }

        [Storable]
        private double finalNAV;

        [VisibleName("Final NAV")]
        [Description("NAV at the end of the trading period")]
        [Category("Liquidity")]
        public double FinalNAV
        {
            get { return finalNAV; }
            set { finalNAV = value; }
        }

        [Storable]
        private double scoredROI;

        [VisibleName("Scored ROI %")]
        [Description("ROI % scored for fitness purposes")]
        [Category("Liquidity")]
        public double ScoredROI
        {
            get { return scoredROI; }
            set { scoredROI = value; }
        }

        [Storable]
        private double lastdayNAV;

        [VisibleName("Last Day NAV")]
        [Description("NAV at the end of the last complete trading day")]
        [Category("Liquidity")]
        public double LastDayNAV
        {
            get { return lastdayNAV; }
            set { lastdayNAV = value; }
        }

        [Storable]
        private double lastdayROI;

        [VisibleName("Last Day ROI %")]
        [Description("ROI at the end of the last complete trading day")]
        [Category("Liquidity")]
        public double LastDayROI
        {
            get { return lastdayROI; }
            set { lastdayROI = value; }
        }

        [Storable]
        private double uMDD;

        [VisibleName("Unrealized MDD %")]
        [Description("Unrealized Maximum DrawDown")]
        [Category("Liquidity")]
        public double UMDD
        {
            get { return uMDD; }
            set { uMDD = value; }
        }
        #endregion

        #region Trades Total

        [Storable]
        private long tot_buys;

        [VisibleName("Num Buys")]
        [Description("Number of Entered Positions")]
        [Category("Trading - Total")]
        public long Tot_buys
        {
            get { return tot_buys; }
            set { tot_buys = value; }
        }

        [Storable]
        private long tot_trades;

        [VisibleName("Num Trades")]
        [Description("Number of Trades")]
        [Category("Trading - Total")]
        public long Tot_trades
        {
            get { return tot_trades; }
            set { tot_trades = value; }
        }

        [Storable]
        private double tot_RetRelStdDev;

        [VisibleName("Returns StdDev %")]
        [Description("Relative Standard Deviation of Returns")]
        [Category("Trading - Total")]
        public double Tot_RetRelStdDev
        {
            get { return tot_RetRelStdDev; }
            set { tot_RetRelStdDev = value; }
        }

        [Storable]
        private double tot_RMDD;
        [VisibleName("Realized MDD %")]
        [Description("Realized Maximum Drawdown")]
        [Category("Trading - Total")]
        public double Tot_RMDD
        {
            get { return tot_RMDD; }
            set { tot_RMDD = value; }
        }

        [Storable]
        private double tot_worstTrade;

        [VisibleName("Worst Trade %")]
        [Description("Worst Realized Trade")]
        [Category("Trading - Total")]
        public double Tot_worstTrade
        {
            get { return tot_worstTrade; }
            set { tot_worstTrade = value; }
        }

        [Storable]
        private double tot_cumulativeReturn;
        [VisibleName("Cumulative Return %")]
        [Description("Cumulative Return")]
        [Category("Trading - Total")]
        public double Tot_cumulativeReturn
        {
            get { return tot_cumulativeReturn; }
            set { tot_cumulativeReturn = value; }
        }
        #endregion

        #region Trades Long
        //TRADES-LONG
        [Storable]
        private long long_buys;
        [VisibleName("Num Buys")]
        [Description("Number of Entered Positions")]
        [Category("Trading - Long")]
        public long Long_buys
        {
            get { return long_buys; }
            set { long_buys = value; }
        }

        [Storable]
        private long long_trades;
        [VisibleName("Num Trades")]
        [Description("Number of Trades")]
        [Category("Trading - Long")]
        public long Long_trades
        {
            get { return long_trades; }
            set { long_trades = value; }
        }

        [Storable]
        private double long_RetRelStdDev;
        [VisibleName("Returns StdDev %")]
        [Description("Relative Standard Deviation of Returns")]
        [Category("Trading - Long")]
        public double Long_RetRelStdDev
        {
            get { return long_RetRelStdDev; }
            set { long_RetRelStdDev = value; }
        }

        [Storable]
        private double long_RMDD;

        [VisibleName("Realized MDD %")]
        [Description("Realized Maximum Drawdown")]
        [Category("Trading - Long")]
        public double Long_RMDD
        {
            get { return long_RMDD; }
            set { long_RMDD = value; }
        }

        [Storable]
        private double long_worstTrade;


        [VisibleName("Worst Trade %")]
        [Description("Worst Realized Trade")]
        [Category("Trading - Long")]
        public double Long_worstTrade
        {
            get { return long_worstTrade; }
            set { long_worstTrade = value; }
        }

        [Storable]
        private double long_cumulativeReturn;

        [VisibleName("Cumulative Return %")]
        [Description("Cumulative Return")]
        [Category("Trading - Long")]
        public double Long_cumulativeReturn
        {
            get { return long_cumulativeReturn; }
            set { long_cumulativeReturn = value; }
        }

        #endregion

        #region Trades Short
        [Storable]
        private long short_buys;

        [VisibleName("Num Buys")]
        [Description("Number of Entered Positions")]
        [Category("Trading - Short")]
        public long Short_buys
        {
            get { return short_buys; }
            set { short_buys = value; }
        }

        [Storable]
        private long short_trades;

        [VisibleName("Num Trades")]
        [Description("Number of Trades")]
        [Category("Trading - Short")]
        public long Short_trades
        {
            get { return short_trades; }
            set { short_trades = value; }
        }

        [Storable]
        private double short_RetRelStdDev;
        [VisibleName("Returns StdDev %")]
        [Description("Relative Standard Deviation of Returns")]
        [Category("Trading - Short")]
        public double Short_RetRelStdDev
        {
            get { return short_RetRelStdDev; }
            set { short_RetRelStdDev = value; }
        }

        [Storable]
        private double short_RMDD;

        [VisibleName("Realized MDD %")]
        [Description("Realized Maximum Drawdown")]
        [Category("Trading - Short")]
        public double Short_RMDD
        {
            get { return short_RMDD; }
            set { short_RMDD = value; }
        }

        [Storable]
        private double short_worstTrade;

        [VisibleName("Worst Trade %")]
        [Description("Worst Realized Trade")]
        [Category("Trading - Short")]
        public double Short_worstTrade
        {
            get { return short_worstTrade; }
            set { short_worstTrade = value; }
        }

        [Storable]
        private double short_cumulativeReturn;

        [VisibleName("Cumulative Return %")]
        [Description("Cumulative Return")]
        [Category("Trading - Short")]
        public double Short_cumulativeReturn
        {
            get { return short_cumulativeReturn; }
            set { short_cumulativeReturn = value; }
        }
        #endregion

        #region Crossings
        [Storable]
        private int longCrossings;

        [VisibleName("Long Crossings")]
        [Description("Long Crossings")]
        [Category("Crossings")]
        public int Long_Crossings
        {
            get { return longCrossings; }
            set { longCrossings = value; }
        }

        [Storable]
        private int shortCrossings;

        [VisibleName("Short Crossings")]
        [Description("Short Crossings")]
        [Category("Crossings")]
        public int Short_Crossings
        {
            get { return shortCrossings; }
            set { shortCrossings = value; }
        }

        [Storable]
        private double longPnL;

        [VisibleName("Long PnL")]
        [Description("Long PnL")]
        [Category("Liquidity")]
        public double LongPnL
        {
            get { return longPnL; }
            set { longPnL = value; }
        }

        [Storable]
        private double shortPnL;

        [VisibleName("Short PnL")]
        [Description("Short PnL")]
        [Category("Liquidity")]
        public double ShortPnL
        {
            get { return shortPnL; }
            set { shortPnL = value; }
        }
        #endregion

        #region Exposure
        [Storable]
        private double expAvg;
        [VisibleName("Exposure Average %")]
        [Description("Exposure Average")]
        [Category("Exposure")]
        public double ExpAvg
        {
            get { return expAvg; }
            set { expAvg = value; }
        }

        [Storable]
        private double expRelStdDev;
        [VisibleName("Exposure StdDev %")]
        [Description("Relative Standard Deviation of Exposure")]
        [Category("Exposure")]
        public double ExpRelStdDev
        {
            get { return expRelStdDev; }
            set { expRelStdDev = value; }
        }

        [Storable]
        private double posExpAvg;
        [VisibleName("Positive Exposure Average %")]
        [Description("Positive Exposure Average")]
        [Category("Exposure")]
        public double PosExpAvg
        {
            get { return posExpAvg; }
            set { posExpAvg = value; }
        }

        [Storable]
        private double negExpAvg;
        [VisibleName("Negative Exposure Average %")]
        [Description("Negative Exposure Average")]
        [Category("Exposure")]
        public double NegExpAvg
        {
            get { return negExpAvg; }
            set { negExpAvg = value; }
        }
        #endregion

        #region Instrument
        //INSTRUMENT

        [Storable]
        private double bhReturn;
        [VisibleName("BuyHold Return %")]
        [Description("Return of BuyHold Strategy")]
        [Category("Instrument")]
        public double BhReturn
        {
            get { return bhReturn; }
            set { bhReturn = value; }
        }

        [Storable]
        private double instrRelStdDev;
        [VisibleName("Instrument StdDev %")]
        [Description("Relative Standard Deviation of Exposure")]
        [Category("Instrument")]
        public double InstrRelStdDev
        {
            get { return instrRelStdDev; }
            set { instrRelStdDev = value; }
        }

        #endregion

        #region Derived Stats

        public double ROI
        {
            get { return ((initialNAV / finalNAV) - 1) * 100; }
        }

        #endregion


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

        #region Constructors
        [StorableConstructor]
        protected TradingEnvelope(bool deserializing) : base(deserializing) { }
        public TradingEnvelope(TradingEnvelope original, Cloner cloner)
            : base(original, cloner)
        {

        }
        public TradingEnvelope()
            : base() { }


        public override IDeepCloneable Clone(Cloner cloner)
        {
            return new TradingEnvelope(this, cloner);
        }
        #endregion
    }
}
