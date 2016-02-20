using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GP4Sim.SimulationFramework.Solutions
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property)]
    public class VisibleName : System.Attribute
    {
        public readonly string Value;

        public VisibleName(string n)
        {
            this.Value = n;
        }
    }

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property)]
    public class TheType : System.Attribute
    {
        public readonly Type Value;

        public TheType(Type n)
        {
            this.Value = n;
        }
    }

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property)]
    public class Description : System.Attribute
    {
        public readonly string Value;

        public Description(string n)
        {
            this.Value = n;
        }
    }

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property)]
    public class Category : System.Attribute
    {
        public readonly string Value;

        public Category(string n)
        {
            this.Value = n;
        }
    }

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property)]
    public class SubCategory : System.Attribute
    {
        public readonly string Value;

        public SubCategory(string n)
        {
            this.Value = n;
        }
    }
}
