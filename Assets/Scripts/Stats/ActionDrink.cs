using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class ActionDrink : PlanAction
{
    public ActionDrink()
    {
        actionName = "Drink";

        preconditions.AddRange(InitPreconditions());
        effects.AddRange(InitEffects());
    }

    public override PlanAction CreateNew(List<object> args)
    {
        return new ActionDrink();
    }

    #region Preconditions
    // Preconditions
    public override List<Predicate> InitPreconditions()
    {
        return new List<Predicate>
        {
            new Predicate(SimDomain.isAt, new List<object> {SimDomain.Water}, true), // isAt(food)  
            new Predicate(SimDomain.isThirsty, new List<object> {}, true)
        };
    }
    #endregion

    // Effects
    public override List<Predicate> InitEffects()
    {
        return new List<Predicate>
        {
           new Predicate(SimDomain.isThirsty, new List<object> {}, false) //not isHungry
        };
    }

    public override async Task Execute(Agent agent)
    {
        // Logical update
        //Debug.Log("Drinking!!!");
        if (agent is SimAgent sim) sim.Drink();
    }
}
