using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace GP4Sim.Trading.Simulation
{
    [Item("CostFunctionValue", "Represents a Cost Function Value")]
    [StorableClass]
    public class CommissionFunctionValue : ValueTypeValue<CommissionFunctionsEnum>, IComparable, IStringConvertibleValue
    {
        public static new Image StaticItemImage
        {
            get { return HeuristicLab.Common.Resources.VSImageLibrary.Field; }
        }

        protected CommissionFunction function;

        [StorableConstructor]
        protected CommissionFunctionValue(bool deserializing) : base(deserializing) { }
        protected CommissionFunctionValue(CommissionFunctionValue original, Cloner cloner)
            : base(original, cloner)
        {
            if (function == null)
            {
                function = CreateFunctionDelegate();
            }

        }
        public CommissionFunctionValue() : base() { }
        public CommissionFunctionValue(CommissionFunctionsEnum value)
            : base(value)
        {
            function = CreateFunctionDelegate();
        }

        public override IDeepCloneable Clone(Cloner cloner)
        {
            return new CommissionFunctionValue(this, cloner);
        }

        public override string ToString()
        {
            return value.ToString();
        }

        public CommissionFunction Function
        {
            get
            {
                if (function == null)
                    function = CreateFunctionDelegate();
                return function;
            }
        }

        private CommissionFunction CreateFunctionDelegate()
        {
            string cfName = Enum.GetName(typeof(CommissionFunctionsEnum), this.Value);
            MethodInfo cfMethodInfo = typeof(CommissionFunctions).GetMethod(cfName);

            return (CommissionFunction)Delegate.CreateDelegate(typeof(CommissionFunction), cfMethodInfo);
        }

        public virtual int CompareTo(object obj)
        {
            CommissionFunctionValue other = obj as CommissionFunctionValue;
            if (other != null)
                return Value.CompareTo(other.Value);
            else
                return Value.CompareTo(obj);
        }

        protected virtual bool Validate(string value, out string errorMessage)
        {
            if (value == null)
            {
                errorMessage = "Invalid Value (commission function must not be null)";
                return false;
            }
            else
            {
                errorMessage = string.Empty;
                return true;
            }
        }

        protected virtual string GetValue()
        {
            return Value.ToString();
        }

        protected virtual bool SetValue(string value)
        {
            int val;
            if (int.TryParse(value, out val))
            {
                if (Enum.IsDefined(typeof(CommissionFunctionsEnum), val))
                {
                    Value = (CommissionFunctionsEnum)val;
                    return true;
                }
                return false;
            }
            else
            {
                return false;
            }
        }

        #region IStringConvertibleValueMembers
        bool IStringConvertibleValue.Validate(string value, out string errorMessage)
        {
            return Validate(value, out errorMessage);
        }

        string IStringConvertibleValue.GetValue()
        {
            return GetValue();
        }

        bool IStringConvertibleValue.SetValue(string value)
        {
            return SetValue(value);
        }

        #endregion
    }
}
