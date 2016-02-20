using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GP4Sim.SimulationFramework.Instances;
using GP4Sim.Trading.Interfaces;
using GP4Sim.Trading.Problem;
using HeuristicLab.Problems.DataAnalysis;

namespace GP4Sim.Trading.Instances
{
    public abstract class TradingDataDescriptor : SimulationDataDescriptor<ITradingProblemData>
    {
        public override ITradingProblemData GenerateData(Dataset dataset)
        {
            TradingProblemData regData = new TradingProblemData(dataset, AllowedInputVariables, PriceVariable, TimePointVariable);
            regData.Name = this.Name;
            regData.Description = this.Description;
            regData.TrainingPartition.Start = this.TrainingPartitionStart;
            regData.TrainingPartition.End = this.TrainingPartitionEnd;
            regData.TestPartition.Start = this.TestPartitionStart;
            regData.TestPartition.End = this.TestPartitionEnd;
            return regData;
        }

        public override ITradingProblemData GenerateData()
        {
            Dataset dataset = new Dataset(VariableNames, this.GenerateValues());


            return GenerateData(dataset);
        }

        protected abstract string PriceVariable { get; }
        protected abstract string TimePointVariable { get; }
    }
}
