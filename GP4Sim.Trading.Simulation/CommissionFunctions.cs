using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GP4Sim.Trading.Simulation
{
    public enum CommissionFunctionsEnum
    {
        NONE = 0, FX_USD = 1, FX_JPY = 2, FX_JPY_WRONG = 3
    }

    public delegate double CommissionFunction(double quantity, double unitPrice);

    public static class CommissionFunctions
    {
        public static double NONE(double quantity, double unitPrice)
        {
            return 0;
        }

        public static double FX_USD(double quantity, double unitPrice)
        {
            //CITI RATES
            float bp = 0.0001f;
            float rate = 0.15f;


            //float minCost = 2.50f;

            double proportional = rate * bp * (quantity * unitPrice);

            //if(proportional <= minCost)
            //return minCost;
            //else
            return proportional;
        }

        public static double FX_JPY(double quantity, double unitPrice)
        {
            float bp = 0.0001f;
            float rate = 0.15f;
            //float minCost = 205.45f;

            double proportional = rate * bp * (quantity * unitPrice);

            //if(proportional <= minCost)
            //return minCost;
            //else
            return proportional;
        }

        public static double FX_JPY_WRONG(double quantity, double unitPrice)
        {
            float bp = 0.0001f;
            float rate = 0.2f;
            float minCost = 205.45f;

            double proportional = rate * bp * (quantity * unitPrice);

            if (proportional <= minCost)
                return minCost;
            else
                return proportional;
        }

    }
}
