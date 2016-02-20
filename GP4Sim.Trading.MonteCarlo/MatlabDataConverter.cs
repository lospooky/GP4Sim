using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GP4Sim.Data;
using HeuristicLab.Data;
using HeuristicLab.Problems.DataAnalysis;
using MathWorks.MATLAB.NET.Arrays;

namespace GP4Sim.Trading.MonteCarlo
{
    public static class MatlabDataConverter
    {
        private static string[] matlabStructFields = { "Name", "Open", "High", "Low", "Close" };

        public static MWStructArray ToMatlabArray(IDataset data, IEnumerable<int> trainingRows)
        {
            List<string> varNames = data.DoubleVariables.ToList();
            List<string> securityNames = VarNamesValidation(varNames);

            if (securityNames == null)
                throw new ArgumentException();
            else
            {
                int nDataPoints = trainingRows.Count();

                MWStructArray mdata = new MWStructArray(1, securityNames.Count(), matlabStructFields);
                
                int colCounter = 0;
                
                for (int i = 0; i < securityNames.Count(); i++)
                {
                    mdata["Name", i + 1] = securityNames[i];
                    mdata["Open", i + 1] = new MWNumericArray(nDataPoints, 1, data.GetDoubleValues(varNames[colCounter++], trainingRows).ToArray());
                    mdata["High", i + 1] = new MWNumericArray(nDataPoints, 1, data.GetDoubleValues(varNames[colCounter++], trainingRows).ToArray());
                    mdata["Low", i + 1] = new MWNumericArray(nDataPoints, 1, data.GetDoubleValues(varNames[colCounter++], trainingRows).ToArray());
                    mdata["Close", i + 1] = new MWNumericArray(nDataPoints, 1, data.GetDoubleValues(varNames[colCounter++], trainingRows).ToArray());
                }

                return mdata;
            }
        }

        public static List<Tuple<List<string>, List<IList>>> FromMatlabArray(MWStructArray matlabOutput)
        {
            int[] dims = matlabOutput.Dimensions;
            int nSets = dims[0];
            int nSecs = dims[1];

            List<Tuple<List<string>, List<IList>>> dataSets = new List<Tuple<List<string>, List<IList>>>();
            for (int i = 1; i <= nSets; i++)
            {
                List<string> vNames = new List<string>();
                List<IList> vValues = new List<IList>();

                for (int j = 1; j <= nSecs; j++)
                {
                    string secName = ((MWArray)matlabOutput["Name", i, j]).ToString();

                    vNames.Add(secName + "_OPEN");
                    vNames.Add(secName + "_HIGH");
                    vNames.Add(secName + "_LOW");
                    vNames.Add(secName + "_CLOSE");


                    Array a = ((MWNumericArray)matlabOutput["Open", i, j]).ToArray(MWArrayComponent.Real);

                    IList d = a.OfType<double>().ToList();


                    vValues.Add(((MWNumericArray)matlabOutput["Open", i, j]).ToArray(MWArrayComponent.Real).OfType<double>().ToList());
                    vValues.Add(((MWNumericArray)matlabOutput["High", i, j]).ToArray(MWArrayComponent.Real).OfType<double>().ToList());
                    vValues.Add(((MWNumericArray)matlabOutput["Low", i, j]).ToArray(MWArrayComponent.Real).OfType<double>().ToList());
                    vValues.Add(((MWNumericArray)matlabOutput["Close", i, j]).ToArray(MWArrayComponent.Real).OfType<double>().ToList());
                }

                dataSets.Add(new Tuple<List<string>, List<IList>>(vNames, vValues));
            }

            return dataSets;
        }

        private static List<string> VarNamesValidation(List<string> varNames)
        {
            if (varNames.Count % 4 != 0)
                return null;
            else
            {
                Dictionary<string, int> counter = new Dictionary<string, int>();

                foreach (string s in varNames)
                {
                    string[] vn = s.Split('_');
                    if (vn == null || vn.Count() != 2)
                        return null;
                    else if (!counter.ContainsKey(vn[0]))
                        counter.Add(vn[0], 0);
                    
                   if(matlabStructFields.Contains(vn[1], StringComparer.InvariantCultureIgnoreCase))
                       counter[vn[0]]++;
                }

                foreach (string security in counter.Keys)
                    if (counter[security] != 4)
                        return null;

                return counter.Keys.ToList();
            }
        }
    }
}
