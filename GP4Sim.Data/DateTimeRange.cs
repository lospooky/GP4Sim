using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace GP4Sim.Data
{
    [StorableClass]
    [Item("DateTimeRange", "Represents a range of datetimes between start and end.")]
    public class DateTimeRange : StringConvertibleValueTuple<DateTimeValue, DateTimeValue>
    {
        public DateTime Start
        {
            get { return Item1.Value; }
            set { Item1.Value = value; }
        }

        public DateTime End
        {
            get { return Item2.Value; }
            set { Item2.Value = value; }
        }

        public TimeSpan Range
        {
            get { return Item2.Value - Item1.Value; }
        }

        [StorableConstructor]
        protected DateTimeRange(bool deserializing) : base(deserializing) { }
        protected DateTimeRange(DateTimeRange original, Cloner cloner)
            : base(original, cloner)
        { }

        public override IDeepCloneable Clone(Cloner cloner)
        {
            return new DateTimeRange(this, cloner);
        }

        public DateTimeRange() : base(new DateTimeValue(), new DateTimeValue()) { }
        public DateTimeRange(DateTimeValue start, DateTimeValue end) : base(start, end) { }
        public DateTimeRange(DateTime start, DateTime end) : base(new DateTimeValue(start), new DateTimeValue(end)) { }

        public override string ToString()
        {
            return string.Format("{0} - {1}", Start.ToString("dd/MM/yyyy"), End.ToString("dd/MM/yyyy"));
        }

        public override StringConvertibleValueTuple<DateTimeValue, DateTimeValue> AsReadOnly()
        {
            var readOnly = new DateTimeRange();
            readOnly.values = Tuple.Create<DateTimeValue, DateTimeValue>((DateTimeValue)Item1.AsReadOnly(), (DateTimeValue)Item2.AsReadOnly());
            readOnly.readOnly = true;

            return readOnly;
        }
    }
}