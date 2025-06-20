using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class Predicate
{
    public Func<List<object>, bool> func { get; private set; }
    // CHECK THIS OUT
    public List<object> args { get; private set; }
    public bool evaluation { get; private set; }

    public Predicate() { }

    public Predicate(Func<List<object>, bool> func, List<object> args, bool neg)
    {
        this.func = func;
        this.args = args;
        this.evaluation = neg;
    }

    /*
    internal Predicate Clone()
    {
        Predicate clone = new Predicate();
        clone.func = func;
        clone.evaluation = evaluation;

        // Args
        clone.args = new List<object>();
        foreach (object arg in args)
            clone.args.Add(arg.Clone());

        return clone;
    }*/

    // How to use .Contains correctly :)
    public override int GetHashCode()
    {
        int hash = func.Method.Name.GetHashCode();
        hash = (hash * 397) ^ evaluation.GetHashCode();
        foreach (var arg in args)
            hash = (hash * 397) ^ (arg.GetHashCode());
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
        if (this.evaluation != other.evaluation)
            return false;

        // Check argument count
        if (this.args.Count != other.args.Count)
            return false;

        // Check each argument
        for (int i = 0; i < this.args.Count; i++)
        {
            if (!this.args[i].Equals(other.args[i]))
                return false;
        }

        return true;
    }

    public bool IsInstantiated()
    {
        return args.All(arg => arg != null);
    }


    public override string ToString()
    {
        string funcName = func?.Method.Name ?? "null";
        string argsString = string.Join(", ", args.Select(arg => arg.ToString() ?? "null"));
        return $"{funcName}({argsString}) - {evaluation}";
    }

    public bool IsOpposite(Predicate other)
    {
        if (this.func?.Method.Name != other.func?.Method.Name)
            return false;

        if (this.args.Count != other.args.Count)
            return false;

        for (int i = 0; i < this.args.Count; i++)
        {
            if (!this.args[i].Equals(other.args[i]))
                return false;
        }

        // Return true if structure matches and negation is opposite
        return this.evaluation != other.evaluation;
    }

    public bool EvaluatePredicate()
    {
        if (func == null)
        {
            Debug.LogError("Predicate function is null.");
            return false;
        }

        try
        {
            bool result = func.Invoke(args);

            return result == evaluation;
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to evaluate predicate {this}: {ex.Message}");
            return false;
        }
    }


}
