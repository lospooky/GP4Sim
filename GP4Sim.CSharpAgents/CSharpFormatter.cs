using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.DataAnalysis.Symbolic;
using GP4Sim.SymbolicTrees;


namespace GP4Sim.CSharpAgents
{
    [Item("Deltix Agent C# Formatter 1.0", "Formats SymbolicTrees into Deltix-Runnable C# Agents, NO LAG")]
    [StorableClass]
    public sealed class CSharpFormatter : NamedItem, ISymbolicExpressionTreeStringFormatter
    {
        //private int currentLag;
        //private int currentIndexNumber;
        //private int linenumber;

        private Dictionary<string, int> varMap;

        #region Constructors

        [StorableConstructor]
        private CSharpFormatter(bool deserializing) : base(deserializing) { }
        private CSharpFormatter(CSharpFormatter original, Cloner cloner) : base(original, cloner) { }
        public CSharpFormatter()
            : base()
        {
            Name = ItemName;
            Description = ItemDescription;
        }


        public override IDeepCloneable Clone(Cloner cloner)
        {
            return new CSharpFormatter(this, cloner);
        }

        #endregion

        #region Formatting Parser Logic

        public string Format(ISymbolicExpressionTree symbolicExpressionTree)
        {
            StringBuilder sb = new StringBuilder();
            if (varMap == null)
                varMap = new Dictionary<string, int>();

            string body = FormatRecursively(symbolicExpressionTree.Root, 0);

            sb.AppendLine(CSStrings.Header);
            sb.AppendLine();
            sb.AppendLine(PrintDescriptiveVarMap());
            sb.AppendLine(FormatRecursively(symbolicExpressionTree.Root, 0));
            sb.AppendLine();
            sb.AppendLine(CSStrings.Footer);


            return sb.ToString();
        }

        public string FormatFull(ISymbolicExpressionTree symbolicexpressionTree, List<string> variables)
        {
            
            GenerateVarMap(variables);
            return Format(symbolicexpressionTree);
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
                    sb.AppendLine(LinearRegister(linReg) + "=" + "(float)Math.Log(" + FormatRecursively(node.GetSubtree(0), linReg) + ");");
                else
                {
                    sb.Append(FormatRecursively(node.GetSubtree(0), linReg));
                    sb.AppendLine(LinearRegister(linReg) + "=" + "(float)Math.Log(" + LinearRegister(linReg) + ");");
                }
            }
            else if (s is Exponential)
            {
                if (node.GetSubtree(0).IsTerminal())
                    sb.AppendLine(LinearRegister(linReg) + "=" + "(float)Math.Exp(" + FormatRecursively(node.GetSubtree(0), linReg) + ");");
                else
                {
                    sb.Append(FormatRecursively(node.GetSubtree(0), linReg));
                    sb.AppendLine(LinearRegister(linReg) + "=" + "(float)Math.Exp(" + LinearRegister(linReg) + ");");
                }
            }
            else if (s is Sine)
            {
                if (node.GetSubtree(0).IsTerminal())
                    sb.AppendLine(LinearRegister(linReg) + "=" + "(float)Math.Sin(" + FormatRecursively(node.GetSubtree(0), linReg) + ");");
                else
                {
                    sb.Append(FormatRecursively(node.GetSubtree(0), linReg));
                    sb.AppendLine(LinearRegister(linReg) + "=" + "(float)Math.Sin(" + LinearRegister(linReg) + ");");
                }
            }
            else if (s is Cosine)
            {
                if (node.GetSubtree(0).IsTerminal())
                    sb.AppendLine(LinearRegister(linReg) + "=" + "(float)Math.Cos(" + FormatRecursively(node.GetSubtree(0), linReg) + ");");
                else
                {
                    sb.Append(FormatRecursively(node.GetSubtree(0), linReg));
                    sb.AppendLine(LinearRegister(linReg) + "=" + "(float)Math.Cos(" + LinearRegister(linReg) + ");");
                }
            }
            else if (s is Tangent)
            {
                if (node.GetSubtree(0).IsTerminal())
                    sb.AppendLine(LinearRegister(linReg) + "=" + "(float)Math.Tan(" + FormatRecursively(node.GetSubtree(0), linReg) + ");");
                else
                {
                    sb.Append(FormatRecursively(node.GetSubtree(0), linReg));
                    sb.AppendLine(LinearRegister(linReg) + "=" + "(float)Math.Tan(" + LinearRegister(linReg) + ");");
                }
            }
            else if (s is SquareRoot)
            {
                if (node.GetSubtree(0).IsTerminal())
                    sb.AppendLine(LinearRegister(linReg) + "=" + "(float)Math.Sqrt(" + FormatRecursively(node.GetSubtree(0), linReg) + ");");
                else
                {
                    sb.Append(FormatRecursively(node.GetSubtree(0), linReg));
                    sb.AppendLine(LinearRegister(linReg) + "=" + "(float)Math.Sqrt(" + LinearRegister(linReg) + ");");
                }
            }
            else if (s is Square)
            {
                if (node.GetSubtree(0).IsTerminal())
                    sb.AppendLine(LinearRegister(linReg) + "=" + "(float)Math.Pow(" + FormatRecursively(node.GetSubtree(0), linReg) + ",2);");
                else
                {
                    sb.Append(FormatRecursively(node.GetSubtree(0), linReg));
                    sb.AppendLine(LinearRegister(linReg) + "=" + "(float)Math.Pow(" + LinearRegister(linReg) + ",2);");
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


                sb.AppendLine("if (!flag) ");
                sb.AppendLine(LinearRegister(linReg) + "=" + LinearRegister(linReg + 1) + ";");

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

                sb.AppendLine("if ( " + LinearRegister(linReg) + " > " + LinearRegister(linReg + 1) + " ) " + "flag=true; else flag=false;");

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

                sb.AppendLine("if ( " + LinearRegister(linReg) + " < " + LinearRegister(linReg + 1) + " ) " + "flag=true; else flag=false;");
            }
            else if (s is Constant)
            {
                sb.Append((node as ConstantTreeNode).Value.ToString() + "F");
            }
            else if (s is HeuristicLab.Problems.DataAnalysis.Symbolic.Variable)
            {

                VariableTreeNode vn = node as VariableTreeNode;

                if (vn.Weight != 1)
                {
                    sb.Append("(float)");
                    sb.Append(vn.Weight.ToString());
                    sb.Append("*");
                }
                sb.Append(Variable(vn.VariableName));
            }
            else if (s is InternalStateTreeNode)
            {
                InternalStateTreeNode vn = node as InternalStateTreeNode;

                if (vn.Weight != 1)
                {
                    sb.Append("(float)");
                    sb.Append(vn.Weight.ToString());
                    sb.Append("*");
                }
                sb.Append(Variable(vn.VariableName));

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

        private string Variable(string vName)
        {
            if (!varMap.Keys.Contains(vName))
                varMap.Add(vName, varMap.Count);



            /*
            int nIdx = vName.IndexOfAny(new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' });
            if (nIdx == 0)
                throw new ArgumentOutOfRangeException("Invalid Variable Name(s)");
            else
            {
                string result = vName.Substring(0, nIdx);
                result += "[";
                result += vName.Substring(nIdx);
                result += "]";

                return result;
            }
             */
            return "x[" + varMap[vName] + "]";
        }
        #endregion

        private string PrintDescriptiveVarMap()
        {
            StringBuilder sb = new StringBuilder();

            foreach (string key in varMap.Keys)
            {
                sb.AppendLine("// x[" + varMap[key] + "] is " + key);
            }

            return sb.ToString();
        }

        private void GenerateVarMap(List<string> variables)
        {
            varMap = new Dictionary<string, int>();
            foreach (string v in variables)
                varMap.Add(v, varMap.Count);
        }

    }
}
