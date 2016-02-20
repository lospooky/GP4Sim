using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GP4Sim.Trading.Simulation
{
    public class LogLineEventArgs : EventArgs
    {
        public string Line { get; private set; }

        public LogLineEventArgs(string line)
        {
            this.Line = line;
        }
    }
}
