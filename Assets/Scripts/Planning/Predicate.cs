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

        public Predicate(string name, Func<List<object>, bool> func, List<Pointer> args, bool? value)
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

        public Predicate Instantiate(List<Pointer> newArgs, bool? v)
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

        public void ApplyBan(List<Predicate> preds)
        {
            foreach (Predicate p in preds.Where(IsThreat))
            {
                if (Args.Count(arg => arg.IsBound()) == Args.Count - 1) //All but one instantiated
                {
                    for (int i = 0; i < Args.Count; i++)
                    {
                        Args[i].Ban(new List<object> { p.Args[i] });
                    }
                }
            }
        }



        public bool IsInstantiated()
        {
            return Args.All(arg => arg.value != null);
        }

        public bool IsThreat(Predicate other)
        {
            // Must have same name
            if (!this.Name.Equals(other.Name))
                return false;

            // Must have same number of arguments
            if (this.Args.Count != other.Args.Count)
                return false;

            // Both must have a value and be opposite
            if (!this.Value.HasValue || !other.Value.HasValue)
                return false;

            return this.Value.Value != other.Value.Value;
        }

        public bool IsOpposite(Predicate other)
        {
            if (other == null)
                return false;

            // Same structure (ignore value)
            if (!EqualsStructure(other))
                return false;

            // Both must have values
            if (!this.Value.HasValue || !other.Value.HasValue)
                return false;

            // Values must be different
            return this.Value.Value != other.Value.Value;
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

        public Predicate Clone(Dictionary<Pointer, Pointer> map)
        {
            var newArgs = Args.Select(arg => arg.Clone(map)).ToList();

            return new Predicate(Name, Condition, newArgs, Value);
        }


        public static bool ContainsPredicate(List<Predicate> pList, Predicate p)
        {
            foreach (Predicate p_list in pList)
            {
                if (p.Equals(p_list))
                    return true;
            }
            return false;
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
