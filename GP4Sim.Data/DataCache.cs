using System;
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.DataAnalysis;

namespace GP4Sim.Data
{
    [Item("DataCache", "Fast Access Data Cache")]
    [StorableClass]
    public class DataCache : NamedItem
    {
        [Storable]
        private double[][] theCache = null;
        [Storable]
        private List<string> curCachedVars;
        [Storable]
        private int nRows = 0;
        private int nCols = 0;


        #region Constructors

        [StorableConstructor]
        private DataCache(bool deserializing)
            : base(deserializing)
        {

        }
        [StorableHook(HookType.AfterDeserialization)]
        private void AfterDeserialization() { }

        private DataCache(DataCache original, Cloner cloner)
            : base(original, cloner)
        {
            this.curCachedVars = original.curCachedVars;
            this.nRows = original.nRows;
            this.nCols = original.nCols;
            this.theCache = original.theCache;
        }

        public override IDeepCloneable Clone(Cloner cloner)
        {
            return new DataCache(this, cloner);
        }

        public DataCache()
        {
            name = ItemName;
            description = ItemDescription;
            curCachedVars = new List<string>();
        }
        public DataCache(string name)
            : base(name)
        {
            description = ItemDescription;
            curCachedVars = new List<string>();
        }

        public DataCache(string name, string description)
            : base(name, description)
        {
            curCachedVars = new List<string>();
        }

        #endregion

        #region Public Methods
        public void Load(IDataset ds, List<string> varToCache)
        {
            if (curCachedVars == null || !curCachedVars.SequenceEqual(varToCache))
            {

                curCachedVars = new List<string>(varToCache);
                int numRows = ds.Rows;
                int numCols = curCachedVars.Count;
                theCache = new double[numRows][];
                for (int i = 0; i < numRows; i++)
                {
                    double[] curRow = new double[numCols];
                    for (int j = 0; j < numCols; j++)
                    {
                        curRow[j] = ds.GetDoubleValue(curCachedVars[j], i);
                    }

                    theCache[i] = curRow;
                }
                nRows = numRows;
                nCols = numCols;
            }

        }

        public double[] GetRow(int row)
        {
            if (row >= 0 && row < nRows)
                return theCache[row];
            else
                throw new ArgumentOutOfRangeException("Out-of-bounds Row");
        }

        public double GetValue(int row, int col)
        {
            if (row >= 0 && row < nRows && col >= 0 && col < nCols)
                return theCache[row][col];
            else
                throw new ArgumentOutOfRangeException("Out-of-bounds Value");
        }

        public double[][] GetCache()
        {
            if (theCache != null)
                return theCache;
            else
                throw new InvalidOperationException("The DataCache is null");
        }

        #endregion

        #region Public Properties
        public bool IsEmpty
        {
            get { if (theCache == null || theCache.Length == 0) return true; else return false; }
        }
        #endregion
    }
}
