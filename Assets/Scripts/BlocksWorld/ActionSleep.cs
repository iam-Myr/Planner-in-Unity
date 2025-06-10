using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

// How can we raise the sleep stat from here?
public class ActionSleep : Action
{
    public ActionSleep()
    {
        actionName = "Sleep";

        preconditions.AddRange(InitPreconditions());
        effects.AddRange(InitEffects());
    }

    public override Action CreateNew(List<Pointer> args)
    {
        return new ActionSleep();
    }

    #region Preconditions
    // Preconditions
    public override List<Predicate> InitPreconditions()
    {
        return new List<Predicate>
        {
            new Predicate(Domain.isAt, new List<Pointer> {Problem.Sleep}, true), // isAt(sleep)  
            new Predicate(Domain.isSleepy, new List<Pointer> {}, true)
        };
    }
    #endregion

    // Effects
    public override List<Predicate> InitEffects()
    {
        return new List<Predicate>
        {
           new Predicate(Domain.isSleepy, new List<Pointer> {}, false) //not isSleepy
        };
    }

    public override async Task Execute()
    {
        // Logical update
        Debug.Log("Sleeping...");
        
    }
}
