using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GP4Sim.SimulationFramework.Parameters;
using GP4Sim.Trading.Interfaces;
using GP4Sim.Trading.Simulation;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.DataAnalysis;

namespace GP4Sim.Trading.Parameters
{
    public class TradingSimulationParameters : SimulationParameters, ITradingSimulationParameters
    {
        #region Parameter Names
        protected const string OrderSizeParameterName = "Min Order Size";
        protected const string NAVParameterName = "Starting NAV";
        protected const string SlackParameterName = "Slack";
        protected const string MinTradesParameterName = "Minimum Trades";
        protected const string ExposureLimitsParameterName = "Exposure Limits";
        protected const string CommissionFunctionParameterName = "Commission Function";
        protected const string PriceVariableParameterName = "Traded Instrument Price";
        #endregion

        #region Parameters

        public IValueParameter<DoubleValue> StartingNAVParameter
        {
            get { return (IValueParameter<DoubleValue>)Parameters[NAVParameterName]; }
        }

        public IValueParameter<IntValue> MinOrderSizeParameter
        {
            get { return (IValueParameter<IntValue>)Parameters[OrderSizeParameterName]; }
        }

        public IValueParameter<DoubleLimit> ExposureLimitsParameter
        {
            get { return (IValueParameter<DoubleLimit>)Parameters[ExposureLimitsParameterName]; }
        }

        public IValueParameter<DoubleValue> SlackParameter
        {
            get { return (IValueParameter<DoubleValue>)Parameters[SlackParameterName]; }
        }

        public IValueParameter<IntValue> MinTradesParameter
        {
            get { return (IValueParameter<IntValue>)Parameters[MinTradesParameterName]; }
        }

        public IConstrainedValueParameter<CommissionFunctionValue> CommissionFunctionParameter
        {
            get { return (IConstrainedValueParameter<CommissionFunctionValue>)Parameters[CommissionFunctionParameterName]; }
        }

        public IConstrainedValueParameter<StringValue> PriceVariableParameter
        {
            get { return (IConstrainedValueParameter<StringValue>)Parameters[PriceVariableParameterName]; }
        }

        #endregion

        #region Properties
        public double StartingNAV
        {
            get { return StartingNAVParameter.Value.Value; }
        }

        public int MinOrderSize
        {
            get { return MinOrderSizeParameter.Value.Value; }
        }

        public double Slack
        {
            get { return SlackParameter.Value.Value; }
        }

        public DoubleLimit ExposureLimits
        {
            get { return ExposureLimitsParameter.Value; }
        }

        public int MinTrades
        {
            get { return MinTradesParameter.Value.Value; }
        }

        public CommissionFunctionValue CommissionFcn
        {
            get { return CommissionFunctionParameter.Value; }
        }
        #endregion

        #region Constructors

        public TradingSimulationParameters()
        {
            Parameters.Add(new ValueParameter<DoubleValue>(NAVParameterName, new DoubleValue(500000)));
            Parameters.Add(new ValueParameter<IntValue>(OrderSizeParameterName, new IntValue(10)));
            Parameters.Add(new ValueParameter<DoubleLimit>(ExposureLimitsParameterName, new DoubleLimit(0, 100)));
            Parameters.Add(new ValueParameter<DoubleValue>(SlackParameterName, new DoubleValue(1.5)));
            Parameters.Add(new ValueParameter<IntValue>(MinTradesParameterName, new IntValue(1)));
            Parameters.Add(new ConstrainedValueParameter<CommissionFunctionValue>(CommissionFunctionParameterName, new ItemSet<CommissionFunctionValue>(CommissionFunctionFactory.List()), CommissionFunctionFactory.List().First()));
        }

        protected TradingSimulationParameters(TradingSimulationParameters original, Cloner cloner)
            : base(original, cloner)
        {
            RegisterEventHandlers();
        }

        public override IDeepCloneable Clone(Cloner cloner)
        {
            return new TradingSimulationParameters(this, cloner);

        }

        [StorableConstructor]
        protected TradingSimulationParameters(bool deserializing) : base(deserializing) { }

        [StorableHook(HookType.AfterDeserialization)]
        private void AfterDeserialization()
        {
            RegisterEventHandlers();
        }
        #endregion


        protected override void RegisterEventHandlers()
        {
            StartingNAVParameter.ValueChanged += new EventHandler(Parameter_ValueChanged);
            MinOrderSizeParameter.ValueChanged += new EventHandler(Parameter_ValueChanged);
            ExposureLimitsParameter.ValueChanged += new EventHandler(Parameter_ValueChanged);
            SlackParameter.ValueChanged += new EventHandler(Parameter_ValueChanged);
            MinTradesParameter.ValueChanged += new EventHandler(Parameter_ValueChanged);
            CommissionFunctionParameter.ValueChanged += new EventHandler(Parameter_ValueChanged);

        }
    }
}
