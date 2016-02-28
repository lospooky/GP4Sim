using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GP4Sim.CSharpAgents;
using GP4Sim.Data;
using GP4Sim.Trading.Simulation;
using HeuristicLab.Problems.DataAnalysis;


namespace GP4Sim.Trading.Simulation
{
    public class TradingSimulationRunner
    {
        private bool hasRun = false;
        private AgentFunction Agent;

        private Pacemaker Pacemaker;
        private TradingSimulationRunMode runMode = TradingSimulationRunMode.EVALUATION;
        private TradingFitnessType fitnessType;
        private bool produceLog = false;
        private bool badOutputFlag = false;
        private bool negativeNAVFlag = false;
        private double[] iv;
        private double maxReturn;
        private int varCount;
        private int statesCount;
        private TradingSimulation Sim;
        private Logger Log;
        private event LogLineEventHandler LogEvent;
        private DataCache data;

        #region Constructor

        public TradingSimulationRunner(AgentFunction agent, IDataAnalysisProblemData problemData, DataCache dataCache, SimulationParameters_Internal simParameters, string tvn, string pvn, int vCount, int sCount, int minLag, bool invertPrices, IEnumerable<int> rows, TradingSimulationRunMode rm, TradingFitnessType fitType, double maxReturn, bool logFlag = false)
        {
            fitnessType = fitType;
            this.maxReturn = maxReturn;
            Sim = new TradingSimulation(rm, fitnessType, simParameters, invertPrices, maxReturn);
            data = dataCache;
            Agent = agent;
            Pacemaker = new Pacemaker(problemData, invertPrices, rows, tvn, pvn);
            Pacemaker.AdjustForLag(minLag);
            runMode = rm;
            produceLog = logFlag;
            varCount = vCount;
            statesCount = sCount;
            iv = new double[varCount + statesCount];
            if (produceLog)
            {
                Log = new Logger();
            }

        }

        #endregion


        #region Public Methods
        public void Run()
        {
            if (produceLog)
                RegisterEvents();

            do
            {
                if (Pacemaker.CurrentPrice > 0)
                {
                    //Log Events is Inside
                    Sim.Step(Pacemaker.CurrentPrice, Pacemaker.CurrentTimePoint, Pacemaker.IsNewDay);
                    if (produceLog)
                    {
                        OnLogLine(Logger.NewDataPoint(Sim.DpIndex, Sim.CurrentTimePoint, Sim.State.NAV, Sim.CurrentPrice));
                        OnLogLine(Logger.NAV(Sim.State.AccountValue, Sim.State.CurrentPositionValue, Sim.State.NAV));
                    }

                    //double[] inputs = InputVector();
                    //if (produceLog)
                        //OnLogLine(Logger.Inputs(inputs));

                    double agentOutput = Agent(Pacemaker.CurrentRow, data.GetCache(), Sim.GetCurrentStates);
                    if (produceLog)
                        OnLogLine(Logger.Output(agentOutput));

                    if (!OutputSanityCheck(agentOutput))
                    {
                        badOutputFlag = true;
                        break;
                    }
                    //Output limit checking
                    long tradeSignal = Sim.GPOutputToTradeSignal(agentOutput);
                    if (produceLog)
                        OnLogLine(Logger.TradeSignal(tradeSignal));

                    //Log Events is inside
                    Sim.TradeAction(tradeSignal);

                    if (!NegativeNAVCheck(Sim.State.NAV, Sim.State.PosQuantity))
                    {
                        negativeNAVFlag = true;
                        break;
                    }

                    if (produceLog)
                        OnLogLine(Logger.DpSeparator);
                }

            } while (Pacemaker.Next());

            Sim.WrapUp();
            hasRun = true;
        }

        #endregion


        #region Private Methods

        //Deprecated
        private double[] InputVector()
        {
            //IEnumerable<double> rowVariables = Dataset.Dataset.DoubleVariables.Select(x => Dataset.Dataset.GetDoubleValue(x, Pacemaker.CurrentRow));
            //double[] rowVariables = Dataset.Dataset.GetDoubleArray(Pacemaker.CurrentRow);

            //Is Already Limited To Allowed Vars
            double[] rowVariables = data.GetRow(Pacemaker.CurrentRow);
            double[] states = Sim.GetCurrentStates;

            Array.Copy(rowVariables, iv, varCount);
            Array.Copy(states, 0, iv, varCount, statesCount);


            //return rowVariables.Concat(states).ToArray();
            return iv;
        }

        private bool OutputSanityCheck(double value)
        {
            if (double.IsInfinity(value) || double.IsNaN(value))
                return false;

            return true;
        }

        private bool NegativeNAVCheck(double nav, double posSize)
        {
            if (nav < 0 || posSize < 0)
                return false;

            return true;
        }

        private void RegisterEvents()
        {
            Sim.LogEvent += Log.LogLine;
            this.LogEvent += Log.LogLine;
        }

        private void UnregisterEvents()
        {

        }
        private void OnLogLine(string line)
        {

            LogEvent(this, new LogLineEventArgs(line));
        }


        #endregion



        #region Public Properties
        public bool HasRun { get { return hasRun; } }

        public TradingSimulationRunMode RunMode { get { return runMode; } }

        public ITradingEnvelope Results { get { return Sim.Results; } }

        public string LogText
        {
            get
            {
                if (Log == null)
                    return "There should be a log";
                else
                    return Log.GetLog;
            }
        }

        public double FitnessScore
        {
            get
            {
                if (fitnessType == TradingFitnessType.NAV)
                {
                    double score;
                    if (badOutputFlag)
                        score = (double)FitnessErrorCodes.INVALID_OUTPUT;
                    if (negativeNAVFlag)
                        score = (double)FitnessErrorCodes.IN_THE_RED;
                    else
                        score = Sim.FinalScore;

                    return FitnessScoring.NEGEXP(score);
                }
                else if (fitnessType == TradingFitnessType.Score || fitnessType == TradingFitnessType.ScorePenalties || fitnessType == TradingFitnessType.ScoreWeighted ||
                    fitnessType == TradingFitnessType.Relative || fitnessType == TradingFitnessType.ActivityAware || fitnessType == TradingFitnessType.Sortino || fitnessType == TradingFitnessType.AnnualizedSortino)
                {
                    if (badOutputFlag)
                    {
                        if (fitnessType == TradingFitnessType.Score || fitnessType == TradingFitnessType.ScorePenalties || fitnessType == TradingFitnessType.ScoreWeighted || fitnessType == TradingFitnessType.Relative)
                            return -200;
                        else if (fitnessType == TradingFitnessType.ActivityAware)
                            return -Math.Exp(3) + 1;
                        else if (fitnessType == TradingFitnessType.Sortino || fitnessType == TradingFitnessType.AnnualizedSortino)
                            return -100;
                        else
                            return 0;
                    }
                    else
                        return Sim.FinalScore;
                }
                   
                    
                else
                    return 0;
            }
        }

        public void TearDown()
        {
            Agent = null;
            Pacemaker = null;
            if(Log!=null)
                Log.Teardown();
            Log = null;
            data = null;
            Sim.Teardown();
            Sim = null;
        }
        #endregion
    }
}
