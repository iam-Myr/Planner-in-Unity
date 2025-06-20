using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class ActionSleep : PlanAction
{
    public ActionSleep()
    {
        actionName = "Sleep";

        preconditions.AddRange(InitPreconditions());
        effects.AddRange(InitEffects());
    }

    public override PlanAction CreateNew(List<object> args)
    {
        return new ActionSleep();
    }

    #region Preconditions
    // Preconditions
    public override List<Predicate> InitPreconditions()
    {
        return new List<Predicate>
        {
            new Predicate(SimDomain.isAt, new List<object> {SimDomain.Sleep}, true), // isAt(sleep)  
            new Predicate(SimDomain.isSleepy, new List<object> {}, true)
        };
    }
    #endregion

    // Effects
    public override List<Predicate> InitEffects()
    {
        return new List<Predicate>
        {
           new Predicate(SimDomain.isSleepy, new List<object> {}, false) //not isSleepy
        };
    }

    public override async Task Execute(Agent agent)
    {
        // Logical update
        //Debug.Log("Sleeping...");

        if (agent is SimAgent sim) sim.Sleep();
    }
}
