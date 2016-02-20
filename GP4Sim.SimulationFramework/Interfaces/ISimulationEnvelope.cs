using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GP4Sim.SimulationFramework.Interfaces
{
    public interface ISimulationEnvelope
    {
        IEnumerable<double> RawOutput { get; }
        double FitnessScore { get; }
    }
}
