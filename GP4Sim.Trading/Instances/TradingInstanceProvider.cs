using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GP4Sim.SimulationFramework.Instances;
using GP4Sim.Trading.Interfaces;

namespace GP4Sim.Trading.Instances
{
    public abstract class TradingInstanceProvider : SimulationInstanceProvider<ITradingProblemData, TradingImportType>
    {
        protected const string DefaultTimePointVariable = "DATE";
        protected const string DefaultPriceVariable = "PRICE";
    }
}
