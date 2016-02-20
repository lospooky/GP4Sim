using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using HeuristicLab.PluginInfrastructure;

namespace GP4Sim.CSharpAgents
{
    [Plugin("GP4Sim.CSharpAgents", "1.0.0.0")]
    [PluginFile("GP4Sim.CSharpAgents-1.0.dll", PluginFileType.Assembly)]

    [PluginDependency("HeuristicLab.Common", "3.3")]
    [PluginDependency("HeuristicLab.Core", "3.3")]
    [PluginDependency("HeuristicLab.Encodings.SymbolicExpressionTreeEncoding", "3.4")]
    [PluginDependency("HeuristicLab.Persistence", "3.3")]
    [PluginDependency("HeuristicLab.Problems.DataAnalysis.Symbolic", "3.4")]

    public class CSharpPlugin : PluginBase
    {

    }
}
