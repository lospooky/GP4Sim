using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GP4Sim.SimulationFramework.Interfaces;
using GP4Sim.Trading.Simulation;

namespace GP4Sim.Trading.Interfaces
{
    public interface ITradingModel : ISimulationModel<ITradingProblemData>
    {
        //IEnumerable<double> GetDailyNavValues(IConcreteProblemData problemData, IEnumerable<int> rows);
        //IEnumerable<double> GetDailyInstrValues(IConcreteProblemData problemData, IEnumerable<int> rows);


        double[] GetDailyNavPoints(ITradingProblemData problemData, IEnumerable<int> rows);
        double[] GetDailyInstrPoints(ITradingProblemData problemData, IEnumerable<int> rows);
        DateTime[] GetDayPoints(ITradingProblemData problemData, IEnumerable<int> rows);
        string GetSimulationLog(ITradingProblemData problemData, IEnumerable<int> rows);

        ITradingSolution CreateSolution(ITradingProblemData problemData);
    }
}
