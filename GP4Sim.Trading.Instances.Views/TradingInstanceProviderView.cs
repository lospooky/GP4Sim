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
using System.IO;
using System.Windows.Forms;
using GP4Sim.Trading.Instances;
using GP4Sim.Trading.Interfaces;
using HeuristicLab.MainForm;
using HeuristicLab.Problems.DataAnalysis;
using HeuristicLab.Problems.Instances;
using HeuristicLab.Problems.Instances.DataAnalysis.Views;

namespace GP4Sim.Trading.Instances.Views
{
    [View("Trading InstanceProvider View")]
    //[Content(typeof(IProblemInstanceProvider<ITradingProblemData>), IsDefaultView = true)]
    [Content(typeof(TradingInstanceProvider), IsDefaultView=true)]
    public partial class TradingInstanceProviderView : DataAnalysisInstanceProviderView<ITradingProblemData>
    {
        public new IProblemInstanceProvider<ITradingProblemData> Content
        {
            get { return (IProblemInstanceProvider<ITradingProblemData>)base.Content; }
            set { base.Content = value; }
        }

        public TradingInstanceProviderView()
        {
            InitializeComponent();
        }

        protected override void importButton_Click(object sender, EventArgs e)
        {
            var provider = Content as TradingInstanceProvider;
            if (provider != null)
            {
                if (provider is TradingCSVInstanceProvider)
                {
                    TradingImportTypeDialog importTypeDialog = new TradingImportTypeDialog();
                    if (importTypeDialog.ShowDialog() == DialogResult.OK)
                    {
                        ITradingProblemData instance = null;
                        try
                        {
                            instance = provider.ImportData(importTypeDialog.Path, importTypeDialog.ImportType, importTypeDialog.CSVFormat);
                        }
                        catch (IOException ex)
                        {
                            ErrorWhileParsing(ex);
                            return;
                        }
                        try
                        {
                            GenericConsumer.Load(instance);
                        }
                        catch (IOException ex)
                        {
                            ErrorWhileLoading(ex, importTypeDialog.Path);
                        }
                    }
                    else
                    {
                        return;
                    }
                }
            }
            else
            {
                base.importButton_Click(sender, e);
            }
        }
    }
}
