using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GP4Sim.Trading.Simulation
{
    public class SideTradingStats
    {
        private int nTrades;
        private int nPositionsEntered;
        private double worstTrade;
        private double cumulativeReturn;
        private double rMDD;
        public OnlineMVSCalculator Returns;

        public SideTradingStats()
        {
            nTrades = 0;
            nPositionsEntered = 0;
            worstTrade = double.MaxValue;
            cumulativeReturn = 0;
            rMDD = double.MinValue;
            Returns = new OnlineMVSCalculator();
        }

        public void EnterPosition()
        {
            nPositionsEntered++;
        }

        public void ExitPosition(double retValue, double drawDown)
        {
            nTrades++;
            Return(retValue, drawDown);
        }

        public void Return(double retValue, double drawDown)
        {
            Returns.Add(retValue);
            retValue = (retValue - 1) * 100;
            if (retValue < worstTrade)
                worstTrade = retValue;
            cumulativeReturn += retValue;
            if ((1 - drawDown) > rMDD)
                rMDD = 1 - (drawDown);
        }

        public int NTrades { get { return nTrades; } }
        public int NPositionsEntered { get { return nPositionsEntered; } }
        public double WorstTrade { get { return worstTrade; } }
        public double CumulativeReturn { get { return cumulativeReturn; } }
        public double RMDD { get { return rMDD; } }

    }
}
