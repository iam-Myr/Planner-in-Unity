using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SharedDelegate
{
    public Func<object[], bool> func { get; private set; } // CHECK THIS OUT
    public List<SharedVar> args { get; private set; }

    public SharedDelegate()
    {
    }

    public SharedDelegate(Func<object[], bool> func, List<SharedVar> args)
    {
        this.func = func;
        this.args = args;
    }

    internal SharedDelegate Clone()
    {
        SharedDelegate clone = new SharedDelegate();
        clone.func = func;

        // Args
        clone.args = new List<SharedVar>();
        foreach (SharedVar arg in args)
            clone.args.Add(arg.Clone());

        return clone;
    }
}

public class Action
{
    protected string actionName;
    protected List<SharedVar> actionArgs = new List<SharedVar>();
    protected List<SharedDelegate> preconditions = new List<SharedDelegate>();
    protected List<SharedDelegate> effects = new List<SharedDelegate>();

    public virtual List<SharedDelegate> InitPreconditions() => new();
    public virtual List<SharedDelegate> InitEffects() => new();
    public virtual void Execute() { }

    public List<SharedDelegate> GetPreconditions() => preconditions; 
    public List<SharedDelegate> GetEffects() => effects;

    public void Print()
    {
        string[] args = new string[actionArgs.Count];
        for (int i = 0; i < actionArgs.Count; i++)
        {
            args[i] = actionArgs[i]?.Get()?.ToString() ?? "null";
        }
        Debug.Log($"{actionName}({string.Join(",", args)})");
    }
    public virtual Action Clone()
    {
        Action clone = (Action)Activator.CreateInstance(this.GetType());
        Dictionary<SharedVar, SharedVar> varMap = new Dictionary<SharedVar, SharedVar>();

        clone.actionArgs = new List<SharedVar>();
        foreach (SharedVar arg in this.actionArgs)
            clone.actionArgs.Add(varMap[arg] = arg.Clone());

        clone.preconditions = new List<SharedDelegate>();
        foreach (SharedDelegate pre in this.preconditions)
        {
            List<SharedVar> clonedArgs = new List<SharedVar>();
            foreach (SharedVar arg in pre.args)
            {
                if (!varMap.ContainsKey(arg)) 
                    varMap[arg] = arg.Clone();
                clonedArgs.Add(varMap[arg]);
            }
            clone.preconditions.Add(new SharedDelegate(pre.func, clonedArgs));
        }

        clone.effects = new List<SharedDelegate>();
        foreach (SharedDelegate eff in this.effects)
        {
            List<SharedVar> clonedArgs = new List<SharedVar>();
            foreach (SharedVar arg in eff.args)
            {
                if (!varMap.ContainsKey(arg)) 
                    varMap[arg] = arg.Clone();
                clonedArgs.Add(varMap[arg]);
            }
            clone.effects.Add(new SharedDelegate(eff.func, clonedArgs));
        }

        clone.actionName = this.actionName;
        return clone;
    }


    // Cool ChatGPT code probably super inefficient 
    public virtual Action Clone1()
    {
        Action clone = new Action();

        // Args
        clone.actionArgs = new List<SharedVar>();
        foreach(SharedVar arg in actionArgs) 
            clone.actionArgs.Add(arg.Clone());

        // Precnditions
        clone.preconditions = new List<SharedDelegate>();
        foreach (SharedDelegate arg in preconditions)
            clone.preconditions.Add(arg.Clone());

        // Effects
        clone.effects = new List<SharedDelegate>();
        foreach (SharedDelegate arg in effects)
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
