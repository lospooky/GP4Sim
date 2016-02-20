using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.Problems.Instances;
using HeuristicLab.Problems.DataAnalysis;
using System.Collections;
using GP4Sim.SimulationFramework.Interfaces;

namespace GP4Sim.SimulationFramework.Instances
{
    public abstract class SimulationDataDescriptor<T> : IDataDescriptor
        where T : ISimulationProblemData
    {
        public abstract string Name { get; }
        public abstract string Description { get; }


        protected abstract string[] VariableNames { get; }
        protected abstract string[] AllowedInputVariables { get; }
        protected abstract int TrainingPartitionStart { get; }
        protected abstract int TrainingPartitionEnd { get; }
        protected abstract int TestPartitionStart { get; }
        protected abstract int TestPartitionEnd { get; }

        public abstract T GenerateData(Dataset dataset);

        public abstract T GenerateData();


        protected abstract List<IList> GenerateValues();
    }
}
