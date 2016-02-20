using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using GP4Sim.Data;
using GP4Sim.SimulationFramework.Interfaces;
using GP4Sim.SymbolicTrees;
using HeuristicLab.Common;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.DataAnalysis.Symbolic;

namespace GP4Sim.SimulationFramework.Solutions
{
    [StorableClass]
    public abstract class SimulationModel<T, V> : SymbolicDataAnalysisModel, ISimulationModel<T>
        where T : class, ISimulationProblemData
        where V : class, ISimulationEnvelope
    {
        #region Properties
        [Storable]
        private Dictionary<IntRange2, V> cache;
        protected Dictionary<IntRange2, V> Cache
        {
            get { return cache; }
        }

       
        private ResultPropertiesDictionary resDictionary;
        public ResultPropertiesDictionary ResultProperties
        {
            get { return resDictionary; }
            set { resDictionary = value; }
        }


        [Storable]
        protected ISimulationEvaluator<T> evaluator;
        

        [Storable]
        private ISymbolicAbstractTreeInterpreter interpreter;
        public new ISymbolicAbstractTreeInterpreter Interpreter { get { return interpreter; } }

        [Storable]
        private ISymbolicExpressionGrammar grammar;
        public ISymbolicExpressionGrammar Grammar { get { return grammar; } }


        #endregion

        #region Constructors
        [StorableConstructor]
        protected SimulationModel(bool deserializing)
            : base(deserializing)
        {
            ResolveResultProperties();
        }
        protected SimulationModel(SimulationModel<T, V> original, Cloner cloner)
            : base(original, cloner)
        {
            //OBS
            //Will It Work? :::DDD Spurdo Sparde Snibety Snab
            cache = new Dictionary<IntRange2, V>(original.cache);
            this.evaluator = cloner.Clone(original.evaluator);
            this.interpreter = cloner.Clone(original.interpreter);
            this.grammar = cloner.Clone(original.grammar);
            //this.compiledTree = cloner.Clone(original.compiledTree);
            this.resDictionary = new ResultPropertiesDictionary();
            ResolveResultProperties();
        }

        protected SimulationModel(ISymbolicExpressionTree tree, ISymbolicAbstractTreeInterpreter interpreter, ISymbolicExpressionGrammar grammar, ISimulationEvaluator<T> evaluator,
          double lowerEstimationLimit = double.MinValue, double upperEstimationLimit = double.MaxValue)
            : base(tree, interpreter, lowerEstimationLimit, upperEstimationLimit)
        {
            cache = new Dictionary<IntRange2, V>();
            this.evaluator = evaluator;
            this.interpreter = interpreter;
            this.grammar = grammar;

            this.resDictionary = new ResultPropertiesDictionary();
            ResolveResultProperties();
        }
        #endregion

        #region Abstract Methods

        [VisibleName("Fitness Score")]
        public double GetFitnessScore(T problemData, IEnumerable<int> rows)
        {
            if (!isCached(rows))
                Evaluate(problemData, rows);

            return Cache[rows.ToIntRange()].FitnessScore;
        }


        //Retrieves Property Names
        public dynamic GetResult(T problemData, string pName, IEnumerable<int> rows)
        {
            if (!isCached(rows))
                Evaluate(problemData, rows);

            return (ResultProperties.PropertyInfo(pName).GetValue(Cache[rows.ToIntRange()], null));
        }

        #endregion

        #region Protected Methods
        protected bool isCached(IEnumerable<int> rows)
        {
            if (cache.ContainsKey(rows.ToIntRange()))
                return true;
            return false;
        }

        protected abstract void Evaluate(T problemData, IEnumerable<int> rows);


        public IEnumerable<double> GetRawOutputValues(T problemData, IEnumerable<int> rows)
        {
            if (!isCached(rows))
                Evaluate(problemData, rows);

            return Cache[rows.ToIntRange()].RawOutput;

        }
        #endregion

        #region Private Methods
        protected void ResolveResultProperties()
        {
            PropertyInfo[] pInfo = typeof(V).GetProperties();
            resDictionary = new ResultPropertiesDictionary();
            foreach (PropertyInfo pi in pInfo)
            {
                if (pi.GetCustomAttributes(false).Any(x => x.GetType().Name.Equals(typeof(VisibleName).Name)))
                {
                    string pname = pi.Name;
                    string vname = ((VisibleName)Attribute.GetCustomAttribute(pi, typeof(VisibleName))).Value;
                    string desc = ((Description)Attribute.GetCustomAttribute(pi, typeof(Description))).Value;
                    string category = ((Category)Attribute.GetCustomAttribute(pi, typeof(Category))).Value;
                    Type t = pi.PropertyType;

                    resDictionary.Add(pname, vname, t, desc, category, pi);
                }
            }
        }
        #endregion

    }
}
