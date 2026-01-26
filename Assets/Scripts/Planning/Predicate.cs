using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Planning
{
    public class Predicate
    {
        public Func<List<object>, bool> Condition { get; private set; }
        public List<Pointer> Args { get; private set; }
        // Nullable value: null = unobserved
        public bool? Value { get; private set; } // Negation or not

        public String Name;

        public Predicate(string name)
        {
            Args = new List<Pointer>();
            Value = null;
            Name = name;
        }

        public Predicate(string name, Func<List<object>, bool> func, List<Pointer> args)
        {
            this.Name = name;
            this.Condition = func;
            this.Args = args;
            this.Value = null;
        }

        public Predicate(string name, Func<List<object>, bool> func, List<Pointer> args, bool value)
        {
            this.Name = name;
            this.Condition = func;
            this.Args = args;
            this.Value = value;
        }

        public void SetCondition(Func<List<object>, bool> func)
        {
            this.Condition = func;
        }

        public Predicate Instantiate(List<Pointer> newArgs, bool v)
        {
            return new Predicate(Name, Condition, newArgs, v);
        }

        public bool Evaluate()
        {
            return Observe() == Value;
        }

        public bool Observe()
        {
            List<object> argValues = Args.Select(arg => arg.Get()).ToList();
            return Condition(argValues);
        }

        public bool IsInstantiated()
        {
            return Args.All(arg => arg.value != null);
        }

        public bool IsOpposite(Predicate other)
        {
            return EqualsStructure(other) && this.Value != other.Value;
        }

        private bool EqualsStructure(Predicate other)
        {
            if (!Name.Equals(other.Name))
            {
                //Debug.Log("Predicate names do not match: " + Name + " vs " + other.Name);
                return false;
            }


            if (Args.Count != other.Args.Count)
                return false;

            for (int i = 0; i < Args.Count; i++)
            {
                if (!Args[i].isSameValue(other.Args[i]))
                    return false;
            }

            return true;
        }

        public override bool Equals(object obj)
        {
            if (obj is not Predicate other)
                return false;

            return EqualsStructure(other) && this.Value == other.Value;
        }

        public override string ToString()
        {
            string argsString = string.Join(", ", Args.Select(arg => arg.value?.ToString() ?? "null"));
            return $"{Name}({argsString}) - {Value}";
        }
    }
}
