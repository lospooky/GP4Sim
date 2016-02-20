using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace GP4Sim.SimulationFramework.Solutions
{
    public class ResultPropertiesDictionary
    {
        private Dictionary<string, List<object>> dictionary;

        public ResultPropertiesDictionary() { dictionary = new Dictionary<string, List<object>>(); }


        public void Add(string pName, string vName, Type t, string description, string category, PropertyInfo pi)
        {
            List<object> l = new List<object>() { vName, t, description, category, pi };
            dictionary.Add(pName, l);
        }

        public List<string> PropertyNames
        {
            get
            {
                return dictionary.Keys.ToList();
            }
        }

        public string VisibleName(string pName)
        {
            return (string)(dictionary[pName][0]);
        }

        public Type Type(string pName)
        {
            return (Type)(dictionary[pName][1]);
        }

        public string Description(string pName)
        {
            return (string)(dictionary[pName][2]);
        }

        public string Category(string pName)
        {
            return (string)(dictionary[pName][3]);
        }

        public List<string> Categories
        {
            get
            {
                return dictionary.Select(x => (string)x.Value[3]).Distinct().ToList();
            }
        }

        public List<string> CategoryValues(string catName)
        {
            return dictionary.Where(x => ((string)x.Value[3]).Equals(catName)).Select(x => x.Key).ToList();
        }

        public PropertyInfo PropertyInfo(string pName)
        {
            return (PropertyInfo)(dictionary[pName][4]);
        }
    }
}
