using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class Action: MonoBehaviour
{
    protected String actionName;
    protected SharedVar[] actionArgs;
    protected List<(Func<object[], bool>, SharedVar[])> preconditions = new List<(Func<object[], bool>, SharedVar[])>();
    protected List<(Func<object[], bool>, SharedVar[])> effects = new List<(Func<object[], bool>, SharedVar[])>();

    public abstract List<(Func<object[], bool>, SharedVar[])> InitPreconditions();
    public abstract List<(Func<object[], bool>, SharedVar[])> InitEffects();
    public abstract void Execute();

    public List<(Func<object[], bool>, SharedVar[])> GetPreconditions() { return preconditions; }
    public List<(Func<object[], bool>, SharedVar[])> GetEffects() { return effects; }

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
