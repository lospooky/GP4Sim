using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GP4Sim.Trading.Simulation
{
    public class BuyHoldStats
    {
        private double bhBuyPrice = 0;
        private double bhSellPrice = 0;
        private double bhReturn = 0;


        public BuyHoldStats()
        {

        }

        public double BHBuyPrice
        {
            get { return bhBuyPrice; }
            set { bhBuyPrice = value; }
        }

        public double BHSellPrice
        {
            get { return bhSellPrice; }
            set { bhSellPrice = value; }
        }

        public double BHReturn
        {
            get { return bhReturn; }
            set { bhReturn = value; }
        }
    }
}
