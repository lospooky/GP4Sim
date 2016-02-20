using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using HeuristicLab.PluginInfrastructure;

namespace GP4Sim.Collections.Views
{
    [Plugin("GP4Sim.Collections.Views", "1.0.0.0")]
    [PluginFile("GP4Sim.Collections.Views-1.0.dll", PluginFileType.Assembly)]

    [PluginDependency("HeuristicLab.Collections", "3.3")]
    [PluginDependency("HeuristicLab.Common", "3.3")]
    [PluginDependency("HeuristicLab.Common.Resources", "3.3")]
    [PluginDependency("HeuristicLab.Core", "3.3")]
    [PluginDependency("HeuristicLab.Core.Views", "3.3")]
    [PluginDependency("HeuristicLab.Encodings.SymbolicExpressionTreeEncoding", "3.4")]
    [PluginDependency("HeuristicLab.Optimization", "3.3")]
    [PluginDependency("HeuristicLab.MainForm", "3.3")]
    [PluginDependency("HeuristicLab.MainForm.WindowsForms", "3.3")]
    [PluginDependency("HeuristicLab.Problems.DataAnalysis", "3.4")]
    [PluginDependency("HeuristicLab.Problems.DataAnalysis.Symbolic", "3.4")]


    public class CollectionViewsPlugin : PluginBase
    {

    }
}
