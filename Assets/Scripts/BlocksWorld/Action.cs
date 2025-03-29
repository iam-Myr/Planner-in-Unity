using System;
using System.Collections.Generic;
using UnityEngine;

// TURN INTO INTERFACE
public abstract class Action: MonoBehaviour
{
    protected List<Func<bool>> preconditions = new List<Func<bool>>();
    public abstract List<Func<bool>> GetPreconditions();
    public abstract void Execute();

    public bool isValid()
    {
        foreach (Func<bool> precond in preconditions)
        {
            if (!precond())
                return false;
        }
        return true;
    }
}
