using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.Problems.DataAnalysis;


namespace GP4Sim.Trading.Simulation
{
    public class Pacemaker
    {
        private IEnumerable<int> range;
        private int currentRow = 0;
        private int currentProgressiveRow = 0;
        private int lastRow = 0;
        private string priceVariableName;
        private string dateVariableName;
        private IDataAnalysisProblemData problemData;
        private DateTime previousTimePoint = DateTime.MinValue;
        private DateTime currentTimePoint = DateTime.MinValue;
        private double currentPrice;
        private bool invertPrice;

        public Pacemaker(IDataAnalysisProblemData problemData, bool invertPrices, IEnumerable<int> rows, string dvn, string pvn)
        {
            this.dateVariableName = dvn;
            this.priceVariableName = pvn;
            this.problemData = problemData;
            this.invertPrice = invertPrices;
            currentRow = rows.First();
            lastRow = rows.Last();
            range = rows;
            Advance();
        }

        public bool AdjustForLag(int maxLag)
        {
            int lagSteps = Math.Abs(maxLag);
            for (int i = 0; i < lagSteps; i++)
            {
                if (Next() == false)
                    return false;
            }
            return true;
        }

        public bool Next()
        {
            if (currentRow == lastRow)
                return false;

            currentRow++;
            currentProgressiveRow++;
            Advance();
            return true;
        }

        public int CurrentRow { get { return currentRow; } }

        public DateTime CurrentTimePoint { get { return currentTimePoint; } }

        public double CurrentPrice { get { return currentPrice; } }

        public double CurrentDatapoint { get { return currentProgressiveRow; } }

        public bool IsNewDay
        {
            get
            {
                if (previousTimePoint != DateTime.MinValue && currentTimePoint != DateTime.MinValue)
                    if (previousTimePoint.Day != currentTimePoint.Day)
                        return true;
                return false;
            }
        }

        private DateTime GetTimePoint(int row)
        {
            return problemData.Dataset.GetDateTimeValue(dateVariableName, currentRow);
        }

        private double GetPrice(int row)
        {
            double value = problemData.Dataset.GetDoubleValue(priceVariableName, currentRow);
            if (invertPrice)
                return 1 / value;

            return value;
        }

        private void Advance()
        {
            previousTimePoint = currentTimePoint;
            currentTimePoint = GetTimePoint(currentRow);
            currentPrice = GetPrice(currentRow);
        }

    }
}
