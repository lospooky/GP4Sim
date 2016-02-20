using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using HeuristicLab.PluginInfrastructure;

namespace GP4Sim.Collections
{
    [Plugin("GP4Sim.Collections", "1.0.0.0")]
    [PluginFile("GP4Sim.Collections-1.0.dll", PluginFileType.Assembly)]

    [PluginDependency("HeuristicLab.Collections", "3.3")]
    [PluginDependency("HeuristicLab.Common", "3.3")]
    [PluginDependency("HeuristicLab.Common.Resources", "3.3")]
    [PluginDependency("HeuristicLab.Core", "3.3")]
    [PluginDependency("HeuristicLab.Optimization", "3.3")]
    [PluginDependency("HeuristicLab.Persistence", "3.3")]

    public class CollectionsPlugin : PluginBase
    {

    }
}
