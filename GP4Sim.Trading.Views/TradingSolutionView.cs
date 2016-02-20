#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2012 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
 *
 * This file is part of HeuristicLab.
 *
 * HeuristicLab is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * HeuristicLab is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with HeuristicLab. If not, see <http://www.gnu.org/licenses/>.
 */
#endregion

using System;
using System.Windows.Forms;
using HeuristicLab.MainForm;
using HeuristicLab.Problems.DataAnalysis.Views;
using HeuristicLab.Core;
using System.IO;
using GP4Sim.CSharpAgents;
using GP4Sim.Trading.Solutions;
using GP4Sim.Trading.Problem;
using GP4Sim.Excel;


namespace GP4Sim.Trading.Views
{
    [Content(typeof(TradingSolution), false)]
    [View("ConcreteSolution View")]
    public partial class TradingSolutionView : DataAnalysisSolutionView
    {
        public TradingSolutionView()
        {
            InitializeComponent();

            this.SortItemsListView(SortOrder.None);


        }

        protected new TradingSolution Content
        {
            get { return (TradingSolution)base.Content; }
            set { base.Content = value; }
        }


        #region drag and drop
        protected override void itemsListView_DragEnter(object sender, DragEventArgs e)
        {
            validDragOperation = false;
            if (ReadOnly) return;

            var dropData = e.Data.GetData(HeuristicLab.Common.Constants.DragDropDataFormat);
            if (dropData is TradingProblemData) validDragOperation = true;
            else if (dropData is IValueParameter)
            {
                var param = (IValueParameter)dropData;
                if (param.Value is TradingProblemData) validDragOperation = true;
            }
        }
        #endregion

        protected void btn_SimplifyModel_Click(object sender, EventArgs e)
        {



        }

        private void ExportAgentButton_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "C# File (*.cs) | *.cs";
            sfd.FileName = Content.DescriptiveName;
            sfd.AddExtension = true;
            sfd.AutoUpgradeEnabled = true;
            sfd.DefaultExt = ".cs";
            DialogResult result = sfd.ShowDialog();
            if (result == DialogResult.OK)
            {
                //CSharpFormatter csf = new CSharpFormatter();
                CSharpFormatterNew csf = new CSharpFormatterNew();
                string source = csf.FormatFull(Content.Model.SymbolicExpressionTree, Content.ActualInputVector);
                string filename = sfd.FileName.ToLower();
                try
                {
                    File.WriteAllText(filename, source);
                }
                catch (Exception ex) { }

            }
        }

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Excel File (*.xlsx) | *.xlsx";
            sfd.FileName = Content.DescriptiveName;
            sfd.AddExtension = true;
            sfd.AutoUpgradeEnabled = true;
            sfd.DefaultExt = ".cs";


            DialogResult result = sfd.ShowDialog();
            if (result == DialogResult.OK)
            {
                ExcelReportFactory.CreateAndSaveReport(Content, sfd.FileName);
            }
        }

        private void LogButton_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Text File (*.txt) | *.txt";
            sfd.FileName = Content.DescriptiveName + "_Log";
            sfd.AddExtension = true;
            sfd.AutoUpgradeEnabled = true;
            sfd.DefaultExt = ".txt";
            DialogResult result = sfd.ShowDialog();

            if (result == DialogResult.OK)
            {
                string logText = Content.Model.GetSimulationLog(Content.ProblemData, Content.ProblemData.TrainingIndices);

                string filename = sfd.FileName.ToLower();
                try
                {
                    File.WriteAllText(filename, logText);
                }
                catch (Exception ex) { }
            }
        }

        private void MCButton_Click(object sender, EventArgs e)
        {

            Content.PerformMonteCarloEvaluation(Content.ProblemData.MonteCarloSets(0, true));
        }
    }
}
