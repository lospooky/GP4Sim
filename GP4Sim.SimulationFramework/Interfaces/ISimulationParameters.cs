using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.Core;

namespace GP4Sim.SimulationFramework.Interfaces
{
    public interface ISimulationParameters : INamedItem
    {
        event EventHandler Changed;
    }
}
