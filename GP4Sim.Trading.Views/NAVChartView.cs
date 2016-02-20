using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using GP4Sim.Trading.Interfaces;
using HeuristicLab.MainForm;
using HeuristicLab.Problems.DataAnalysis.Views;

namespace GP4Sim.Trading.Views
{
    [View("NAV Chart")]
    [Content(typeof(ITradingSolution))]
    public partial class NAVChartView : DataAnalysisSolutionEvaluationView
    {
        private static string TrNAVPointsSeriesName = "NAV - Training";
        private static string TrInstrPointsSeriesName = "Instrument - Training";

        private static string TeNAVPointsSeriesName = "NAV - Test";
        private static string TeInstrPointsSeriesName = "Instrument - Test";

        private static string ZeroLineSeriesName = "ZeroLine";

        private static string DayPointsSeriesName = "Day #";

        private static Color NAVColor = Color.FromArgb(65, 140, 240);
        private static Color InstrColor = Color.FromArgb(252, 180, 65);

        private static Color TrainingBgColor = Color.FromArgb(40, Color.LightGray);
        private static Color TestBgColor = Color.FromArgb(20, Color.LightGreen);


        #region Constructors
        public new ITradingSolution Content
        {
            get { return (ITradingSolution)base.Content; }
            set { base.Content = value; }
        }

        public NAVChartView()
            : base()
        {
            InitializeComponent();
            //configure axis
            this.chart.CustomizeAllChartAreas();
            this.chart.ChartAreas[0].CursorX.IsUserSelectionEnabled = true;
            this.chart.ChartAreas[0].AxisX.ScaleView.Zoomable = true;
            this.chart.ChartAreas[0].AxisX.IsStartedFromZero = true;
            this.chart.ChartAreas[0].CursorX.Interval = 1;


            this.chart.ChartAreas[0].CursorY.IsUserSelectionEnabled = true;
            this.chart.ChartAreas[0].AxisY.ScaleView.Zoomable = true;
            this.chart.ChartAreas[0].CursorY.Interval = 0;

        }
        #endregion

        #region Private Methods

        private void RedrawChart()
        {
            this.chart.Series.Clear();
            if (Content != null)
            {
                DateTime[] trdayValues = Content.Model.GetDayPoints(Content.ProblemData, Content.ProblemData.TrainingIndices);
                DateTime[] tedayValues = Content.Model.GetDayPoints(Content.ProblemData, Content.ProblemData.TestIndices);


                double[] trnavPoints = Content.Model.GetDailyNavPoints(Content.ProblemData, Content.ProblemData.TrainingIndices);
                double trfirstnav = trnavPoints[0];
                trnavPoints = trnavPoints.Select(x => Math.Round((x / trfirstnav) * 100, 2)).ToArray();

                double[] trinstrPoints = Content.Model.GetDailyInstrPoints(Content.ProblemData, Content.ProblemData.TrainingIndices);
                double trfirstinstr = trinstrPoints[0];
                trinstrPoints = trinstrPoints.Select(x => Math.Round((x / trfirstinstr) * 100, 2)).ToArray();

                double[] tenavPoints = Content.Model.GetDailyNavPoints(Content.ProblemData, Content.ProblemData.TestIndices);
                double tefirstnav = tenavPoints[0];
                tenavPoints = tenavPoints.Select(x => Math.Round((x / tefirstnav) * 100, 2)).ToArray();

                double[] teinstrPoints = Content.Model.GetDailyInstrPoints(Content.ProblemData, Content.ProblemData.TestIndices);
                double tefirstinstr = teinstrPoints[0];
                teinstrPoints = teinstrPoints.Select(x => Math.Round((x / tefirstinstr) * 100, 2)).ToArray();


                this.chart.ChartAreas[0].AxisX.Minimum = DateTime.Compare(trdayValues.First(), tedayValues.First()) < 0 ? trdayValues.First().ToOADate() : tedayValues.First().ToOADate();
                this.chart.ChartAreas[0].AxisX.Maximum = DateTime.Compare(trdayValues.Last(), tedayValues.Last()) < 0 ? tedayValues.Last().ToOADate() : trdayValues.Last().ToOADate();
                this.chart.ChartAreas[0].AxisX.LabelStyle.Format = "dd/MM";
                this.chart.ChartAreas[0].AxisX.Interval = trdayValues.Count() + tedayValues.Count() <= 5 ? 1 : 5;
                this.chart.ChartAreas[0].AxisX.IntervalType = DateTimeIntervalType.Days;

                double aMax = new[] { trnavPoints.Max(), trinstrPoints.Max(), tenavPoints.Max(), teinstrPoints.Max() }.Max();
                int cMax = (int)Math.Ceiling(aMax);
                double aMin = new[] { trnavPoints.Min(), trinstrPoints.Min(), tenavPoints.Min(), teinstrPoints.Min() }.Min();
                int cMin = (int)Math.Floor(aMin);
                this.chart.ChartAreas[0].AxisY.Maximum = cMax - aMax > 0.5 ? cMax : cMax + 1;
                this.chart.ChartAreas[0].AxisY.Minimum = Math.Abs(cMin - aMin) > 0.5 ? cMin : cMin - 1;
                this.chart.ChartAreas[0].AxisY.Interval = cMax - cMin <= 10 ? 1 : cMax - cMin <= 20 ? 2 : cMax - cMin <= 30 ? 3 : cMax - cMin <= 40 ? 4 : cMax - cMin <= 50 ? 5 : 10;

                this.chart.Series.Add(TrNAVPointsSeriesName);
                this.chart.Series[TrNAVPointsSeriesName].XValueType = ChartValueType.Date;
                this.chart.Series[TrNAVPointsSeriesName].LegendText = TrNAVPointsSeriesName;
                this.chart.Series[TrNAVPointsSeriesName].ChartType = SeriesChartType.FastLine;
                this.chart.Series[TrNAVPointsSeriesName].Points.DataBindXY(trdayValues, trnavPoints);
                this.chart.Series[TrNAVPointsSeriesName].Color = NAVColor;

                this.chart.Series.Add(TrInstrPointsSeriesName);
                this.chart.Series[TrInstrPointsSeriesName].XValueType = ChartValueType.Date;
                this.chart.Series[TrInstrPointsSeriesName].LegendText = TrInstrPointsSeriesName;
                this.chart.Series[TrInstrPointsSeriesName].ChartType = SeriesChartType.FastLine;
                this.chart.Series[TrInstrPointsSeriesName].Points.DataBindXY(trdayValues, trinstrPoints);
                this.chart.Series[TrInstrPointsSeriesName].Color = InstrColor;

                this.chart.Series.Add(TeNAVPointsSeriesName);
                this.chart.Series[TeNAVPointsSeriesName].XValueType = ChartValueType.Date;
                this.chart.Series[TeNAVPointsSeriesName].LegendText = TeNAVPointsSeriesName;
                this.chart.Series[TeNAVPointsSeriesName].ChartType = SeriesChartType.FastLine;
                this.chart.Series[TeNAVPointsSeriesName].Points.DataBindXY(tedayValues, tenavPoints);
                this.chart.Series[TeNAVPointsSeriesName].Color = NAVColor;

                this.chart.Series.Add(TeInstrPointsSeriesName);
                this.chart.Series[TeInstrPointsSeriesName].XValueType = ChartValueType.Date;
                this.chart.Series[TeInstrPointsSeriesName].LegendText = TeInstrPointsSeriesName;
                this.chart.Series[TeInstrPointsSeriesName].ChartType = SeriesChartType.FastLine;
                this.chart.Series[TeInstrPointsSeriesName].Points.DataBindXY(tedayValues, teinstrPoints);
                this.chart.Series[TeInstrPointsSeriesName].Color = InstrColor;

                List<DateTime> zeroDayPoints = new List<DateTime>();
                zeroDayPoints.AddRange(trdayValues);
                zeroDayPoints.AddRange(tedayValues);
                this.chart.Series.Add(ZeroLineSeriesName);
                this.chart.Series[TeInstrPointsSeriesName].XValueType = ChartValueType.Date;
                this.chart.Series[ZeroLineSeriesName].ChartType = SeriesChartType.FastLine;
                this.chart.Series[ZeroLineSeriesName].Color = Color.Gray;
                this.chart.Series[ZeroLineSeriesName].IsVisibleInLegend = false;
                this.chart.Series[ZeroLineSeriesName].Points.DataBindXY(zeroDayPoints.ToArray(), Enumerable.Repeat(100, zeroDayPoints.Count).ToArray());
                this.chart.Series[ZeroLineSeriesName].Label = "100";


                StripLine trstrip = new StripLine();
                trstrip.BackColor = TrainingBgColor;
                trstrip.Text = "Training";
                trstrip.Interval = 0;
                trstrip.IntervalOffsetType = DateTimeIntervalType.Days;
                trstrip.IntervalOffset = trdayValues.First().ToOADate();
                trstrip.StripWidthType = DateTimeIntervalType.Days;
                trstrip.StripWidth = trdayValues.Last().ToOADate() - trdayValues.First().ToOADate();
                this.chart.ChartAreas[0].AxisX.StripLines.Add(trstrip);


                StripLine testrip = new StripLine();
                testrip.BackColor = TestBgColor;
                testrip.Text = "Test";
                testrip.Interval = 0;
                testrip.IntervalOffsetType = DateTimeIntervalType.Days;
                testrip.IntervalOffset = tedayValues.First().ToOADate();
                testrip.IntervalType = DateTimeIntervalType.Days;
                testrip.StripWidthType = DateTimeIntervalType.Days;
                testrip.StripWidth = tedayValues.Last().ToOADate() - tedayValues.First().ToOADate();
                this.chart.ChartAreas[0].AxisX.StripLines.Add(testrip);


                //UpdateCursorInterval();
                //this.UpdateStripLines();
            }
        }

        private void UpdateCursorInterval()
        {
            var navValues = this.chart.Series[TrNAVPointsSeriesName].Points.Select(x => x.YValues[0]).DefaultIfEmpty(1.0);
            double navValuesRange = navValues.Max() - navValues.Min();
            var instrValues = this.chart.Series[TrInstrPointsSeriesName].Points.Select(x => x.YValues[0]).DefaultIfEmpty(1.0);
            double instrValuesRange = instrValues.Max() - instrValues.Min();

            double interestingValuesRange = Math.Min(Math.Max(navValuesRange, 1.0), Math.Max(instrValuesRange, 1.0));
            double digits = (int)Math.Log10(interestingValuesRange) - 3;
            double yZoomInterval = Math.Max(Math.Pow(10, digits), 10E-5);
            this.chart.ChartAreas[0].CursorY.Interval = yZoomInterval;
        }

        private void UpdateStripLines()
        {

        }

        private void CreateAndAddStripLine()
        {

        }

        private void ToggleSeriesData(string seriesName)
        {

            if (this.chart.Series[seriesName].Enabled)
                this.chart.Series[seriesName].Enabled = false;
            else
                this.chart.Series[seriesName].Enabled = true;

            /*
            if (series.Points.Count > 0)
            {
                if (this.chart.Series.Any(s => s != series && s.Points.Count > 0))
                {
                    ClearPointsQuick(series.Points);
                }
            }
            else if (Content != null)
            {
                DateTime[] dayPoints = Content.Model.GetDayPoints(Content.ProblemData, Content.ProblemData.TrainingIndices);
                double[] values = null;

                if(series.Name.Equals(TrNAVPointsSeriesName))
                    values = Content.Model.GetDailyNavPoints(Content.ProblemData, Content.ProblemData.TrainingIndices);
                else if (series.Name.Equals(TrInstrPointsSeriesName))
                    values = Content.Model.GetDailyInstrPoints(Content.ProblemData, Content.ProblemData.TrainingIndices);

                double firstnav = values[0];
                values = values.Select(x => Math.Round((x / firstnav) * 100, 2)).ToArray();

                if (values.Length > 0)
                {
                    series.Points.DataBindXY(dayPoints, values);
                    this.InsertEmptyPoints(series);
                }
                chart.Legends[series.Legend].ForeColor = Color.Black;
                UpdateCursorInterval();
                chart.Refresh();
            }
            */
        }

        private void InsertEmptyPoints(Series series)
        {
            int i = 0;
            while (i < series.Points.Count - 1)
            {
                if (series.Points[i].IsEmpty)
                {
                    ++i;
                    continue;
                }

                var p1 = series.Points[i];
                var p2 = series.Points[i + 1];
                // check for consecutive indices
                if ((int)p2.XValue - (int)p1.XValue != 1)
                {
                    // insert an empty point between p1 and p2 so that the line will be invisible (transparent)
                    var p = new DataPoint((int)((p1.XValue + p2.XValue) / 2), 0.0) { IsEmpty = true };
                    series.Points.Insert(i + 1, p);
                }
                ++i;
            }
        }

        // workaround as per http://stackoverflow.com/questions/5744930/datapointcollection-clear-performance
        private static void ClearPointsQuick(DataPointCollection points)
        {
            points.SuspendUpdates();
            while (points.Count > 0)
                points.RemoveAt(points.Count - 1);
            points.ResumeUpdates();
        }

        private void MultiMin(double[] a1, double[] a2)
        {

        }

        #endregion

        #region Events

        protected override void RegisterContentEvents()
        {
            base.RegisterContentEvents();
            Content.ModelChanged += new EventHandler(Content_ModelChanged);
            Content.ProblemDataChanged += new EventHandler(Content_ProblemDataChanged);
        }
        protected override void DeregisterContentEvents()
        {
            base.DeregisterContentEvents();
            Content.ModelChanged -= new EventHandler(Content_ModelChanged);
            Content.ProblemDataChanged -= new EventHandler(Content_ProblemDataChanged);
        }

        protected override void OnContentChanged()
        {
            base.OnContentChanged();
            RedrawChart();
        }
        private void Content_ProblemDataChanged(object sender, EventArgs e)
        {
            RedrawChart();
        }
        private void Content_ModelChanged(object sender, EventArgs e)
        {
            RedrawChart();
        }


        #endregion

        #region Event Handlers
        private void chart_CustomizeLegend(object sender, CustomizeLegendEventArgs e)
        {
            if (chart.Series.Count != 2) return;
            e.LegendItems[0].Cells[1].ForeColor = this.chart.Series[TrNAVPointsSeriesName].Points.Count == 0 ? Color.Gray : Color.Black;
            e.LegendItems[1].Cells[1].ForeColor = this.chart.Series[TrInstrPointsSeriesName].Points.Count == 0 ? Color.Gray : Color.Black;
        }

        private void chart_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            /*
            HitTestResult result = chart.HitTest(e.X, e.Y);
            if (result.ChartArea != null && (result.ChartElementType == ChartElementType.PlottingArea ||
                                             result.ChartElementType == ChartElementType.Gridlines) ||
                                             result.ChartElementType == ChartElementType.StripLines)
            {
                foreach (var axis in result.ChartArea.Axes)
                    axis.ScaleView.ZoomReset(int.MaxValue);
            }
            */
        }

        private void chart_MouseDown(object sender, MouseEventArgs e)
        {
            HitTestResult result = chart.HitTest(e.X, e.Y);
            if (result.ChartElementType == ChartElementType.LegendItem)
            {
                ToggleSeriesData(result.Series.Name);
            }
        }

        private void chart_MouseMove(object sender, MouseEventArgs e)
        {
            HitTestResult result = chart.HitTest(e.X, e.Y);
            if (result.ChartElementType == ChartElementType.LegendItem && !result.Series.Name.Equals(TrNAVPointsSeriesName))
                Cursor = Cursors.Hand;
            else
                Cursor = Cursors.Default;
        }
        #endregion
    }
}
