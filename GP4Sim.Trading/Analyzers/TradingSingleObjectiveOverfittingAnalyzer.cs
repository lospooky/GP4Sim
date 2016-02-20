using GP4Sim.SimulationFramework.Analyzers;
using GP4Sim.Trading.Interfaces;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace GP4Sim.Trading.Analyzers
{
    [Item("TradingSingleObjectiveOverfittingAnalyzer", "Calculates and tracks correlation of training and validation fitness of symbolic Trading models.")]
    [StorableClass]
    public class TradingSingleObjectiveOverfittingAnalyzer : SimulationSingleObjectiveOverfittingAnalyzer<ITradingSingleObjectiveEvaluator, ITradingProblemData>
    {
        [StorableConstructor]
        private TradingSingleObjectiveOverfittingAnalyzer(bool deserializing) : base(deserializing) { }
        private TradingSingleObjectiveOverfittingAnalyzer(TradingSingleObjectiveOverfittingAnalyzer original, Cloner cloner) : base(original, cloner) { }
        public TradingSingleObjectiveOverfittingAnalyzer()
            : base()
        {

        }

        public override IDeepCloneable Clone(Cloner cloner)
        {
            return new TradingSingleObjectiveOverfittingAnalyzer(this, cloner);
        }
    }
}
