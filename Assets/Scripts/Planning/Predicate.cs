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

        private List<Func<List<Pointer>, bool>> constraints
        = new List<Func<List<Pointer>, bool>>();


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

        public Predicate(string name, Func<List<object>, bool> func, List<Pointer> args, bool? value)
        {
            this.Name = name;
            this.Condition = func;
            this.Args = args;
            this.Value = value;
        }

        public Predicate Instantiate(List<Pointer> newArgs, bool? v)
        {
            Predicate p = new Predicate(Name, Condition, newArgs, v);
            p.constraints.AddRange(this.constraints);
            return p;
        }



        public void AddConstraint(Func<List<Pointer>, bool> constraint)
        {
            constraints.Add(constraint);
        }

        public bool CheckConstraints()
        {
            foreach (Func<List<Pointer>, bool> c in constraints)
            {
                if (!c(Args))
                    return false;
            }
            return true;
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
                return false;

            if (Args.Count != other.Args.Count)
                return false;

            for (int i = 0; i < Args.Count; i++)
            {
                Pointer a = Args[i];
                Pointer b = other.Args[i];

                // Compare by reference if either is unbound
                if (!a.IsBound() || !b.IsBound())
                {
                    if (!ReferenceEquals(a, b))
                        return false;
                }
                else
                {
                    // Both bound: compare final values
                    if (!a.isSameValue(b))
                        return false;
                }
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
            string argsString = string.Join(", ", Args.Select(a => a?.ToString() ?? "null"));
            return $"{Name}({argsString}) - {Value}";
        }
    }
}
