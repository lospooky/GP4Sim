using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GP4Sim.Trading.Simulation
{
    public class FullTradingStats
    {
        private double navPeak;
        private double uMDD;
        private double worstTrade;
        private double cumulativeReturn;
        private double rMDD;

        public SideTradingStats LongTrades;
        public SideTradingStats ShortTrades;
        public OnlineMVSCalculator Returns;
        public CrossingStats Crossings;


        public FullTradingStats(double initialNav)
        {
            worstTrade = double.MaxValue;
            cumulativeReturn = 0;
            rMDD = 0;
            uMDD = 0;
            navPeak = initialNav;

            LongTrades = new SideTradingStats();
            ShortTrades = new SideTradingStats();
            Returns = new OnlineMVSCalculator();
            Crossings = new CrossingStats(initialNav);
        }

        public void EnterLong()
        {
            LongTrades.EnterPosition();
        }

        public void ExitLong(double retValue, double drawDown)
        {
            LongTrades.ExitPosition(retValue, drawDown);
            Return(retValue, drawDown);
        }

        public void EnterShort()
        {
            ShortTrades.EnterPosition();
        }

        public void ExitShort(double retValue, double drawDown)
        {
            ShortTrades.ExitPosition(retValue, drawDown);
            Return(retValue, drawDown);
        }

        public void Return(double retValue, double drawDown)
        {
            Returns.Add(retValue);
            //Is still in decimal form
            retValue = (retValue - 1) * 100;
            if (retValue < worstTrade)
                worstTrade = retValue;
            cumulativeReturn += retValue;

            //if ((1 - drawDown) > rMDD)
                //rMDD = 1 - (drawDown);
        }

        public void NewDDPoint(double navPoint)
        {
            if (navPoint > navPeak)
                navPeak = navPoint;
            else
            {
                double DD = (navPeak - navPoint) / navPeak;
                if (DD > uMDD)
                    uMDD = DD;
            }
        }

        public void NewRDDPoint(double navPoint)
        {
            if (navPoint > navPeak)
                navPeak = navPoint;
            else
            {
                double DD = (navPeak - navPoint) / navPeak;
                if (DD > rMDD)
                    rMDD = DD;
            }
        }

        public double NPositionsEntered { get { return LongTrades.NPositionsEntered + ShortTrades.NPositionsEntered; } }
        public double NumTrades { get { return LongTrades.NTrades + ShortTrades.NTrades; } }
        public double UMDD { get { return uMDD; } }
        public double RMDD { get { return rMDD; } }
        public double WorstTrade { get { return worstTrade; } }
        public double CumulativeReturn { get { return cumulativeReturn; } }
    }
}
