using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GP4Sim.SimulationFramework.Interfaces
{
    public interface ISimulationCalculator<T, U>
        where T : class, ISimulationProblemData
        where U : class, ISimulationEnvelope
    {
        //Statics in Interface: NO CAN DO
        //public static double FitnessScore();
        //public static U AnalysisResults();
    }
}
