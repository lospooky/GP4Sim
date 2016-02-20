using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using GP4Sim.Trading.Solutions;
using HeuristicLab.Problems.DataAnalysis.Symbolic;
using OfficeOpenXml;

namespace GP4Sim.Excel
{
    public static class ExcelReportFactory
    {
        public static void CreateAndSaveReport(ISymbolicDataAnalysisSolution solution, string fullPath)
        {
            FileInfo fi = new FileInfo(fullPath.ToLower());
            if (fi.Exists)
            {
                fi.Delete();
                fi = new FileInfo(fullPath.ToLower());
            }

            using (ExcelPackage package = new ExcelPackage(fi))
            {

                ExcelWorksheet ws = package.Workbook.Worksheets.Add("Solution Stats");
                if (solution is TradingSolution)
                {
                    TradingSolution tradingSolution = (TradingSolution)solution;
                    TradingReportFactory.Header(ws);
                    TradingReportFactory.Body(ws, tradingSolution);
                }
                package.Save();
            }
        }

        public static void CreateAndSaveBatchReport(List<ISymbolicDataAnalysisSolution> solutions, string fullPath)
        {
            FileInfo fi = new FileInfo(fullPath.ToLower());
            if (fi.Exists)
            {
                fi.Delete();
                fi = new FileInfo(fullPath.ToLower());
            }

            using (ExcelPackage package = new ExcelPackage(fi))
            {
                ExcelWorksheet ws = package.Workbook.Worksheets.Add("Agents Stats");
                if(solutions.First() is TradingSolution)
                {
                    List<TradingSolution> tradingsolutions = solutions.Cast<TradingSolution>().ToList();
                    //TradingReportFactory.BatchReport(ws, tradingsolutions);
                    TradingReportFactory.ShortBatchReport(ws, tradingsolutions);
                }

                package.Save();
            }
        }
    }
}
