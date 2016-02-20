using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GP4Sim.Trading.Interfaces;
using GP4Sim.Trading.Problem;
using HeuristicLab.Problems.DataAnalysis;
using HeuristicLab.Problems.Instances;

namespace GP4Sim.Trading.Instances
{
    public class TradingTimeBaseInstanceProvider : TradingInstanceProvider
    {
        public override string Name
        {
            get { return "TimeBase"; }
        }

        public override string Description
        {
            get
            {
                return "Imports data from a local or remote TimeBase instance";
            }
        }
        public override Uri WebLink
        {
            get { return null; }
        }
        public override string ReferencePublication
        {
            get { return ""; }
        }

        public override IEnumerable<IDataDescriptor> GetDataDescriptors()
        {
            return new List<IDataDescriptor>();
        }
        public override ITradingProblemData LoadData(IDataDescriptor descriptor)
        {
            throw new NotImplementedException();
        }

        public override bool CanImportData
        {
            get { return true; }
        }

        public ITradingProblemData ProblemDataFromDataset(Dataset ds)
        {
            int length = ds.Rows;
            List<string> allowedInputVars = ds.DoubleVariables.ToList();
            IEnumerable<string> dtvars = ds.DateTimeVariables();
            string priceVariable = ds.DoubleVariables.Contains(DefaultPriceVariable) ? DefaultPriceVariable : ds.DoubleVariables.Last();
            string timePointVariable =dtvars.Contains(DefaultTimePointVariable) ? DefaultTimePointVariable : dtvars.First();

            TradingProblemData problemData = new TradingProblemData(ds, allowedInputVars, priceVariable, timePointVariable);
            problemData.TrainingPartition.Start = 0;
            problemData.TrainingPartition.End = (int)Math.Floor(length * 0.7);
            problemData.TestPartition.Start = problemData.TrainingPartition.End + 1;
            problemData.TestPartition.End = length;
            problemData.Name = "TimeBase Dataset";

            return problemData;



        }
    }
}
