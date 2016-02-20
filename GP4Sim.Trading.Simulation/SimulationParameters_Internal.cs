using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.Problems.DataAnalysis;

namespace GP4Sim.Trading.Simulation
{
    public class SimulationParameters_Internal
    {
        private int minOrderSize;
        private double startingNAV;
        private double slack;
        private int minTrades;
        private double minValidOutput;
        private double maxValidOutput;
        private double minValidPosition;
        private double maxValidPosition;

        private CommissionFunction commissions;

        public SimulationParameters_Internal(int minOrderSize, double startingNAV, double slack, int minTrades, DoubleLimit ExposureLimits, CommissionFunction CommissionFcn)
        {
            this.minOrderSize = minOrderSize;
            this.startingNAV = startingNAV;
            this.slack = slack;
            this.minTrades = minTrades;
            this.minValidOutput = ExposureLimits.Lower * slack;
            this.maxValidOutput = ExposureLimits.Upper * slack;
            this.minValidPosition = ExposureLimits.Lower;
            this.maxValidPosition = ExposureLimits.Upper;
            commissions = CommissionFcn;
        }

        public int MinOrderSize { get { return minOrderSize; } }
        public double StartingNAV { get { return startingNAV; } }
        public double Slack { get { return slack; } }
        public int MinTrades { get { return minTrades; } }
        public double MinValidOutput { get { return minValidOutput; } }
        public double MaxValidOutput { get { return maxValidOutput; } }
        public double MinValidPosition { get { return minValidPosition; } }
        public double MaxValidPosition { get { return maxValidPosition; } }
        public CommissionFunction Commissions { get { return commissions; } }

    }
}
