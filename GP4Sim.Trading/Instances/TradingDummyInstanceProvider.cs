using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GP4Sim.Trading.Interfaces;
using HeuristicLab.Problems.Instances;

namespace GP4Sim.Trading.Instances
{
    public class TradingDummyInstanceProvider : TradingInstanceProvider
    {
        public override string Name
        {
            get { return "Dummy Problem"; }
        }
        public override string Description
        {
            get { return ""; }
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
            List<IDataDescriptor> descriptorList = new List<IDataDescriptor>();
            descriptorList.Add(new DummyTradingDataDescriptor());
            return descriptorList;
        }

        public override ITradingProblemData LoadData(IDataDescriptor descriptor)
        {
            return ((TradingDataDescriptor)descriptor).GenerateData();
        }

    }
}
