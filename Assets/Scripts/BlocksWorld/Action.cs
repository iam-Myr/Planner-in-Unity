using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class Action: MonoBehaviour
{
    protected String actionName;
    protected SharedVar[] actionArgs;
    protected List<(Func<object[], bool>, object[])> preconditions = new List<(Func<object[], bool>, object[])>();
    protected List<(Func<object[], bool>, object[])> effects = new List<(Func<object[], bool>, object[])>();

    public abstract List<(Func<object[], bool>, object[])> InitPreconditions();
    public abstract List<(Func<object[], bool>, object[])> InitEffects();
    public abstract void Execute();

    public List<(Func<object[], bool>, object[])> GetPreconditions() { return preconditions; }
    public List<(Func<object[], bool>, object[])> GetEffects() { return effects; }

    public void Print()
    {
        string[] args = new string[actionArgs.Length];
        for (int i = 0; i < actionArgs.Length; i++)
        {
            args[i] = actionArgs[i]?.Get()?.ToString() ?? "null";
        }
        print($"{actionName}({string.Join(",", args)})");
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
