using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;



namespace GP4Sim.Trading.Simulation
{
    public delegate void LogLineEventHandler(object sender, LogLineEventArgs e);

    public class TradingSimulation
    {
        private TradingSimulationRunMode runMode;
        private TradingFitnessType fitnessType;
        private double maxReturn;
        private TradingSimulationState state;
        private SimulationStats stats;
        private SimulationParameters_Internal settings;
        private double[] stateArray;

        private long dpIndex = 0;
        private double currentPrice = 0;
        private DateTime currentTimePoint;

        private ITradingEnvelope results = null;
        private MCEnvelope mcResults = null;

        private bool invertedInstrumentPrices = false;

        public event LogLineEventHandler LogEvent;

        #region Constructor
        public TradingSimulation(TradingSimulationRunMode rm, TradingFitnessType type, SimulationParameters_Internal simParameters, bool invertedInstrumentPrices, double maxReturn)
        {
            runMode = rm;
            fitnessType = type;
            this.maxReturn = maxReturn;
            settings = simParameters;
            this.invertedInstrumentPrices = invertedInstrumentPrices;
            state = new TradingSimulationState(settings.StartingNAV);
            stats = new SimulationStats(settings.StartingNAV, invertedInstrumentPrices);
            stateArray = new double[3];


        }
        #endregion

        #region Public Methods
        public double[] GetCurrentStates
        {
            get
            {
                stateArray[0] = State.PosAvgCost;
                stateArray[1] = State.PosAsNavPerc;
                stateArray[2] = State.PosQuantity;
                return stateArray;
            }
        }

        public void Step(double currentPrice, DateTime timePoint, bool isNewDay)
        {
            if (isNewDay)
            {
                //Stats.LastDayNAV = stats.SnapshotNAV;
                Stats.LastDayNAV = State.NAV;
                //Stats.AddNavPoint(stats.SnapshotNAV);
                Stats.AddNavPoint(State.NAV);
                Stats.AddInstrPoint(currentPrice);
                Stats.AddDayPoint(currentTimePoint);
                Stats.AddDailyTrades();

            }
            dpIndex++;
            this.currentPrice = currentPrice;
            currentTimePoint = timePoint;
            State.SetCurrentPrice(currentPrice);

            if (runMode!=TradingSimulationRunMode.EVALUATION)
            {
                Stats.Instrument.Add(currentPrice);
                if (dpIndex == 1)
                    stats.BuyHold.BHBuyPrice = currentPrice;
            }


        }

        public long GPOutputToTradeSignal(double agentOutput)
        {
            

            double deltaPercentage = 0;
            double posNavPerc = State.PosAsNavPerc;
            double desiredNavPosition = posNavPerc;

            if (agentOutput >= 0 && agentOutput < settings.MaxValidOutput)
                desiredNavPosition = Math.Min(settings.MinOrderSize * Math.Floor(agentOutput / settings.MinOrderSize), settings.MaxValidPosition);
            else if (agentOutput <= 0 && agentOutput > settings.MinValidOutput)
                desiredNavPosition = Math.Max(settings.MinOrderSize * Math.Ceiling(agentOutput / settings.MinOrderSize), settings.MinValidPosition);

            if (runMode!=TradingSimulationRunMode.EVALUATION)
                Stats.Exposure.Update(desiredNavPosition);

            deltaPercentage = Math.Round(desiredNavPosition - posNavPerc);

            if (Math.Abs(deltaPercentage) >= settings.MinOrderSize)
            {
                long finalLotSize = State.PosQuantity != 0 ? (long)((State.PosQuantity * desiredNavPosition) / posNavPerc) : (long)Math.Floor((desiredNavPosition / 100 * state.NAV) / currentPrice);
                long deltaShares = finalLotSize - (long)state.PosQuantity;
                return deltaShares;
            }
            else  
             
                return 0;
             
        }

        public void TradeAction(long tradeSignal)
        {
            if (tradeSignal > 0)
            {
                BuyAction(tradeSignal);
                Stats.Trading.NewRDDPoint(state.NAV);
            }
            else if (tradeSignal < 0)
            {
                SellAction(tradeSignal);
                Stats.Trading.NewRDDPoint(state.NAV);
            }

            Stats.SnapshotNAV = State.NAV;

            if (runMode!=TradingSimulationRunMode.EVALUATION)
            {
                Stats.Trading.NewDDPoint(State.NAV);
            }
        }


        public void WrapUp()
        {
            if (runMode==TradingSimulationRunMode.ANALYSIS)
            {
                Stats.BuyHold.BHSellPrice = currentPrice;
                Stats.BuyHold.BHReturn = BuyHoldReturn();
                Stats.AddInstrPoint(currentPrice);
                Stats.AddNavPoint(Stats.SnapshotNAV);
                Stats.AddDayPoint(currentTimePoint);
                Stats.Trading.Crossings.Finalize(State.NAV);
                results = GenerateResults();
            }
            else if (runMode == TradingSimulationRunMode.MONTECARLO)
            {
                Stats.BuyHold.BHSellPrice = currentPrice;
                Stats.BuyHold.BHReturn = BuyHoldReturn();
                Stats.AddInstrPoint(currentPrice);
                Stats.AddNavPoint(Stats.SnapshotNAV);
                Stats.AddDayPoint(currentTimePoint);
                Stats.Trading.Crossings.Finalize(State.NAV);
                mcResults = GenerateMCResults();

            }
        }
        #endregion

        #region Private Methods

        private void OnLogLine(string line)
        {

            LogEvent(this, new LogLineEventArgs(line));
        }

        private void BuyAction(long lotSize)
        {
            if (State.PosQuantity < 0)
            {
                long buyToCoverLot = Math.Min((long)Math.Abs(State.PosQuantity), lotSize);
                BuyToCover(buyToCoverLot);
                lotSize -= buyToCoverLot;
            }
            if (lotSize > 0)
                Buy(lotSize);
        }

        private void Buy(long lotSize)
        {
            lotSize = AdjustLot(lotSize);

            if (LogEvent != null)
            {
                OnLogLine(Logger.ActionSeparator);
                OnLogLine(Logger.NAV(State.AccountValue, State.CurrentPositionValue, state.NAV));

            }
            //if (lotSize > 0)
            //{
            double totalBuyPrice = currentPrice * lotSize;
            double commission = settings.Commissions(lotSize, currentPrice);

            double virtualSellPrice = State.AccountAvgCost * lotSize;
            double tradeReturn = TradeReturn(virtualSellPrice, totalBuyPrice, commission);

            

            State.Buy(totalBuyPrice + commission, lotSize);

            Stats.Trading.ExitShort(tradeReturn, State.NAV / Stats.SnapshotNAV);            
            Stats.Trading.Crossings.Cross(State.PosAsNavPerc, State.NAV);
            Stats.Trading.EnterLong();

            Stats.SnapshotNAV = State.NAV;
            if (LogEvent != null)
            {
                OnLogLine(Logger.Action("BUY", lotSize, currentPrice, commission, State.PosAvgCost));
                OnLogLine(Logger.NAV(State.AccountValue, state.CurrentPositionValue, state.NAV));
            }
            //}
        }

        private void BuyToCover(long lotSize)
        {
            throw new NotImplementedException();
        }

        private void SellAction(long lotSize)
        {
            lotSize = Math.Abs(lotSize);

            if (State.PosQuantity > 0)
            {
                long sellLot = (long)Math.Min(State.PosQuantity, lotSize);
                Sell(sellLot);
                lotSize -= sellLot;
            }

            //Quick and Dirty Fix
            //OBS
            if (lotSize > 0)
                //SellShort(lotSize);
                lotSize = 0;
        }

        private void Sell(long lotSize)
        {
            if (LogEvent != null)
            {
                OnLogLine(Logger.ActionSeparator);
                OnLogLine(Logger.NAV(State.AccountValue, State.CurrentPositionValue, state.NAV));
            }
            double totalBuyPrice = State.PosAvgCost * lotSize;
            double totalSellPrice = currentPrice * lotSize;
            double commission = settings.Commissions(lotSize, currentPrice);

            double earnings = totalSellPrice - commission;

            double tradeReturn = TradeReturn(totalBuyPrice, totalSellPrice, commission);

            State.Sell(earnings, lotSize);

            Stats.Trading.ExitLong(tradeReturn, State.NAV / Stats.SnapshotNAV);
            Stats.Trading.Crossings.Cross(State.PosAsNavPerc, State.NAV);
            Stats.SnapshotNAV = State.NAV;

            Stats.Trading.EnterShort();

            if (LogEvent != null)
            {
                OnLogLine(Logger.Action("SELL", lotSize, currentPrice, commission, State.PosAvgCost));
                OnLogLine(Logger.NAV(State.AccountValue, state.CurrentPositionValue, state.NAV));
            }
        }

        private void SellShort(long lotSize)
        {
            throw new NotImplementedException();
        }

        private long AdjustLot(long lotSize)
        {
            long originalSize = lotSize;

            while (State.AccountValue < (lotSize * currentPrice) + settings.Commissions(lotSize, currentPrice))
            {
                lotSize--;
                if (lotSize == 0)
                    break;
            }
            return lotSize;
        }

        private double TradeReturn(double buyPrice, double sellPrice, double costs)
        {
            double expenses = buyPrice + costs;
            //return sellPrice / expenses;
            //Is Percentage
            //Cannot do % here because of stddev computation
            return (sellPrice / expenses);
        }

        private double BuyHoldReturn()
        {
            double bhLot = Math.Floor(settings.StartingNAV / Stats.BuyHold.BHBuyPrice);
            double bhCosts = settings.Commissions(bhLot, Stats.BuyHold.BHBuyPrice) + settings.Commissions(bhLot, Stats.BuyHold.BHSellPrice);
            double bhReturn = TradeReturn(bhLot * Stats.BuyHold.BHBuyPrice, bhLot * Stats.BuyHold.BHSellPrice, bhCosts);

            return (bhReturn - 1) * 100;
        }

        private ITradingEnvelope GenerateResults()
        {
            TradingEnvelope env = new TradingEnvelope();
            env.FitnessScore = FitnessScoring.NEGEXP(FinalScore);
            env.InitialNAV = settings.StartingNAV;
            env.FinalNAV = Stats.SnapshotNAV;
            env.ScoredROI = (ScoredROI - 1) * 100;

            env.LastDayNAV = Stats.LastDayNAV;
            env.LastDayROI = ((Stats.LastDayNAV / settings.StartingNAV) - 1) * 100;

            env.UMDD = Stats.Trading.UMDD * 100;

            env.Tot_buys = (long)Stats.Trading.NPositionsEntered / 2;
            env.Tot_trades = (long)Stats.Trading.NumTrades / 2;
            env.Tot_RetRelStdDev = Stats.Trading.Returns.PercRelStdDev;
            env.Tot_RMDD = Stats.Trading.RMDD * 100;
            env.Tot_worstTrade = Stats.Trading.WorstTrade;
            env.Tot_cumulativeReturn = Stats.Trading.CumulativeReturn;

            env.Long_buys = (long)Stats.Trading.LongTrades.NPositionsEntered / 2;
            env.Long_trades = (long)Stats.Trading.LongTrades.NTrades / 2;
            env.Long_RetRelStdDev = Stats.Trading.LongTrades.Returns.PercRelStdDev;
            env.Long_RMDD = Stats.Trading.LongTrades.RMDD * 100;
            env.Long_worstTrade = Stats.Trading.LongTrades.WorstTrade;
            env.Long_cumulativeReturn = stats.Trading.LongTrades.CumulativeReturn;

            env.Short_buys = (long)Stats.Trading.ShortTrades.NPositionsEntered / 2;
            env.Short_trades = (long)Stats.Trading.ShortTrades.NTrades / 2;
            env.Short_RetRelStdDev = Stats.Trading.ShortTrades.Returns.PercRelStdDev;
            env.Short_RMDD = Stats.Trading.ShortTrades.RMDD * 100;
            env.Short_worstTrade = Stats.Trading.ShortTrades.WorstTrade;
            env.Short_cumulativeReturn = stats.Trading.ShortTrades.CumulativeReturn;

            env.Long_Crossings = Stats.Trading.Crossings.LongCrossings;
            env.LongPnL = stats.Trading.Crossings.LongPnL;
            env.Short_Crossings = Stats.Trading.Crossings.ShortCrossings;
            env.ShortPnL = stats.Trading.Crossings.ShortPnL;

            env.ExpAvg = Stats.Exposure.Total.Mean;
            env.ExpRelStdDev = Stats.Exposure.Total.PercRelStdDev;
            env.PosExpAvg = Stats.Exposure.Positive.Mean;
            env.NegExpAvg = Stats.Exposure.Negative.Mean;

            env.BhReturn = Stats.BuyHold.BHReturn;
            env.InstrRelStdDev = Stats.Instrument.PercRelStdDev;

            env.DailyNavPoints = Stats.DailyNavPoints;
            env.DailyInstrPoints = Stats.DailyInstrPoints;
            env.DayPoints = Stats.DayPoints;
            return env;
        }


        private MCEnvelope GenerateMCResults()
        {
            MCEnvelope env = new MCEnvelope();
            env.DailyNavPoints = Stats.DailyNavPoints;
            env.DailyInstrPoints = Stats.DailyInstrPoints;
            env.DayPoints = Stats.DayPoints;
            env.LastDayNAV = Stats.LastDayNAV;

            return env;
        }
        #endregion

        #region Public Properties
        public TradingSimulationState State { get { return state; } }
        public SimulationStats Stats { get { return stats; } }
        public ITradingEnvelope Results
        {
            get { return results; }

        }
        public MCEnvelope MCResults
        {
            get { return mcResults; }
        }
        public long DpIndex { get { return dpIndex; } }
        public DateTime CurrentTimePoint { get { return currentTimePoint; } }
        public double CurrentPrice { get { return currentPrice; } }

        public double ScoredROI { get { return Stats.SnapshotNAV / settings.StartingNAV; } }

        public double FinalScore
        {
            get
            {
                if (fitnessType == TradingFitnessType.NAV)
                {
                    if (Stats.SnapshotNAV < 0)
                        return (double)FitnessErrorCodes.IN_THE_RED;
                    else if (Stats.Trading.NumTrades < settings.MinTrades)
                        return (double)FitnessErrorCodes.TOO_FEW_TRADES;
                    else if (!Double.IsNaN(ScoredROI))
                        return
                            ScoredROI - 1;
                    else
                        //DEBUG PRIO #1
                        return (double)FitnessErrorCodes.NAV_IS_NAN;
                }
                else if (Stats.Trading.NumTrades < settings.MinTrades)
                {
                    if (fitnessType == TradingFitnessType.Score || fitnessType == TradingFitnessType.ScorePenalties || fitnessType == TradingFitnessType.ScoreWeighted || fitnessType == TradingFitnessType.Relative)
                        return -200;
                    else if (fitnessType == TradingFitnessType.ActivityAware)
                        return -Math.Exp(3) + 1;
                    else if (fitnessType == TradingFitnessType.Sortino || fitnessType == TradingFitnessType.AnnualizedSortino)
                        return -100;
                    else
                        return 0;
                }
                else if (fitnessType == TradingFitnessType.Score)
                    return ScoreAssigner.AssignScore(stats.DailyNavPoints, stats.DailyInstrPoints, invertedInstrumentPrices, maxReturn, settings.StartingNAV, stats.SnapshotNAV);
                else if (fitnessType == TradingFitnessType.ScorePenalties)
                    return ScoreAssignerWPenalties.AssignScore(stats.DailyNavPoints, stats.DailyInstrPoints, invertedInstrumentPrices, maxReturn, settings.StartingNAV, stats.SnapshotNAV);
                else if (fitnessType == TradingFitnessType.ScoreWeighted)
                    return ScoreAssignerWeighted.AssignScore(stats.DailyNavPoints, stats.DailyInstrPoints, invertedInstrumentPrices, maxReturn, settings.StartingNAV, stats.SnapshotNAV);
                else if (fitnessType == TradingFitnessType.Relative)
                    return ScoreAssignerRelative.AssignScore(stats.DailyNavPoints, stats.DailyInstrPoints, invertedInstrumentPrices, maxReturn, settings.StartingNAV, stats.SnapshotNAV);
                else if (fitnessType == TradingFitnessType.ActivityAware)
                    return ScoreAssignerActivityAware.AssignScore(stats.DailyNavPoints, stats.DailyInstrPoints, stats.DailyTrades, invertedInstrumentPrices, maxReturn, settings.StartingNAV, stats.SnapshotNAV, stats.Trading.RMDD);
                else if (fitnessType == TradingFitnessType.Sortino)
                    return ScoreAssignerSortino.AssignScore(stats.DailyNavPoints, stats.DailyInstrPoints, invertedInstrumentPrices, maxReturn, settings.StartingNAV, stats.SnapshotNAV, false);
                else if (fitnessType == TradingFitnessType.AnnualizedSortino)
                    return ScoreAssignerSortino.AssignScore(stats.DailyNavPoints, stats.DailyInstrPoints, invertedInstrumentPrices, maxReturn, settings.StartingNAV, stats.SnapshotNAV, true);
                else
                    return 0;

            }
        }

        public void Teardown()
        {
            state = null;
            stats.Teardown();
            stats = null;
            results = null;
            mcResults = null;
        }

        #endregion
    }
}
