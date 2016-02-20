using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GP4Sim.Trading.Simulation
{
    public enum FitnessErrorCodes
    {
        //From Worse To Best
        NAV_IS_NAN = -4, INVALID_OUTPUT = -3, TOO_FEW_TRADES = -2, IN_THE_RED = -1
    }
}
