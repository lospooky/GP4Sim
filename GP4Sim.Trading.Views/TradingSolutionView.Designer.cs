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

using HeuristicLab.Problems.DataAnalysis.Views;
namespace GP4Sim.Trading.Views
{
    partial class TradingSolutionView : DataAnalysisSolutionView
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        /// 

        #if Limited==false
        private System.Windows.Forms.Button btnSimplify;
        private System.Windows.Forms.Button ExcelExportButton;
        private System.Windows.Forms.Button ExportAgentButton;
        private System.Windows.Forms.Button LogButton;
        #endif

        private void InitializeComponent()
        {
            #if Limited == false
            this.btnSimplify = new System.Windows.Forms.Button();
            this.ExportAgentButton = new System.Windows.Forms.Button();
            this.ExcelExportButton = new System.Windows.Forms.Button();
            this.LogButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            #endif

            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.itemsGroupBox.SuspendLayout();
            this.detailsGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer
            // 
            // 
            // splitContainer.Panel1
            //
#if Limited==false
            this.splitContainer.Panel1.Controls.Add(this.btnSimplify);
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.LogButton);
            this.splitContainer.Panel2.Controls.Add(this.ExcelExportButton);
            this.splitContainer.Panel2.Controls.Add(this.ExportAgentButton);
#endif
            this.splitContainer.Size = new System.Drawing.Size(544, 275);
            this.splitContainer.SplitterDistance = 255;
            // 
            // itemsGroupBox
            // 
            this.itemsGroupBox.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.itemsGroupBox.Size = new System.Drawing.Size(550, 294);
            this.itemsGroupBox.Text = "Concrete Solution";
            // 
            // itemsListView
            // 
            this.itemsListView.Size = new System.Drawing.Size(249, 0);
            // 
            // detailsGroupBox
            // 
            this.detailsGroupBox.Size = new System.Drawing.Size(149, 0);
            // 
            // addButton
            // 
            this.toolTip.SetToolTip(this.addButton, "Add");
            // 
            // removeButton
            // 
            this.toolTip.SetToolTip(this.removeButton, "Remove");
            // 
            // viewHost
            // 
            this.viewHost.Size = new System.Drawing.Size(137, 0);
            // 
            // btnSimplify
            //
#if Limited == false
            this.btnSimplify.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSimplify.Location = new System.Drawing.Point(177, 4);
            this.btnSimplify.Name = "btnSimplify";
            this.btnSimplify.Size = new System.Drawing.Size(75, 23);
            this.btnSimplify.TabIndex = 6;
            this.btnSimplify.Text = "Simplify";
            this.btnSimplify.UseVisualStyleBackColor = true;
            this.btnSimplify.Click += new System.EventHandler(this.btn_SimplifyModel_Click);
            // 
            // ExportAgentButton
            // 
            this.ExportAgentButton.Location = new System.Drawing.Point(3, 4);
            this.ExportAgentButton.Name = "ExportAgentButton";
            this.ExportAgentButton.Size = new System.Drawing.Size(75, 23);
            this.ExportAgentButton.TabIndex = 7;
            this.ExportAgentButton.Text = "Export C#";
            this.ExportAgentButton.UseVisualStyleBackColor = true;
            this.ExportAgentButton.Click += new System.EventHandler(this.ExportAgentButton_Click);
            // 
            // ExcelExportButton
            // 
            this.ExcelExportButton.Location = new System.Drawing.Point(92, 4);
            this.ExcelExportButton.MinimumSize = new System.Drawing.Size(80, 23);
            this.ExcelExportButton.Name = "ExcelExportButton";
            this.ExcelExportButton.Size = new System.Drawing.Size(80, 23);
            this.ExcelExportButton.TabIndex = 8;
            this.ExcelExportButton.Text = "Export Excel";
            this.ExcelExportButton.UseVisualStyleBackColor = true;
            this.ExcelExportButton.Click += new System.EventHandler(this.ExcelExportButton_Click);
            // 
            // LogButton
            // 
            this.LogButton.Location = new System.Drawing.Point(186, 3);
            this.LogButton.Name = "LogButton";
            this.LogButton.Size = new System.Drawing.Size(75, 23);
            this.LogButton.TabIndex = 9;
            this.LogButton.Text = "Log";
            this.LogButton.UseVisualStyleBackColor = true;
            this.LogButton.Click += new System.EventHandler(this.LogButton_Click);
#endif
            // 
            // TradingSolutionView
            // 
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Name = "TradingSolutionView";
            this.Size = new System.Drawing.Size(550, 294);
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            this.itemsGroupBox.ResumeLayout(false);
            this.detailsGroupBox.ResumeLayout(false);
            this.ResumeLayout(false);
           

        }

        #endregion


    }
}
