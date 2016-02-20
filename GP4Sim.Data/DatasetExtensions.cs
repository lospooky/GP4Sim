using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HeuristicLab.Problems.DataAnalysis
{
    public static class DatasetExtensions
    {
        public static DateTime GetDateTimeValue(this IDataset dataset, string variableName, int row)
        {
            List<DateTime> dateTimeValues = dataset.GetDateTimeValues(variableName).ToList();
            if (dateTimeValues.Count < row + 1)
                throw new ArgumentException("Index " + row + " for variable " + variableName + " is not a valid.");
            else
                return dateTimeValues[row];
        }

        public static IEnumerable<string> DateTimeVariables(this Dataset dataset)
        {
            List<string> result = new List<string>();
            List<string> variableNames = dataset.VariableNames.ToList();

            foreach (string variable in variableNames)
            {
                if (dataset.VariableHasType<DateTime>(variable))
                    result.Add(variable);
            }

            return result;
        }
    }
}