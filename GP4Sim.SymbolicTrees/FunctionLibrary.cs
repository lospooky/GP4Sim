using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using HeuristicLab.Common;

namespace GP4Sim.SymbolicTrees
{
    public static class FunctionLibrary
    {
        public static readonly Type Functions = typeof(FunctionLibrary);

        #region MethodInfos

        public static MethodInfo AiryA = Functions.GetMethod("AiryA_f");
        public static MethodInfo AiryB = Functions.GetMethod("AiryB_f");
        public static MethodInfo Bessel = Functions.GetMethod("Bessel_f");
        public static MethodInfo CosIntegral = Functions.GetMethod("CosIntegral_f");
        public static MethodInfo Dawson = Functions.GetMethod("Dawson_f");
        public static MethodInfo Erf = Functions.GetMethod("Erf_f");
        public static MethodInfo ExpIntegralEi = Functions.GetMethod("ExpIntegralEi_f");
        public static MethodInfo FresnelCosIntegral = Functions.GetMethod("FresnelCosIntegral_f");
        public static MethodInfo FresnelSinIntegral = Functions.GetMethod("FresnelSinIntegral_f");
        public static MethodInfo Gamma = Functions.GetMethod("Gamma_f");
        public static MethodInfo HypCosIntegral = Functions.GetMethod("HypCosIntegral_f");
        public static MethodInfo HypSinIntegral = Functions.GetMethod("HypSinIntegral_f");
        public static MethodInfo Norm = Functions.GetMethod("Norm_f");
        public static MethodInfo Psi = Functions.GetMethod("Psi_f");
        public static MethodInfo SinIntegral = Functions.GetMethod("SinIntegral_f");


        #endregion

        #region Function Methods
        public static double AiryA_f(double x)
        {
            if (double.IsNaN(x)) return double.NaN;
            double ai, aip, bi, bip;
            alglib.airy(x, out ai, out aip, out bi, out bip);
            return ai;
        }

        public static double AiryB_f(double x)
        {
            if (double.IsNaN(x)) return double.NaN;
            double ai, aip, bi, bip;
            alglib.airy(x, out ai, out aip, out bi, out bip);
            return bi;
        }
        public static double Dawson_f(double x)
        {
            if (double.IsNaN(x)) return double.NaN;
            return alglib.dawsonintegral(x);
        }

        public static double Gamma_f(double x)
        {
            if (double.IsNaN(x)) return double.NaN;
            return alglib.gammafunction(x);
        }

        public static double Psi_f(double x)
        {
            if (double.IsNaN(x)) return double.NaN;
            else if (x <= 0 && (Math.Floor(x) - x).IsAlmost(0)) return double.NaN;
            return alglib.psi(x);
        }

        public static double ExpIntegralEi_f(double x)
        {
            if (double.IsNaN(x)) return double.NaN;
            return alglib.exponentialintegralei(x);
        }

        public static double SinIntegral_f(double x)
        {
            if (double.IsNaN(x)) return double.NaN;
            double si, ci;
            alglib.sinecosineintegrals(x, out si, out ci);
            return si;
        }

        public static double CosIntegral_f(double x)
        {
            if (double.IsNaN(x)) return double.NaN;
            double si, ci;
            alglib.sinecosineintegrals(x, out si, out ci);
            return ci;
        }

        public static double HypSinIntegral_f(double x)
        {
            if (double.IsNaN(x)) return double.NaN;
            double shi, chi;
            alglib.hyperbolicsinecosineintegrals(x, out shi, out chi);
            return shi;
        }

        public static double HypCosIntegral_f(double x)
        {
            if (double.IsNaN(x)) return double.NaN;
            double shi, chi;
            alglib.hyperbolicsinecosineintegrals(x, out shi, out chi);
            return chi;
        }

        public static double FresnelCosIntegral_f(double x)
        {
            if (double.IsNaN(x)) return double.NaN;
            double c = 0, s = 0;
            alglib.fresnelintegral(x, ref c, ref s);
            return c;
        }

        public static double FresnelSinIntegral_f(double x)
        {
            if (double.IsNaN(x)) return double.NaN;
            double c = 0, s = 0;
            alglib.fresnelintegral(x, ref c, ref s);
            return s;
        }

        public static double Norm_f(double x)
        {
            if (double.IsNaN(x)) return double.NaN;
            return alglib.normaldistribution(x);
        }

        public static double Erf_f(double x)
        {
            if (double.IsNaN(x)) return double.NaN;
            return alglib.errorfunction(x);
        }

        public static double Bessel_f(double x)
        {
            if (double.IsNaN(x)) return double.NaN;
            return alglib.besseli0(x);
        }
        #endregion
    }
}
