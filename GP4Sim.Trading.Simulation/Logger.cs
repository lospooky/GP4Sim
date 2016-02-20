using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GP4Sim.Trading.Simulation
{
    public class Logger
    {
        StringBuilder sb;

        public Logger()
        {
            sb = new StringBuilder();
        }

        public void LogLine(object sender, LogLineEventArgs e)
        {
            sb.AppendLine(e.Line);
        }

        public string GetLog
        {
            get
            {
                if (sb == null || sb.Length == 0)
                    return String.Empty;
                else
                    return sb.ToString();
            }
        }

        public static string NewDataPoint(long dpIndex, DateTime tp, double nav, double price)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("DataPoint #: " + dpIndex);
            sb.AppendLine("TimeStamp: " + tp.ToString());
            sb.Append("Current Price: " + price.ToString("F4"));

            return sb.ToString();
        }

        public static string NAV(double av, double pv, double nav)
        {
            return "NAV: " + av.ToString("F0") + " + " + pv.ToString("F0") + " = " + nav.ToString("F0");
        }

        public static string Inputs(double[] iv)
        {
            string s = "Inputs: ";
            foreach (double v in iv)
            {
                s += v.ToString("F4");
                s += " ";
            }

            return s;
        }

        public static string Action(string aName, long lotSize, double price, double commissions, double avc)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(aName + " " + lotSize + " @" + price);
            sb.Append("Commissions: " + commissions.ToString("F4"));

            return sb.ToString();
        }

        public static string Output(double v)
        {
            return "Output: " + v.ToString("F4");
        }

        public static string TradeSignal(double v)
        {
            return "Trade Signal: " + v.ToString("F4");
        }

        public static string DpSeparator
        {
            get { return "------------------------------------------------"; }
        }

        public static string ActionSeparator
        {
            get { return "-------------------------"; }
        }

        public void Teardown()
        {
            if (sb != null)
                sb.Clear();
            sb = null;
        }
    }
}
