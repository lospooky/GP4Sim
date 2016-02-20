using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using GP4Sim.Data;
using GP4Sim.Trading.Interfaces;
using GP4Sim.Trading.Problem;
using HeuristicLab.Common;
using HeuristicLab.Problems.DataAnalysis;
using HeuristicLab.Problems.Instances;
using HeuristicLab.Problems.Instances.DataAnalysis;

namespace GP4Sim.Trading.Instances
{
    public class TradingCSVInstanceProvider : TradingInstanceProvider
    {
        public override string Name
        {
            get { return "CSV File"; }
        }
        public override string Description
        {
            get
            {
                return "";
            }
        }
        public override Uri WebLink
        {
            get { return null; }
        }
        public override string ReferencePublication
        {
            get { return ""; }
        }

        public override IEnumerable<IDataDescriptor> GetDataDescriptors()
        {
            return new List<IDataDescriptor>();
        }
        public override ITradingProblemData LoadData(IDataDescriptor descriptor)
        {
            throw new NotImplementedException();
        }

        public override bool CanImportData
        {
            get { return true; }
        }
        public override ITradingProblemData ImportData(string path)
        {
            TableFileParser csvFileParser = new TableFileParser();
            csvFileParser.Parse(path, true);

            Dataset dataset = new Dataset(csvFileParser.VariableNames, csvFileParser.Values);
            string targetVar = dataset.DoubleVariables.Last();
            IEnumerable<string> dtvars = dataset.DateTimeVariables();
            string tpVar = dtvars.First();

            // turn of input variables that are constant in the training partition
            var allowedInputVars = new List<string>();
            var trainingIndizes = Enumerable.Range(0, (csvFileParser.Rows * 2) / 3);
            if (trainingIndizes.Count() >= 2)
            {
                foreach (var variableName in dataset.DoubleVariables)
                {
                    //RANGE ERROR
                    if (dataset.GetDoubleValues(variableName, trainingIndizes).Range() > 0 &&
                      variableName != targetVar)
                        allowedInputVars.Add(variableName);
                }
            }
            else
            {
                allowedInputVars.AddRange(dataset.DoubleVariables.Where(x => !x.Equals(targetVar)));
            }

            ITradingProblemData ConcreteData = new TradingProblemData(dataset, allowedInputVars, dataset.VariableNames.First(), tpVar);

            var trainingPartEnd = trainingIndizes.Last();
            ConcreteData.TrainingPartition.Start = trainingIndizes.First();
            ConcreteData.TrainingPartition.End = trainingPartEnd;
            ConcreteData.TestPartition.Start = trainingPartEnd;
            ConcreteData.TestPartition.End = csvFileParser.Rows;

            ConcreteData.Name = Path.GetFileName(path);

            return ConcreteData;
        }

        protected override ITradingProblemData ImportData(string path, TradingImportType type, TableFileParser csvFileParser)
        {
            List<IList> values = csvFileParser.Values;
            if (type.Shuffle)
            {
                values = Shuffle(values);
            }
            Dataset dataset = new Dataset(csvFileParser.VariableNames, values);

            // turn of input variables that are constant in the training partition
            var allowedInputVars = new List<string>();
            int trainingPartEnd = (csvFileParser.Rows * type.TrainingPercentage) / 100;
            trainingPartEnd = trainingPartEnd > 0 ? trainingPartEnd : 1;
            var trainingIndizes = Enumerable.Range(0, trainingPartEnd);
            if (trainingIndizes.Count() >= 2)
            {
                foreach (var variableName in dataset.DoubleVariables)
                {
                    //RANGE ERROS
                    if (dataset.GetDoubleValues(variableName, trainingIndizes).Range() > 0)
                        allowedInputVars.Add(variableName);
                }
            }
            else
            {
                allowedInputVars.AddRange(dataset.DoubleVariables);
            }

            IEnumerable<string> dtvars = dataset.DateTimeVariables();
            string priceVariable = dataset.DoubleVariables.Contains(DefaultPriceVariable) ? DefaultPriceVariable : dataset.DoubleVariables.Last();           
            string timePointVariable = dtvars.Contains(DefaultTimePointVariable) ? DefaultTimePointVariable : dtvars.First();

            TradingProblemData ConcreteData = new TradingProblemData(dataset, allowedInputVars, priceVariable, timePointVariable);
            ConcreteData.TrainingPartition.Start = 0;
            ConcreteData.TrainingPartition.End = trainingPartEnd;
            ConcreteData.TestPartition.Start = trainingPartEnd;
            ConcreteData.TestPartition.End = csvFileParser.Rows;

            ConcreteData.Name = Path.GetFileName(path);

            return ConcreteData;
        }
    }
}
