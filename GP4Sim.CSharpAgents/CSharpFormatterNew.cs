using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GP4Sim.SymbolicTrees;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.DataAnalysis.Symbolic;

namespace GP4Sim.CSharpAgents
{
    [Item("Deltix Agent C# Formatter 2.0", "Formats SymbolicTrees into Deltix-Runnable C# Agents, LAG SUPPORT")]
    [StorableClass]
    public sealed class CSharpFormatterNew : NamedItem, ISymbolicExpressionTreeStringFormatter
    {
        #region Constructors

        [StorableConstructor]
        private CSharpFormatterNew(bool deserializing) : base(deserializing) { }
        private CSharpFormatterNew(CSharpFormatterNew original, Cloner cloner) : base(original, cloner) { }
        public CSharpFormatterNew()
            : base()
        {
            Name = ItemName;
            Description = ItemDescription;
        }


        public override IDeepCloneable Clone(Cloner cloner)
        {
            return new CSharpFormatterNew(this, cloner);
        }

        #endregion

        #region Formatting Parser Logic
        public string Format(ISymbolicExpressionTree symbolicExpressionTree)
        {
            StringBuilder sb = new StringBuilder();

           

            return sb.ToString();
        }

        public string FormatFull(ISymbolicExpressionTree symbolicExpressionTree, List<string> actualVariables)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine(NewCSStrings.ClassHeader);

            sb.AppendLine(PrintVarMap(actualVariables));

            sb.AppendLine(NewCSStrings.FunctionHeader);
            sb.AppendLine(FormatRecursively(symbolicExpressionTree.Root, 0));
            sb.AppendLine(NewCSStrings.FunctionFooter);

            sb.AppendLine(NewCSStrings.ClassFooter);


            return sb.ToString();
        }

        private string FormatRecursively(ISymbolicExpressionTreeNode node, int linReg)
        {
            StringBuilder sb = new StringBuilder();
            ISymbol s = node.Symbol;

            if (s is ProgramRootSymbol)
                sb.Append(FormatRecursively(node.GetSubtree(0), linReg));
            else if (s is StartSymbol)
                sb.Append(FormatRecursively(node.GetSubtree(0), linReg));
            else if (s is Addition)
            {
                for (int i = 0; i < node.SubtreeCount; i++)
                {
                    if (node.GetSubtree(i).IsTerminal())
                    {
                        if (i == 0)
                            sb.AppendLine(LinearRegister(linReg) + Assignment + FormatRecursively(node.GetSubtree(i), linReg) + ";");
                        else
                            sb.AppendLine(LinearRegister(linReg) + Addition + FormatRecursively(node.GetSubtree(i), linReg) + ";");

                    }
                    else
                    {
                        sb.Append(FormatRecursively(node.GetSubtree(i), linReg + i));
                        if (i == node.SubtreeCount - 1)
                            sb.AppendLine(LinearRegister(linReg) + Addition + LinearRegister(linReg + i) + ";");
                    }
                }
            }
            else if (s is Subtraction)
            {
                for (int i = 0; i < node.SubtreeCount; i++)
                {

                    if (node.GetSubtree(i).IsTerminal())
                    {
                        if (i == 0)
                            sb.AppendLine(LinearRegister(linReg) + Assignment + FormatRecursively(node.GetSubtree(i), linReg) + ";");
                        else
                            sb.AppendLine(LinearRegister(linReg) + Subtraction + FormatRecursively(node.GetSubtree(i), linReg) + ";");

                    }
                    else
                    {
                        sb.Append(FormatRecursively(node.GetSubtree(i), linReg + i));
                        if (i == node.SubtreeCount - 1)
                            sb.AppendLine(LinearRegister(linReg) + Subtraction + LinearRegister(linReg + i) + ";");
                    }
                }

            }
            else if (s is Multiplication)
            {
                for (int i = 0; i < node.SubtreeCount; i++)
                {

                    if (node.GetSubtree(i).IsTerminal())
                    {
                        if (i == 0)
                            sb.AppendLine(LinearRegister(linReg) + Assignment + FormatRecursively(node.GetSubtree(i), linReg) + ";");
                        else
                            sb.AppendLine(LinearRegister(linReg) + Multiplication + FormatRecursively(node.GetSubtree(i), linReg) + ";");

                    }
                    else
                    {
                        sb.Append(FormatRecursively(node.GetSubtree(i), linReg + i));
                        if (i == node.SubtreeCount - 1)
                            sb.AppendLine(LinearRegister(linReg) + Multiplication + LinearRegister(linReg + i) + ";");
                    }
                }

            }
            else if (s is Division)
            {
                for (int i = 0; i < node.SubtreeCount; i++)
                {

                    if (node.GetSubtree(i).IsTerminal())
                    {
                        if (i == 0)
                            sb.AppendLine(LinearRegister(linReg) + Assignment + FormatRecursively(node.GetSubtree(i), linReg) + ";");
                        else
                            sb.AppendLine(LinearRegister(linReg) + Division + FormatRecursively(node.GetSubtree(i), linReg) + ";");

                    }
                    else
                    {
                        sb.Append(FormatRecursively(node.GetSubtree(i), linReg + i));
                        if (i == node.SubtreeCount - 1)
                            sb.AppendLine(LinearRegister(linReg) + Division + LinearRegister(linReg + i) + ";");
                    }

                }

            }
            else if (s is Logarithm)
            {
                if (node.GetSubtree(0).IsTerminal())
                    sb.AppendLine(LinearRegister(linReg) + "=" + "Math.Log(" + FormatRecursively(node.GetSubtree(0), linReg) + ");");
                else
                {
                    sb.Append(FormatRecursively(node.GetSubtree(0), linReg));
                    sb.AppendLine(LinearRegister(linReg) + "=" + "Math.Log(" + LinearRegister(linReg) + ");");
                }
            }
            else if (s is Exponential)
            {
                if (node.GetSubtree(0).IsTerminal())
                    sb.AppendLine(LinearRegister(linReg) + "=" + "Math.Exp(" + FormatRecursively(node.GetSubtree(0), linReg) + ");");
                else
                {
                    sb.Append(FormatRecursively(node.GetSubtree(0), linReg));
                    sb.AppendLine(LinearRegister(linReg) + "=" + "Math.Exp(" + LinearRegister(linReg) + ");");
                }
            }
            else if (s is Sine)
            {
                if (node.GetSubtree(0).IsTerminal())
                    sb.AppendLine(LinearRegister(linReg) + "=" + "Math.Sin(" + FormatRecursively(node.GetSubtree(0), linReg) + ");");
                else
                {
                    sb.Append(FormatRecursively(node.GetSubtree(0), linReg));
                    sb.AppendLine(LinearRegister(linReg) + "=" + "Math.Sin(" + LinearRegister(linReg) + ");");
                }
            }
            else if (s is Cosine)
            {
                if (node.GetSubtree(0).IsTerminal())
                    sb.AppendLine(LinearRegister(linReg) + "=" + "Math.Cos(" + FormatRecursively(node.GetSubtree(0), linReg) + ");");
                else
                {
                    sb.Append(FormatRecursively(node.GetSubtree(0), linReg));
                    sb.AppendLine(LinearRegister(linReg) + "=" + "Math.Cos(" + LinearRegister(linReg) + ");");
                }
            }
            else if (s is Tangent)
            {
                if (node.GetSubtree(0).IsTerminal())
                    sb.AppendLine(LinearRegister(linReg) + "=" + "Math.Tan(" + FormatRecursively(node.GetSubtree(0), linReg) + ");");
                else
                {
                    sb.Append(FormatRecursively(node.GetSubtree(0), linReg));
                    sb.AppendLine(LinearRegister(linReg) + "=" + "Math.Tan(" + LinearRegister(linReg) + ");");
                }
            }
            else if (s is SquareRoot)
            {
                if (node.GetSubtree(0).IsTerminal())
                    sb.AppendLine(LinearRegister(linReg) + "=" + "Math.Sqrt(" + FormatRecursively(node.GetSubtree(0), linReg) + ");");
                else
                {
                    sb.Append(FormatRecursively(node.GetSubtree(0), linReg));
                    sb.AppendLine(LinearRegister(linReg) + "=" + "Math.Sqrt(" + LinearRegister(linReg) + ");");
                }
            }
            else if (s is Square)
            {
                if (node.GetSubtree(0).IsTerminal())
                    sb.AppendLine(LinearRegister(linReg) + "=" + "Math.Pow(" + FormatRecursively(node.GetSubtree(0), linReg) + ",2);");
                else
                {
                    sb.Append(FormatRecursively(node.GetSubtree(0), linReg));
                    sb.AppendLine(LinearRegister(linReg) + "=" + "Math.Pow(" + LinearRegister(linReg) + ",2);");
                }
            }

            else if (s is IfThenElse)
            {
                //Is Always GreaterThan or LessThan
                sb.Append(FormatRecursively(node.GetSubtree(0), linReg));

                //TRUE Branch
                if (node.GetSubtree(1).IsTerminal())
                    sb.AppendLine(LinearRegister(linReg) + Assignment + FormatRecursively(node.GetSubtree(1), linReg) + ";");
                else
                    sb.Append(FormatRecursively(node.GetSubtree(1), linReg));

                //FALSE Branch
                if (node.GetSubtree(2).IsTerminal())
                    sb.AppendLine(LinearRegister(linReg + 1) + Assignment + FormatRecursively(node.GetSubtree(2), linReg + 1) + ";");
                else
                    sb.Append(FormatRecursively(node.GetSubtree(2), linReg + 1));


                sb.AppendLine("if (!flagstack[fc-1]) ");
                sb.AppendLine(LinearRegister(linReg) + "=" + LinearRegister(linReg + 1) + ";");
                sb.AppendLine("fc--;");

            }
            else if (s is GreaterThan)
            {
                if (node.GetSubtree(0).IsTerminal())
                    sb.AppendLine(LinearRegister(linReg) + Assignment + FormatRecursively(node.GetSubtree(0), linReg) + ";");
                else
                    sb.Append(FormatRecursively(node.GetSubtree(0), linReg));

                if (node.GetSubtree(1).IsTerminal())
                    sb.AppendLine(LinearRegister(linReg + 1) + Assignment + FormatRecursively(node.GetSubtree(1), linReg + 1) + ";");
                else
                    sb.Append(FormatRecursively(node.GetSubtree(1), linReg + 1));

                sb.AppendLine("if ( " + LinearRegister(linReg) + " > " + LinearRegister(linReg + 1) + " ) " + "flagstack[fc]=true; else flagstack[fc]=false;");
                sb.AppendLine("fc++;");

            }
            else if (s is LessThan)
            {
                if (node.GetSubtree(0).IsTerminal())
                    sb.AppendLine(LinearRegister(linReg) + Assignment + FormatRecursively(node.GetSubtree(0), linReg) + ";");
                else
                    sb.Append(FormatRecursively(node.GetSubtree(0), linReg));

                if (node.GetSubtree(1).IsTerminal())
                    sb.AppendLine(LinearRegister(linReg + 1) + Assignment + FormatRecursively(node.GetSubtree(1), linReg + 1) + ";");
                else
                    sb.Append(FormatRecursively(node.GetSubtree(1), linReg + 1));

                sb.AppendLine("if ( " + LinearRegister(linReg) + " < " + LinearRegister(linReg + 1) + " ) " + "flagstack[fc]=true; else flagstack[fc]=false;");
                sb.AppendLine("fc++;");
            }
            else if (s is Constant)
            {
                sb.Append((node as ConstantTreeNode).Value.ToString() + "F");
            }
            else if (s is LaggedVariable)
            {
                LaggedVariableTreeNode lvn = node as LaggedVariableTreeNode;
                if (lvn.Weight != 1)
                {
                    sb.Append(lvn.Weight.ToString());
                    sb.Append("*");
                }
                sb.Append(Variable(lvn.VariableName, lvn.Lag));
            }
            else if (s is HeuristicLab.Problems.DataAnalysis.Symbolic.Variable)
            {

                VariableTreeNode vn = node as VariableTreeNode;

                if (vn.Weight != 1)
                {
                    sb.Append(vn.Weight.ToString());
                    sb.Append("*");
                }
                sb.Append(Variable(vn.VariableName, 0));
            }
            else if (s is InternalState)
            {
                InternalStateTreeNode vn = node as InternalStateTreeNode;

                if (vn.Weight != 1)
                {
                    sb.Append(vn.Weight.ToString());
                    sb.Append("*");
                }
                sb.Append(InternalState(vn.VariableName));
            }

            return sb.ToString();
        }

        private static string LinearRegister(int idx)
        {
            if (idx >= 0 && idx <= 7)
                return ("f[" + idx + "]");
            else
                throw new ArgumentOutOfRangeException("Linear Registers are 0-7");
        }

        #endregion

        #region Operators Syntax

        private static string Addition { get { return "+="; } }
        private static string Subtraction { get { return "-="; } }
        private static string Multiplication { get { return "*="; } }
        private static string Division { get { return "/="; } }
        private static string Assignment { get { return "="; } }

        private string Variable(string vName, int lag)
        {
            string[] v = vName.Split('_');
            return "InputVars[\""+v[0]+"_"+v[1]+"_"+Math.Abs(lag)+"\"].Invoke()";
        }

        private string InternalState(string sName)
        {
            return "InternalStates[\""+sName+"\"].Invoke()";
        }

        private string Capitalize(string s)
        {
            if (String.IsNullOrEmpty(s))
                return string.Empty;

            return Char.ToUpper(s[0]) + s.Substring(1).ToLower();
        }

        private string PrintVarMap(List<string> vars)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(NewCSStrings.VarsHeader);
            foreach (string v in vars)
            {
                sb.Append("\"" + v.Replace('_',' ') + "\"");
                if (!vars.Last().Equals(v))
                    sb.AppendLine(",");
            }
            sb.AppendLine(NewCSStrings.VarsFooter);

            return sb.ToString();
        }
        #endregion
    }
}
