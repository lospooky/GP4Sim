using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Core;
using HeuristicLab.Common;
using System.Drawing;

namespace GP4Sim.SymbolicTrees
{
    [StorableClass]
    [Item("CompiledSymbolicExpressionTree", "Represents a compiled symbolic expression tree")]
    public class CompiledSymbolicExpressionTree : Item
    {
        public static new Image StaticItemImage
        {
            get { return HeuristicLab.Common.Resources.VSImageLibrary.Function; }
        }

        [Storable]
        private Func<double[], double> compiledTree;
        public Func<double[], double> CompiledTree
        {
            get { return compiledTree; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException();
                else if (value != compiledTree)
                {
                    compiledTree = value;
                    OnToStringChanged();
                }
            }
        }


        #region Constructors
        [StorableConstructor]
        protected CompiledSymbolicExpressionTree(bool deserializing) : base(deserializing) { }
        protected CompiledSymbolicExpressionTree(CompiledSymbolicExpressionTree original, Cloner cloner)
            : base(original, cloner)
        {
            this.compiledTree = original.compiledTree;
        }
        public CompiledSymbolicExpressionTree() : base() { }
        public CompiledSymbolicExpressionTree(Func<double[], double> compiledTree)
            : base()
        {
            this.CompiledTree = compiledTree;
        }
        public override IDeepCloneable Clone(Cloner cloner)
        {
            return new CompiledSymbolicExpressionTree(this, cloner);
        }
        #endregion
    }

}
