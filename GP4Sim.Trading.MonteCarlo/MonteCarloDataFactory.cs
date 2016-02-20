using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.Data;
using HeuristicLab.Problems.DataAnalysis;
using MathWorks.MATLAB.NET.Arrays;
using MonteCarlo;

namespace GP4Sim.Trading.MonteCarlo
{
    public static class MonteCarloDataFactory
    {
        public static List<Dataset> GenerateMonteCarloData(IDataset dataset, IntRange range, int nSamples, int nSets, string timePointVariable)
        {

            IEnumerable<int> rows = Enumerable.Range(range.Start, range.Size);

            MWStructArray matlabInput = MatlabDataConverter.ToMatlabArray(dataset, rows);

            MonteCarloDataGenerator mcdg = new MonteCarloDataGenerator();

            MWStructArray matlabOutput = (MWStructArray)mcdg.Main(matlabInput, range.Size, nSamples, nSets);

            List<Tuple<List<string>, List<IList>>> rawSets = MatlabDataConverter.FromMatlabArray(matlabOutput);


            List<DateTime> timeStamps = Enumerable.Range(1, nSamples+1).Select(x=> dataset.GetDateTimeValues(timePointVariable).Last().AddMinutes(5*x)).ToList();

            List<Dataset> datasets = new List<Dataset>();

            foreach (Tuple<List<string>, List<IList>> t in rawSets)
            {
                t.Item1.Add(timePointVariable);
                t.Item2.Add(timeStamps);
                datasets.Add(new Dataset(t.Item1, t.Item2));
            }

            return datasets;

        }
    }
}
