using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GP4Sim.Trading.Simulation
{
    public class CrossingStats
    {
        private double lastPosition = double.NaN;
        private double lastNAV;
        private int longCrossings = 0;
        private int shortCrossings = 0;
        private double longpnl = 0;
        private double shortpnl = 0;

        public int LongCrossings { get { return longCrossings; } }
        public int ShortCrossings { get { return shortCrossings; } }

        public double LongPnL { get { return longpnl; } }
        public double ShortPnL { get { return shortpnl; } }

        public CrossingStats(double initialNav)
        {
            lastNAV = initialNav;
        }

        public void Cross(double newPos, double newNav)
        {
            if (double.IsNaN(lastPosition))
            {
                lastPosition = newPos;
                return;
            }
            else if (newPos!=50)
            {
                if (lastPosition < 50 && newPos > 50)
                {
                    shortCrossings++;
                    shortpnl += newNav - lastNAV;
                    lastNAV = newNav;
                }
                else if (lastPosition > 50 && newPos < 50)
                {
                    longCrossings++;
                    longpnl += newNav - lastNAV;
                    lastNAV = newNav;
                }
                lastPosition = newPos;
            }
        }

        public void Finalize(double finalNav)
        {
            if (lastPosition > 50)
                longpnl += finalNav - lastNAV;
            else if (lastPosition < 50)
                shortpnl += finalNav - lastNAV;
        }
    }
}
