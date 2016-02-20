using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using GP4Sim.Trading.Solutions;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using HeuristicLab.Problems.DataAnalysis.Symbolic;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace GP4Sim.Excel
{
    internal static class TradingReportFactory
    {
        private static Color HeaderColor = Color.FromArgb(184, 204, 228);
        private static Color CatColor = Color.FromArgb(218, 238, 243);
        private static Color LightRowShading = Color.FromArgb(242, 242, 242);
        private static Color DarkRowShading = Color.FromArgb(217, 217, 217);
        private static Color SetColor = Color.FromArgb(253, 233, 217);
        private static int nCols = 3;

        public static void Header(ExcelWorksheet ws)
        {
            ws.DefaultColWidth = 30;

            for (int i = 1; i <= nCols; i++)
            {
                ws.Cells[1, i].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Double;
                ws.Cells[1, i].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                ws.Cells[1, i].Style.Fill.BackgroundColor.SetColor(HeaderColor);
                ws.Cells[1, i].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
            }

            for (int i = 2; i <= nCols; i++)
            {
                ws.Column(i).Width = 20;
                ws.Column(i).Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

            }

            ws.Row(1).Style.Font.Bold = true;
            ws.Row(1).Height = 25;
            ws.Row(1).Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            ws.Row(1).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

            ws.Cells[1, 1].Value = "Attribute";
            ws.Cells[1, 2].Value = "Training Set";
            ws.Cells[1, 3].Value = "Test Set";
        }

        public static void Body(ExcelWorksheet ws, TradingSolution sol)
        {
            int idx = 2;
            ResultCollection trainRc = sol.TrainingResultCollection;
            ResultCollection testRc = sol.TestResultCollection;
            TradingModel model = (TradingModel)sol.Model;
            foreach (string cat in model.ResultProperties.Categories)
            {
                if (!cat.Equals("Daily"))
                {
                    ws.Row(idx).Style.Font.Bold = true;
                    ws.Row(idx).Height = 25;
                    ws.Row(idx).Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    ws.Row(idx).Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    ws.Cells[idx, 1].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    ws.Cells[idx, 2].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    ws.Cells[idx, 3].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    ws.Cells[idx, 1].Style.Fill.BackgroundColor.SetColor(CatColor);
                    ws.Cells[idx, 2].Style.Fill.BackgroundColor.SetColor(CatColor);
                    ws.Cells[idx, 3].Style.Fill.BackgroundColor.SetColor(CatColor);
                    ws.Cells[idx++, 1].Value = cat;

                    foreach (string val in model.ResultProperties.CategoryValues(cat))
                    {
                        ws.Cells[idx, 1].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        ws.Cells[idx, 2].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        ws.Cells[idx, 3].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        if ((idx % 2) == 0)
                        {
                            ws.Cells[idx, 1].Style.Fill.BackgroundColor.SetColor(LightRowShading);
                            ws.Cells[idx, 2].Style.Fill.BackgroundColor.SetColor(LightRowShading);
                            ws.Cells[idx, 3].Style.Fill.BackgroundColor.SetColor(LightRowShading);
                        }
                        else
                        {
                            ws.Cells[idx, 1].Style.Fill.BackgroundColor.SetColor(DarkRowShading);
                            ws.Cells[idx, 2].Style.Fill.BackgroundColor.SetColor(DarkRowShading);
                            ws.Cells[idx, 3].Style.Fill.BackgroundColor.SetColor(DarkRowShading);
                        }

                        string vName = model.ResultProperties.VisibleName(val);
                        ws.Cells[idx, 1].Value = vName;
                        if (vName.Contains('%'))
                            ws.Cells[idx, 2].Style.Numberformat.Format = "#0\\.00%";
                        if (vName.Contains("NAV") || vName.Contains("PnL"))
                            ws.Cells[idx, 2].Style.Numberformat.Format = "#,##0";

                        Type t = model.ResultProperties.Type(val);
                        if (!trainRc.ContainsKey(cat))
                            continue;
                        ResultCollection catrc = (ResultCollection)trainRc[cat].Value;


                        IItem v;
                        if (catrc.ContainsKey(model.ResultProperties.VisibleName(val)))
                            v = catrc[model.ResultProperties.VisibleName(val)].Value;
                        else
                            v = new StringValue("Not Available");
      

                        if (v is DoubleValue)
                        {
                            double n = ((DoubleValue)v).Value;
                            if (double.IsNaN(n))
                                ws.Cells[idx, 2].Value = "NaN";
                            else if (double.IsInfinity(n))
                                ws.Cells[idx, 2].Value = "Inf";
                            else if (n == double.MaxValue || n == double.MinValue)
                                ws.Cells[idx, 2].Value = "---";
                            else if (n == 0)
                                ws.Cells[idx, 2].Value = 0;
                            else
                            {
                                ws.Cells[idx, 2].Value = Math.Round(n, 4);
                            }
                        }
                        else if (v is IntValue)
                            ws.Cells[idx, 2].Value = ((IntValue)v).Value;
                        else if (v is StringValue)
                            ws.Cells[idx, 2].Value = ((StringValue)v).Value;


                        if (vName.Contains('%'))
                            ws.Cells[idx, 3].Style.Numberformat.Format = "#0\\.00%";
                        if (vName.Contains("NAV") || vName.Contains("PnL"))
                            ws.Cells[idx, 3].Style.Numberformat.Format = "#,##0";

                        if (!testRc.ContainsKey(cat))
                            continue;
                        catrc = (ResultCollection)testRc[cat].Value;

                        if (catrc.ContainsKey(model.ResultProperties.VisibleName(val)))
                            v = catrc[model.ResultProperties.VisibleName(val)].Value;
                        else
                            v = new StringValue("Not Available");


                        if (v is DoubleValue)
                        {
                            double n = ((DoubleValue)v).Value;
                            if (double.IsNaN(n))
                                ws.Cells[idx++, 3].Value = "NaN";
                            else if (double.IsInfinity(n))
                                ws.Cells[idx++, 3].Value = "Inf";
                            else if (n == double.MaxValue || n == double.MinValue)
                                ws.Cells[idx++, 3].Value = "---";
                            else if (n == 0)
                                ws.Cells[idx++, 3].Value = 0;
                            else
                            {
                                ws.Cells[idx++, 3].Value = Math.Round(n, 4);

                            }

                        }
                        else if (v is IntValue)
                            ws.Cells[idx++, 3].Value = ((IntValue)v).Value;
                        else if (v is StringValue)
                            ws.Cells[idx++, 3].Value = ((StringValue)v).Value;

                    }
                }
            }
            idx--;
            ws.Cells[idx, 1].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
            ws.Cells[idx, 2].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
            ws.Cells[idx, 3].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
            ws.Cells[1, 1, idx, nCols].Style.Border.Right.Style = ExcelBorderStyle.Thin;

        }

        public static void BatchReport(ExcelWorksheet ws, List<TradingSolution> solutions)
        {
                bool isFirst = true;
                int nAgents = 0;
                int nMaxRows = 0;
                List<int> catRows = new List<int>();
                int testsetrow = 0;


                ws.DefaultColWidth = 30;
                ws.Row(1).Style.Font.Bold = true;
                ws.Row(1).Height = 25;
                ws.Row(1).Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                ws.Row(1).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                ws.Cells[1, 1].Value = "Attribute";

                foreach (TradingSolution sol in solutions)
                {
                    nAgents++;
                    int idx = 2;

                    ResultCollection trainRc = sol.TrainingResultCollection;
                    ResultCollection testRc = sol.TestResultCollection;
                    TradingModel model = (TradingModel)sol.Model;

                    //Layout Attribute Rows, 2x
                    if (isFirst)
                    {
                        catRows.Add(idx);
                        ws.Cells[idx++, 1].Value = "Training Set";
                        foreach (string cat in model.ResultProperties.Categories)
                        {
                            if (!cat.Equals("Daily"))
                            {
                                catRows.Add(idx);
                                ws.Cells[idx++, 1].Value = cat;

                                foreach (string val in model.ResultProperties.CategoryValues(cat))
                                {
                                    ws.Cells[idx++, 1].Value = model.ResultProperties.VisibleName(val);
                                }
                            }
                        }
                        catRows.Add(idx);
                        testsetrow = idx;
                        ws.Cells[idx++, 1].Value = "Test Set";
                        foreach (string cat in model.ResultProperties.Categories)
                        {
                            if (!cat.Equals("Daily"))
                            {
                                catRows.Add(idx);
                                ws.Cells[idx++, 1].Value = cat;

                                foreach (string val in model.ResultProperties.CategoryValues(cat))
                                {
                                    ws.Cells[idx++, 1].Value = model.ResultProperties.VisibleName(val);
                                }
                            }
                        }
                        nMaxRows = idx - 1;
                        idx = 2;
                    }

                    ws.Cells[1, nAgents + 1].Value = "Agent " + nAgents;

                    //Training Set
                    idx++;
                    foreach (string cat in model.ResultProperties.Categories)
                    {
                        if (!cat.Equals("Daily"))
                        {
                            idx++;

                            foreach (string val in model.ResultProperties.CategoryValues(cat))
                            {
                                string vName = model.ResultProperties.VisibleName(val);
                                if (vName.Contains('%'))
                                    ws.Cells[idx, nAgents + 1].Style.Numberformat.Format = "#0\\.00%";
                                if (vName.Contains("NAV") || vName.Contains("PnL"))
                                    ws.Cells[idx, nAgents + 1].Style.Numberformat.Format = "#,##0";


                                Type t = model.ResultProperties.Type(val);
                                if (!trainRc.ContainsKey(cat))
                                {
                                    ws.Cells[idx++, nAgents + 1].Value = "Not Available";
                                    continue;
                                }
                                ResultCollection catrc = (ResultCollection)trainRc[cat].Value;

                                IItem v;
                                if (catrc.ContainsKey(model.ResultProperties.VisibleName(val)))
                                    v = catrc[model.ResultProperties.VisibleName(val)].Value;
                                else
                                    v = new StringValue("Not Available");

                                if (v is DoubleValue)
                                {
                                    double n = ((DoubleValue)v).Value;
                                    if (double.IsNaN(n))
                                        ws.Cells[idx++, nAgents + 1].Value = "NaN";
                                    else if (double.IsInfinity(n))
                                        ws.Cells[idx++, nAgents + 1].Value = "Inf";
                                    else if (n == double.MaxValue || n == double.MinValue)
                                        ws.Cells[idx++, nAgents + 1].Value = "---";
                                    else if (n == 0)
                                        ws.Cells[idx++, nAgents + 1].Value = 0;
                                    else
                                    {
                                        ws.Cells[idx++, nAgents + 1].Value = Math.Round(n, 4);
                                    }
                                }
                                else if (v is IntValue)
                                    ws.Cells[idx++, nAgents + 1].Value = ((IntValue)v).Value;
                                else if (v is StringValue)
                                    ws.Cells[idx++, nAgents + 1].Value = ((StringValue)v).Value;
                            }
                        }
                    }

                    idx++;
                    //Validation Set
                    foreach (string cat in model.ResultProperties.Categories)
                    {
                        if (!cat.Equals("Daily"))
                        {
                            idx++;

                            foreach (string val in model.ResultProperties.CategoryValues(cat))
                            {
                                string vName = model.ResultProperties.VisibleName(val);
                                if (vName.Contains('%'))
                                    ws.Cells[idx, nAgents + 1].Style.Numberformat.Format = "#0\\.00%";
                                if (vName.Contains("NAV") || vName.Contains("PnL"))
                                    ws.Cells[idx, nAgents + 1].Style.Numberformat.Format = "#,##0";

                                Type t = model.ResultProperties.Type(val);
                                if (!testRc.ContainsKey(cat))
                                {
                                    ws.Cells[idx++, nAgents + 1].Value = "Not Available";
                                    continue;
                                }
                                ResultCollection catrc = (ResultCollection)testRc[cat].Value;

                                IItem v;
                                if (catrc.ContainsKey(model.ResultProperties.VisibleName(val)))
                                    v = catrc[model.ResultProperties.VisibleName(val)].Value;
                                else
                                    v = new StringValue("Not Available");

                                if (v is DoubleValue)
                                {
                                    double n = ((DoubleValue)v).Value;
                                    if (double.IsNaN(n))
                                        ws.Cells[idx++, nAgents + 1].Value = "NaN";
                                    else if (double.IsInfinity(n))
                                        ws.Cells[idx++, nAgents + 1].Value = "Inf";
                                    else if (n == double.MaxValue || n == double.MinValue)
                                        ws.Cells[idx++, nAgents + 1].Value = "---";
                                    else if (n == 0)
                                        ws.Cells[idx++, nAgents + 1].Value = 0;
                                    else
                                    {
                                        ws.Cells[idx++, nAgents + 1].Value = Math.Round(n, 4);
                                        if (vName.Contains('%'))
                                            ws.Cells[idx, nAgents + 1].Style.Numberformat.Format = "#0\\.00%";
                                    }
                                }
                                else if (v is IntValue)
                                    ws.Cells[idx++, nAgents + 1].Value = ((IntValue)v).Value;
                                else if (v is StringValue)
                                    ws.Cells[idx++, nAgents + 1].Value = ((StringValue)v).Value;
                            }


                            if (isFirst)
                                isFirst = false;
                        }
                    }

                }


                ws.Cells[1, 1, 1, nAgents + 1].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                ws.Cells[1, 1, 1, nAgents + 1].Style.Fill.BackgroundColor.SetColor(HeaderColor);
                foreach (int i in catRows)
                {
                    ws.Row(i).Style.Font.Bold = true;
                    ws.Row(i).Height = 25;
                    ws.Row(i).Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    ws.Cells[i, 1, i, nAgents + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    ws.Cells[i, 1, i, nAgents + 1].Style.Fill.BackgroundColor.SetColor(CatColor);
                }
                ws.Cells[1, 1, 1, nAgents + 1].Style.Border.Bottom.Style = ExcelBorderStyle.Double;
                ws.Cells[2, 1, 2, nAgents + 1].Style.Border.Bottom.Style = ExcelBorderStyle.Double;
                ws.Cells[testsetrow, 1, testsetrow, nAgents + 1].Style.Border.Top.Style = ExcelBorderStyle.Double;
                ws.Cells[2, 1, 2, nAgents + 1].Style.Fill.BackgroundColor.SetColor(SetColor);
                ws.Cells[testsetrow, 1, testsetrow, nAgents + 1].Style.Fill.BackgroundColor.SetColor(SetColor);
                ws.Cells[testsetrow, 1, testsetrow, nAgents + 1].Style.Border.Bottom.Style = ExcelBorderStyle.Double;
                ws.Cells[1, nAgents + 1, nMaxRows, nAgents + 1].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                ws.Cells[nMaxRows, 1, nMaxRows, nAgents + 1].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                ws.Cells[2, 2, nMaxRows, nAgents + 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                for (int i = 2; i <= nAgents + 1; i++)
                    ws.Column(i).Width = 20;

                for (int i = 4; i <= nMaxRows; i++)
                {
                    ws.Cells[i, 1, i, nAgents + 1].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    if (!catRows.Contains(i))
                    {
                        if (i % 2 == 0)
                            ws.Cells[i, 1, i, nAgents + 1].Style.Fill.BackgroundColor.SetColor(LightRowShading);
                        else
                            ws.Cells[i, 1, i, nAgents + 1].Style.Fill.BackgroundColor.SetColor(DarkRowShading);
                    }
                }
        }

        public static void ShortBatchReport(ExcelWorksheet ws, List<TradingSolution> solutions)
        {
            bool isFirst = true;
            int nAgents = 0;
            int nMaxRows = 0;
            List<int> catRows = new List<int>();
            int testsetrow = 0;


            ws.DefaultColWidth = 30;
            ws.Row(1).Style.Font.Bold = true;
            ws.Row(1).Height = 25;
            ws.Row(1).Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            ws.Row(1).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            ws.Cells[1, 1].Value = "Attribute";

            foreach (TradingSolution sol in solutions)
            {
                nAgents++;
                int idx = 2;

                ResultCollection trainRc = sol.TrainingResultCollection;
                ResultCollection testRc = sol.TestResultCollection;
                TradingModel model = (TradingModel)sol.Model;

                if (isFirst)
                {
                    catRows.Add(idx);

                    ws.Cells[idx++, 1].Value = "Training Set";
                    foreach (string cat in model.ResultProperties.Categories)
                    {
                        foreach (string val in model.ResultProperties.CategoryValues(cat))
                        {
                            string vn = model.ResultProperties.VisibleName(val);

                            if (vn.Equals("Initial NAV") || vn.Equals("Last Day NAV") || vn.Equals("Long PnL") || vn.Equals("Short PnL") ||
                                (vn.Equals("Num Trades") && cat.Equals("Trading - Total")) || vn.Equals("Long Crossings") || vn.Equals("Short Crossings") || 
                                vn.Equals("Exposure Average %") || vn.Equals("BuyHold Return %") || vn.Equals("Fitness Score "))
                            {
                                ws.Cells[idx++, 1].Value = vn;
                            }
                        }
                    }
                    catRows.Add(idx);
                    testsetrow = idx;
                    ws.Cells[idx++, 1].Value = "Test Set";
                    foreach (string cat in model.ResultProperties.Categories)
                    {

                        foreach (string val in model.ResultProperties.CategoryValues(cat))
                        {
                            string vn = model.ResultProperties.VisibleName(val);

                            if (vn.Equals("Initial NAV") || vn.Equals("Last Day NAV") || vn.Equals("Long PnL") || vn.Equals("Short PnL") ||
                                (vn.Equals("Num Trades") && cat.Equals("Trading - Total")) || vn.Equals("Long Crossings") || vn.Equals("Short Crossings") ||
                                vn.Equals("Exposure Average %") || vn.Equals("BuyHold Return %") || vn.Equals("Fitness Score "))
                            {
                                ws.Cells[idx++, 1].Value = vn;
                            }
                        }
                    }
                    nMaxRows = idx - 1;
                    idx = 2;
                }

                
                ws.Cells[1, nAgents + 1].Value = "Agent " + nAgents;

                //Training Set
                idx++;
                foreach (string cat in model.ResultProperties.Categories)
                {
                    foreach (string val in model.ResultProperties.CategoryValues(cat))
                    {
                        string vn = model.ResultProperties.VisibleName(val);

                        if (vn.Equals("Initial NAV") || vn.Equals("Last Day NAV") || vn.Equals("Long PnL") || vn.Equals("Short PnL") ||
                            (vn.Equals("Num Trades") && cat.Equals("Trading - Total")) || vn.Equals("Long Crossings") || vn.Equals("Short Crossings") ||
                            vn.Equals("Exposure Average %") || vn.Equals("BuyHold Return %") || vn.Equals("Fitness Score "))
                        {
                            if (vn.Contains('%'))
                                ws.Cells[idx, nAgents + 1].Style.Numberformat.Format = "#0\\.00%";
                            if (vn.Contains("NAV") || vn.Contains("PnL"))
                                ws.Cells[idx, nAgents + 1].Style.Numberformat.Format = "#,##0";

                            Type t = model.ResultProperties.Type(val);
                            if (!trainRc.ContainsKey(cat))
                            {
                                ws.Cells[idx++, nAgents + 1].Value = "Not Available";
                                continue;
                            }
                            ResultCollection catrc = (ResultCollection)trainRc[cat].Value;

                            IItem v;
                            if (catrc.ContainsKey(model.ResultProperties.VisibleName(val)))
                                v = catrc[model.ResultProperties.VisibleName(val)].Value;
                            else
                                v = new StringValue("Not Available");

                            if (v is DoubleValue)
                            {
                                double n = ((DoubleValue)v).Value;
                                if (double.IsNaN(n))
                                    ws.Cells[idx++, nAgents + 1].Value = "NaN";
                                else if (double.IsInfinity(n))
                                    ws.Cells[idx++, nAgents + 1].Value = "Inf";
                                else if (n == double.MaxValue || n == double.MinValue)
                                    ws.Cells[idx++, nAgents + 1].Value = "---";
                                else if (n == 0)
                                    ws.Cells[idx++, nAgents + 1].Value = 0;
                                else
                                {
                                    ws.Cells[idx++, nAgents + 1].Value = Math.Round(n, 4);
                                }
                            }
                            else if (v is IntValue)
                                ws.Cells[idx++, nAgents + 1].Value = ((IntValue)v).Value;
                            else if (v is StringValue)
                                ws.Cells[idx++, nAgents + 1].Value = ((StringValue)v).Value;
                        }
                    }
                }
                idx++;
                //Test Set
                foreach (string cat in model.ResultProperties.Categories)
                {
                    foreach (string val in model.ResultProperties.CategoryValues(cat))
                    {
                        string vn = model.ResultProperties.VisibleName(val);

                        if (vn.Equals("Initial NAV") || vn.Equals("Last Day NAV") || vn.Equals("Long PnL") || vn.Equals("Short PnL") ||
                            (vn.Equals("Num Trades") && cat.Equals("Trading - Total")) || vn.Equals("Long Crossings") || vn.Equals("Short Crossings") ||
                            vn.Equals("Exposure Average %") || vn.Equals("BuyHold Return %") || vn.Equals("Fitness Score "))
                        {
                            if (vn.Contains('%'))
                                ws.Cells[idx, nAgents + 1].Style.Numberformat.Format = "#0\\.00%";
                            if (vn.Contains("NAV") || vn.Contains("PnL"))
                                ws.Cells[idx, nAgents + 1].Style.Numberformat.Format = "#,##0";

                            Type t = model.ResultProperties.Type(val);
                            if (!testRc.ContainsKey(cat))
                            {
                                ws.Cells[idx++, nAgents + 1].Value = "Not Available";
                                continue;
                            }
                            ResultCollection catrc = (ResultCollection)testRc[cat].Value;

                            IItem v;
                            if (catrc.ContainsKey(model.ResultProperties.VisibleName(val)))
                                v = catrc[model.ResultProperties.VisibleName(val)].Value;
                            else
                                v = new StringValue("Not Available");

                            if (v is DoubleValue)
                            {
                                double n = ((DoubleValue)v).Value;
                                if (double.IsNaN(n))
                                    ws.Cells[idx++, nAgents + 1].Value = "NaN";
                                else if (double.IsInfinity(n))
                                    ws.Cells[idx++, nAgents + 1].Value = "Inf";
                                else if (n == double.MaxValue || n == double.MinValue)
                                    ws.Cells[idx++, nAgents + 1].Value = "---";
                                else if (n == 0)
                                    ws.Cells[idx++, nAgents + 1].Value = 0;
                                else
                                {
                                    ws.Cells[idx++, nAgents + 1].Value = Math.Round(n, 4);
                                }
                            }
                            else if (v is IntValue)
                                ws.Cells[idx++, nAgents + 1].Value = ((IntValue)v).Value;
                            else if (v is StringValue)
                                ws.Cells[idx++, nAgents + 1].Value = ((StringValue)v).Value;
                        }
                    }
                }
                if (isFirst)
                    isFirst = false;
            }
            ws.Cells[1, 1, 1, nAgents + 1].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            ws.Cells[1, 1, 1, nAgents + 1].Style.Fill.BackgroundColor.SetColor(HeaderColor);
            foreach (int i in catRows)
            {
                ws.Row(i).Style.Font.Bold = true;
                ws.Row(i).Height = 25;
                ws.Row(i).Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                ws.Cells[i, 1, i, nAgents + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                ws.Cells[i, 1, i, nAgents + 1].Style.Fill.BackgroundColor.SetColor(CatColor);
            }
            ws.Cells[1, 1, 1, nAgents + 1].Style.Border.Bottom.Style = ExcelBorderStyle.Double;
            ws.Cells[2, 1, 2, nAgents + 1].Style.Border.Bottom.Style = ExcelBorderStyle.Double;
            ws.Cells[testsetrow, 1, testsetrow, nAgents + 1].Style.Border.Top.Style = ExcelBorderStyle.Double;
            ws.Cells[2, 1, 2, nAgents + 1].Style.Fill.BackgroundColor.SetColor(SetColor);
            ws.Cells[testsetrow, 1, testsetrow, nAgents + 1].Style.Fill.BackgroundColor.SetColor(SetColor);
            ws.Cells[testsetrow, 1, testsetrow, nAgents + 1].Style.Border.Bottom.Style = ExcelBorderStyle.Double;
            ws.Cells[1, nAgents + 1, nMaxRows, nAgents + 1].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            ws.Cells[nMaxRows, 1, nMaxRows, nAgents + 1].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

            ws.Cells[2, 2, nMaxRows, nAgents + 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

            for (int i = 2; i <= nAgents + 1; i++)
                ws.Column(i).Width = 20;

            for (int i = 3; i <= nMaxRows; i++)
            {
                ws.Cells[i, 1, i, nAgents + 1].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                if (!catRows.Contains(i))
                {
                    if (i % 2 == 0)
                        ws.Cells[i, 1, i, nAgents + 1].Style.Fill.BackgroundColor.SetColor(LightRowShading);
                    else
                        ws.Cells[i, 1, i, nAgents + 1].Style.Fill.BackgroundColor.SetColor(DarkRowShading);
                }
            
            }

        }
    
    }
}
