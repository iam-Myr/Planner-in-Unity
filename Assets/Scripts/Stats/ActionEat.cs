using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class ActionEat : PlanAction
{
    public ActionEat()
    {
        actionName = "Eat";

        preconditions.AddRange(InitPreconditions());
        effects.AddRange(InitEffects());
    }

    public override PlanAction CreateNew(List<object> args)
    {
        return new ActionEat();
    }

    #region Preconditions
    // Preconditions
    public override List<Predicate> InitPreconditions()
    {
        return new List<Predicate>
        {
            new Predicate(SimDomain.isAt, new List<object> {SimDomain.Food}, true), // isAt(food)  
            new Predicate(SimDomain.isHungry, new List<object> {}, true)
        };
    }
    #endregion

    // Effects
    public override List<Predicate> InitEffects()
    {
        return new List<Predicate>
        {
           new Predicate(SimDomain.isHungry, new List<object> {}, false) //not isHungry
        };
    }

    public override async Task Execute(Agent agent)
    {
        // Logical update
        //Debug.Log("Eating..!");
        if (agent is SimAgent sim) sim.Eat();
    }
}
