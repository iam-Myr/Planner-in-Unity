using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace Planning
{
    public class Observable
    {
        public Func<List<object>, bool> func { get; private set; }
        public List<Pointer> args { get; private set; }

        public Observable() { }
        public Observable(Func<List<object>, bool> func, List<Pointer> args)
        {
            this.func = func;
            this.args = args;   
        }
        
        // Returns the value of func(args)
        public bool Observe()
        {
            List<object> argsValues= new List<object>();
            foreach(Pointer arg in args) {argsValues.Add(arg.Get());}
            return func(argsValues);
        }

        public void SetFunc(Func<List<object>, bool> func) { this.func = func; }
        public void SetArgs(List<Pointer> args) {  this.args = args; }

        public override bool Equals(object obj)
        {
            if (obj is not Observable other)
                return false;

            // Check function names
            if (this.func?.Method.Name != other.func?.Method.Name)
                return false;

            // Check argument count
            if (this.args.Count != other.args.Count)
                return false;

            // Check each argument
            for (int i = 0; i < this.args.Count; i++)
            {
                if (!this.args[i].isSameValue(other.args[i]))
                    return false;
            }

            return true;
        }

        public bool IsInstantiated()
        {
            return args.All(arg => arg.value != null);
        }

        public override string ToString()
        {
            string funcName = func?.Method.Name ?? "null";
            string argsString = string.Join(", ", args.Select(arg => arg.value?.ToString() ?? "null"));
            return $"{funcName}({argsString})";
        }

    }

    // A predicate is an observable (func, args) and the value it has to return when func(args)
    public class Predicate
    {
        public Observable observable;
        public bool value { get; private set; }

        public Predicate() { observable = new Observable()}
        public Predicate(Observable o, bool value) {
            observable = o;
            this.value = value;
        }

        public Predicate(Func<List<object>, bool> func, List<Pointer> args, bool value) : 
            this(new Observable(func, args), value) {}


        public void SetCondition(Func<List<object>, bool> func)
        {
            this.observable.SetFunc(func);
        }

        public Predicate CreateNew(List<Pointer> args, bool v)
        {
            Predicate p = new Predicate();
            p.value = v;
            p.observable.SetArgs(args);

            return p;
        }

        /*
        internal Predicate Clone()
        {
            Predicate clone = new Predicate();
            clone.func = func;
            clone.value = value;

            // Args
            clone.args = new List<Pointer>();
            foreach (Pointer arg in args)
                clone.args.Add(arg.Clone());

            return clone;
        }
        */


        // How to use .Contains correctly :)

        /*
        public override int GetHashCode()
        {
            int hash = func?.Method.Name.GetHashCode() ?? 0;
            hash = (hash * 397) ^ value.GetHashCode();
            foreach (var arg in args)
                hash = (hash * 397) ^ (arg.value?.GetHashCode() ?? 0);
            return hash;
        }*/

        public override bool Equals(object obj)
        {
            if (obj is not Predicate other)
                return false;

            if (!this.observable.Equals(other.observable)) 
                return false;

            // Check negation
            if (this.value != other.value)
                return false;

            return true;
        }

        public override string ToString()
        {
            return $"{observable} - {value}";
        }

        public bool IsOpposite(Predicate other)
        {
            if (!observable.Equals(other.observable))
                return false;

            // Return true if structure matches and negation is opposite
            return this.value != other.value;
        }

        // Can evaluate itself!
        public bool Evaluate()
        {
            return observable.Observe() == value;
        }

    }
}
