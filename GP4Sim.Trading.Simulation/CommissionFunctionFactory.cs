using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GP4Sim.Trading.Simulation
{
    public static class CommissionFunctionFactory
    {
        public static CommissionFunction Create(CommissionFunctionsEnum enumValue)
        {
            if (enumValue == CommissionFunctionsEnum.NONE)
                return CommissionFunctions.NONE;
            else if (enumValue == CommissionFunctionsEnum.FX_USD)
                return CommissionFunctions.FX_USD;
            else if (enumValue == CommissionFunctionsEnum.FX_JPY)
                return CommissionFunctions.FX_JPY;
            else if (enumValue == CommissionFunctionsEnum.FX_JPY_WRONG)
                return CommissionFunctions.FX_JPY_WRONG;
            else
                return CommissionFunctions.NONE;
        }


        public static CommissionFunctionValue[] List()
        {
            List<CommissionFunctionValue> cList = new List<CommissionFunctionValue>();

            foreach (string ename in Enum.GetNames(typeof(CommissionFunctionsEnum)))
            {
                cList.Add(new CommissionFunctionValue((CommissionFunctionsEnum)Enum.Parse(typeof(CommissionFunctionsEnum), ename)));
            }

            return cList.ToArray();
        }

        /*
        public static Dictionary<CommissionFunctionsEnum, CommissionFunction> CFDictionary
        {
            get
            {
                Dictionary<CommissionFunctionsEnum, CommissionFunction> cfDict = new Dictionary<CommissionFunctionsEnum, CommissionFunction>();

                foreach (string ename in Enum.GetNames(typeof(CommissionFunctionsEnum)))
                {
                    MethodInfo cfMethodInfo = typeof(CommissionFunctions).GetMethod(ename);
                    CommissionFunction cf = (CommissionFunction) Delegate.CreateDelegate(typeof(CommissionFunction), cfMethodInfo);

                    cfDict.Add((CommissionFunctionsEnum) Enum.Parse(typeof(CommissionFunctionsEnum), ename), cf);
                }

                return cfDict;
            }
        }
        */
    }
}
