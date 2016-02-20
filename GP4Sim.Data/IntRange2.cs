#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2012 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
 *
 * This file is part of HeuristicLab.
 *
 * HeuristicLab is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * HeuristicLab is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with HeuristicLab. If not, see <http://www.gnu.org/licenses/>.
 */
#endregion

using System;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace GP4Sim.Data
{
    [StorableClass]
    [Item("IntRange2", "Represents a range of values betweent start and end.")]
    public class IntRange2 : StringConvertibleValueTuple<IntValue, IntValue>
    {
        #region Properties
        public int Start
        {
            get { return Item1.Value; }
            set { Item1.Value = value; }
        }
        public int End
        {
            get { return Item2.Value; }
            set { Item2.Value = value; }
        }
        public int Size
        {
            get { return End - Start; }
        }
        #endregion

        #region Constructors
        [StorableConstructor]
        protected IntRange2(bool deserializing) : base(deserializing) { }
        protected IntRange2(IntRange2 original, Cloner cloner)
            : base(original, cloner)
        {
        }
        public override IDeepCloneable Clone(Cloner cloner)
        {
            return new IntRange2(this, cloner);
        }

        public IntRange2() : base(new IntValue(), new IntValue()) { }
        public IntRange2(IntValue start, IntValue end) : base(start, end) { }
        public IntRange2(int start, int end) : base(new IntValue(start), new IntValue(end)) { }
        #endregion

        #region Overrides
        public override string ToString()
        {
            return string.Format("Start: {0}, End: {1}", Start, End);
        }

        public override StringConvertibleValueTuple<IntValue, IntValue> AsReadOnly()
        {
            var readOnly = new IntRange2();
            readOnly.values = Tuple.Create<IntValue, IntValue>((IntValue)Item1.AsReadOnly(), (IntValue)Item2.AsReadOnly());
            readOnly.readOnly = true;
            return readOnly;
        }
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            IntRange2 ir = obj as IntRange2;
            if ((System.Object)ir == null)
                return false;

            return (Start == ir.Start) && (End == ir.End);
        }
        public bool Equals(IntRange2 ir)
        {
            if ((object)ir == null)
                return false;

            return (Start == ir.Start) && (End == ir.End);
        }
        public override int GetHashCode()
        {
            return Start ^ End;
        }
        #endregion
    }
}
