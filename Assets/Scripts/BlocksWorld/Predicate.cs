using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class Predicate
{
    public Func<object[], bool> func { get; private set; } // CHECK THIS OUT
    public List<Pointer> args { get; private set; }
    public bool not_negated { get; private set; }

    public Predicate() { }

    public Predicate(Func<object[], bool> func, List<Pointer> args, bool neg)
    {
        this.func = func;
        this.args = args;
        this.not_negated = neg;
    }

    internal Predicate Clone()
    {
        Predicate clone = new Predicate();
        clone.func = func;
        clone.not_negated = not_negated;

        // Args
        clone.args = new List<Pointer>();
        foreach (Pointer arg in args)
            clone.args.Add(arg.Clone());

        return clone;
    }

    // How to use .Contains correctly :)
    public override int GetHashCode()
    {
        int hash = func?.Method.Name.GetHashCode() ?? 0;
        hash = (hash * 397) ^ not_negated.GetHashCode();
        foreach (var arg in args)
            hash = (hash * 397) ^ (arg.value?.GetHashCode() ?? 0);
        return hash;
    }


    public override bool Equals(object obj)
    {
        if (obj is not Predicate other)
            return false;

        // Check function names
        if (this.func?.Method.Name != other.func?.Method.Name)
            return false;

        // Check negation
        if (this.not_negated != other.not_negated)
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

    public bool IsOpposite(Predicate other)
    {
        if (this.func?.Method.Name != other.func?.Method.Name)
            return false;

        if (this.args.Count != other.args.Count)
            return false;

        for (int i = 0; i < this.args.Count; i++)
        {
            if (!this.args[i].isSameValue(other.args[i]))
                return false;
        }

        // Return true if structure matches and negation is opposite
        return this.not_negated != other.not_negated;
    }

}
