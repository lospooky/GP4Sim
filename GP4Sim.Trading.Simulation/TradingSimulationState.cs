using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GP4Sim.Trading.Simulation
{
    public delegate void OutOfMoneyEventHandler(object sender, EventArgs e);

    public class TradingSimulationState
    {
        public event OutOfMoneyEventHandler OutOfMoney;

        #region Variables

        private double initialNAV;
        private double accountValue;
        private double avgCost;
        private double posQuantity;
        private double currentPrice;
        private double currentPositionValue;
        private float posAsNavPerc;
        private double accountQuantity;
        private double accountAvgCost;
        private double currentNAV;
        bool initialized;

        #endregion

        #region Constructor

        public TradingSimulationState(double initialNav)
        {
            initialized = false;
            accountValue = initialNav;
            this.initialNAV = initialNav;
            avgCost = 0;
            posQuantity = 0;
            currentPrice = 0;
            accountQuantity = 0;
            accountAvgCost = 0;
        }

        #endregion

        #region Public Methods

        public void SetCurrentPrice(double newPrice)
        {
            if (!initialized)
            {
                initialized = true;
                
                
                double totalQuantity = (accountValue / newPrice);

                accountQuantity = totalQuantity / 2;
                posQuantity = accountQuantity;

                avgCost = newPrice;
                accountAvgCost = newPrice;
                // 50/50

            }

            currentPrice = newPrice;
            Update();
        }

        public void Buy(double deltaAccountValue, long deltaPosition)
        {
            double deltaAccountQuantity = (accountQuantity * deltaAccountValue) / AccountValue;
            accountQuantity -= deltaAccountQuantity;
            if (accountQuantity == 0)
                accountAvgCost = 0;

            avgCost = (deltaAccountValue + (avgCost * Math.Abs(posQuantity))) / (Math.Abs(posQuantity) + deltaPosition);
            posQuantity += deltaPosition;
            Update();
            return;
        }

        public void Sell(double deltaAccountValue, long deltaPosition)
        {
            double deltaAccountQuantity = deltaAccountValue / currentPrice;
            accountAvgCost = (deltaAccountValue + (accountAvgCost * Math.Abs(accountQuantity))) / (Math.Abs(accountQuantity) + deltaAccountQuantity);
            accountQuantity += deltaAccountQuantity;

            posQuantity -= deltaPosition;
            if (posQuantity == 0)
                avgCost = 0;

            Update();
            return;
        }

        #endregion


        #region Private Methods

        private void Update()
        {
            accountValue = UpdatedAccountValue;
            currentPositionValue = UpdatedCurrentPositionValue;
            currentNAV = UpdatedNAV;
            posAsNavPerc = UpdatePosAsNavPerc;
        }

        #endregion

        #region Private Properties

        private double UpdatedAccountValue
        {
            get { return (2 * accountAvgCost - currentPrice) * Math.Abs(accountQuantity); }
        }

        private double UpdatedCurrentPositionValue
        {
            get
            {
                if (posQuantity > 0)
                    return (currentPrice * posQuantity);
                else
                    return 0;
            }
        }

        private double UpdatedNAV
        {
            get { return AccountValue + CurrentPositionValue; }
        }

        private float UpdatePosAsNavPerc
        {
            get
            {
                if (PosQuantity > 0)
                    return (float)(CurrentPositionValue / NAV) * 100;
                else
                    return 0;
            }
        }

        #endregion

        /*
        protected virtual void OnOutOfMoney(EventArgs e)
        {
            if (OutOfMoney != null)
                OutOfMoney(this, e);
        }
        */

        #region Public Properties

        public double AccountValue { get { return accountValue; } }

        public double CurrentPositionValue { get { return currentPositionValue; } }

        public double NAV { get { return currentNAV; } }

        public double PosQuantity { get { return posQuantity; } }

        public double PosAvgCost { get { return avgCost; } }

        public float PosAsNavPerc { get { return posAsNavPerc; } }

        public double AccountAvgCost { get { return accountAvgCost; } }

        public double AccountQty { get { return accountQuantity; } }

        public double InitialNAV { get { return initialNAV; } }

        #endregion
    }
}
