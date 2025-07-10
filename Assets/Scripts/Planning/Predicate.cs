using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Planning
{
    public class Predicate
    {
        public Func<List<object>, bool> func { get; private set; }
        public List<Pointer> args { get; private set; }
        // Nullable value: null = unobserved
        public bool? value { get; private set; }

        public Predicate()
        {
            args = new List<Pointer>();
            value = null;
        }

        public Predicate(Func<List<object>, bool> func, List<Pointer> args)
        {
            this.func = func;
            this.args = args;
            this.value = null;
        }

        public Predicate(Func<List<object>, bool> func, List<Pointer> args, bool value)
        {
            this.func = func;
            this.args = args;
            this.value = value;
        }

        public void SetCondition(Func<List<object>, bool> func)
        {
            this.func = func;
        }


        public Predicate Instantiate(List<Pointer> newArgs, bool v)
        {
            return new Predicate(func, newArgs, v);
        }

        public bool Evaluate()
        {
            return Observe() == value;
        }

        public bool Observe()
        {
            List<object> argValues = args.Select(arg => arg.Get()).ToList();
            return func(argValues);
        }

        public bool IsInstantiated()
        {
            return args.All(arg => arg.value != null);
        }

        public bool IsOpposite(Predicate other)
        {
            return EqualsStructure(other) && this.value != other.value;
        }

        private bool EqualsStructure(Predicate other)
        {
            if (func?.Method.Name != other.func?.Method.Name)
                return false;

            if (args.Count != other.args.Count)
                return false;

            for (int i = 0; i < args.Count; i++)
            {
                if (!args[i].isSameValue(other.args[i]))
                    return false;
            }

            return true;
        }

        public override bool Equals(object obj)
        {
            if (obj is not Predicate other)
                return false;

            return EqualsStructure(other) && this.value == other.value;
        }

        public override string ToString()
        {
            string funcName = func?.Method.Name ?? "null";
            string argsString = string.Join(", ", args.Select(arg => arg.value?.ToString() ?? "null"));
            return $"{funcName}({argsString}) - {value}";
        }
    }
}
