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

namespace GP4Sim.Trading.Instances.Views
{
    partial class TradingImportTypeDialog
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed 
        /// should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            ((System.ComponentModel.ISupportInitialize)(this.TrainingTestTrackBar)).BeginInit();
            this.CSVSettingsGroupBox.SuspendLayout();
            this.ProblemDataSettingsGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // SeparatorInfoLabel
            // 
            this.ToolTip.SetToolTip(this.SeparatorInfoLabel, "Select the separator used to separate columns in the csv file.");
            // 
            // DateTimeFormatInfoLabel
            // 
            this.ToolTip.SetToolTip(this.DateTimeFormatInfoLabel, "Select the date time format used in the csv file");
            // 
            // DecimalSeparatorInfoLabel
            // 
            this.ToolTip.SetToolTip(this.DecimalSeparatorInfoLabel, "Select the decimal separator used to for double values");
            // 
            // ShuffelInfoLabel
            // 
            this.ToolTip.SetToolTip(this.ShuffelInfoLabel, "Check, if the importat data should be shuffled");
            // 
            // ConcreteImportTypeDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(471, 442);
            this.Name = "ConcreteImportTypeDialog";
            this.Text = "Concrete CSV Import";
            ((System.ComponentModel.ISupportInitialize)(this.TrainingTestTrackBar)).EndInit();
            this.CSVSettingsGroupBox.ResumeLayout(false);
            this.CSVSettingsGroupBox.PerformLayout();
            this.ProblemDataSettingsGroupBox.ResumeLayout(false);
            this.ProblemDataSettingsGroupBox.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        //protected System.Windows.Forms.ComboBox TargetVariableComboBox;
        //protected System.Windows.Forms.Label TargetVariableLabel;
        //protected System.Windows.Forms.Label TargetVariableInfoLabel;


    }
}