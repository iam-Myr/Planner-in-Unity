using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Predicate
{
    public Func<object[], bool> func { get; private set; } // CHECK THIS OUT
    public List<SharedVar> args { get; private set; }

    public Predicate() {}

    public Predicate(Func<object[], bool> func, List<SharedVar> args)
    {
        this.func = func;
        this.args = args;
    }

    internal Predicate Clone()
    {
        Predicate clone = new Predicate();
        clone.func = func;

        // Args
        clone.args = new List<SharedVar>();
        foreach (SharedVar arg in args)
            clone.args.Add(arg.Clone());

        return clone;
    }

    public bool isSame(Predicate sD)
    {
        // Check if the functions are the same by comparing their method names
        if (this.func.Method.Name != sD.func.Method.Name)
            return false;

        // Check if the arguments list is the same length
        if (this.args.Count != sD.args.Count)
            return false;

        // Compare each argument
        for (int i = 0; i < this.args.Count; i++)
        {
            if (!(this.args[i].value == sD.args[i].value))
                return false;
        }
        return true;
    }

    public override string ToString()
    {
        string funcName = func?.Method.Name ?? "null";
        string argsString = string.Join(", ", args.Select(arg => arg.value?.ToString() ?? "null"));
        return $"{funcName}({argsString})";
    }

}

public class Action
{
    protected string actionName;
    protected List<SharedVar> actionArgs = new List<SharedVar>();
    protected List<Predicate> preconditions = new List<Predicate>();
    protected List<Predicate> effects = new List<Predicate>();

    public virtual List<Predicate> InitPreconditions() => new();
    public virtual List<Predicate> InitEffects() => new();
    public virtual void Execute() { }

    public List<Predicate> GetPreconditions() => preconditions; 
    public List<Predicate> GetEffects() => effects;

    public string Print()
    {
        string[] args = new string[actionArgs.Count];
        for (int i = 0; i < actionArgs.Count; i++)
        {
            args[i] = actionArgs[i]?.Get()?.ToString() ?? "null";
        }
        return $"{actionName}({string.Join(",", args)})"; // Use string interpolation
    }

    public virtual Action Clone()
    {
        Action clone = (Action)Activator.CreateInstance(this.GetType());
        Dictionary<SharedVar, SharedVar> varMap = new Dictionary<SharedVar, SharedVar>();

        clone.actionArgs = new List<SharedVar>();
        foreach (SharedVar arg in this.actionArgs)
            clone.actionArgs.Add(varMap[arg] = arg.Clone());

        clone.preconditions = new List<Predicate>();
        foreach (Predicate pre in this.preconditions)
        {
            List<SharedVar> clonedArgs = new List<SharedVar>();
            foreach (SharedVar arg in pre.args)
            {
                if (!varMap.ContainsKey(arg)) 
                    varMap[arg] = arg.Clone();
                clonedArgs.Add(varMap[arg]);
            }
            clone.preconditions.Add(new Predicate(pre.func, clonedArgs));
        }

        clone.effects = new List<Predicate>();
        foreach (Predicate eff in this.effects)
        {
            List<SharedVar> clonedArgs = new List<SharedVar>();
            foreach (SharedVar arg in eff.args)
            {
                if (!varMap.ContainsKey(arg)) 
                    varMap[arg] = arg.Clone();
                clonedArgs.Add(varMap[arg]);
            }
            clone.effects.Add(new Predicate(eff.func, clonedArgs));
        }

        clone.actionName = this.actionName;
        return clone;
    }

    public virtual Action CreateNew() => (Action)Activator.CreateInstance(this.GetType());

    // Cool ChatGPT code probably super inefficient 
    public virtual Action Clone1()
    {
        Action clone = new Action();

        // Args
        clone.actionArgs = new List<SharedVar>();
        foreach(SharedVar arg in actionArgs) 
            clone.actionArgs.Add(arg.Clone());

        // Precnditions
        clone.preconditions = new List<Predicate>();
        foreach (Predicate arg in preconditions)
            clone.preconditions.Add(arg.Clone());

        // Effects
        clone.effects = new List<Predicate>();
        foreach (Predicate arg in effects)
            clone.effects.Add(arg.Clone());

        clone.actionName = actionName;

        return clone;

        //// Registry to keep same logical variable shared
        //Dictionary<SharedVar, SharedVar> varMap = new Dictionary<SharedVar, SharedVar>();

        //// Clone actionArgs and store in registry
        //newAction.actionArgs = this.actionArgs.Select(arg =>
        //{
        //    var cloned = arg.Clone();
        //    varMap[arg] = cloned;
        //    return cloned;
        //}).ToArray();

        //// Clone preconditions with shared vars
        //newAction.preconditions = this.preconditions
        //    .Select(p => (
        //        p.Item1,
        //        p.Item2.Select(v =>
        //        {
        //            if (!varMap.ContainsKey(v))
        //                varMap[v] = v.Clone();
        //            return varMap[v];
        //        }).ToArray()
        //    )).ToList();

        //// Clone effects with shared vars
        //newAction.effects = this.effects
        //    .Select(e => (
        //        e.Item1,
        //        e.Item2.Select(v =>
        //        {
        //            if (!varMap.ContainsKey(v))
        //                varMap[v] = v.Clone();
        //            return varMap[v];
        //        }).ToArray()
        //    )).ToList();


    }



    public bool isValid()
    {
        throw new NotImplementedException();
        //foreach (Func<bool> precond in preconditions)
        //{
        //    if (!precond())
       //         return false;
       // }
       // return true;
    }
}
