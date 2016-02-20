using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GP4Sim.Trading.Simulation
{
    public class ExposureStats
    {
        public OnlineMVSCalculator Total;
        public OnlineMVSCalculator Positive;
        public OnlineMVSCalculator Negative;

        public ExposureStats()
        {
            Total = new OnlineMVSCalculator();
            Positive = new OnlineMVSCalculator();
            Negative = new OnlineMVSCalculator();
        }

        public void Update(double newValue)
        {
            Total.Add(newValue);
            if (newValue > 0)
                Positive.Add(newValue);
            else if (newValue < 0)
                Negative.Add(newValue);
        }
    }
}
