using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using HeuristicLab.PluginInfrastructure;

namespace GP4Sim.Data
{
    [Plugin("GP4Sim.Data", "1.0.0.0")]
    [PluginFile("GP4Sim.Data-1.0.dll", PluginFileType.Assembly)]

    [PluginDependency("HeuristicLab.Common", "3.3")]
    [PluginDependency("HeuristicLab.Core", "3.3")]
    [PluginDependency("HeuristicLab.Data", "3.3")]
    [PluginDependency("HeuristicLab.Persistence", "3.3")]
    [PluginDependency("HeuristicLab.Problems.DataAnalysis", "3.4")]

    public class DataPlugin : PluginBase
    {

    }
}
