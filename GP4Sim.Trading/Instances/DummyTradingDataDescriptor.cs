using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.Problems.Instances.DataAnalysis;
using HeuristicLab.Random;

namespace GP4Sim.Trading.Instances
{
    public class DummyTradingDataDescriptor : TradingDataDescriptor
    {
        public override string Name
        {
            get { return "Dummy Data Descriptor - Sinc"; }
        }

        public override string Description
        {
            get { return ""; }
        }

        protected override string PriceVariable { get { return "D"; } }
        protected override string TimePointVariable { get { return "DATE"; } }
        protected override string[] VariableNames { get { return new string[] { "DATE", "D", "X" }; } }
        protected override string[] AllowedInputVariables { get { return new string[] { "D", "X" }; } }
        protected override int TrainingPartitionStart { get { return 0; } }
        protected override int TrainingPartitionEnd { get { return 20; } }
        protected override int TestPartitionStart { get { return 20; } }
        protected override int TestPartitionEnd { get { return 520; } }


        protected override List<IList> GenerateValues()
        {
            List<IList> data = new List<IList>();

            List<double> x = ValueGenerator.GenerateUniformDistributedValues(520, -1, 1).ToList();
            List<double> results = new List<double>();
            List<DateTime> timePoints = new List<DateTime>();
            for (int i = 0; i < x.Count; i++)
            {
                double val = x[i];
                results.Add(val != 0.0 ? Math.Sin(val) / val : 1.0);
                timePoints.Add(DateTime.Today);
            }

            data.Add(timePoints);
            data.Add(x);
            data.Add(results);
            return data;
        }
    }
}
