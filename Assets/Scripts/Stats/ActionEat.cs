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

    public override PlanAction CreateNew(List<Pointer> args)
    {
        return new ActionEat();
    }

    #region Preconditions
    // Preconditions
    public override List<Predicate> InitPreconditions()
    {
        return new List<Predicate>
        {
            new Predicate(Domain.isAt, new List<Pointer> {Domain.Food}, true), // isAt(food)  
            new Predicate(Domain.isHungry, new List<Pointer> {}, true)
        };
    }
    #endregion

    // Effects
    public override List<Predicate> InitEffects()
    {
        return new List<Predicate>
        {
           new Predicate(Domain.isHungry, new List<Pointer> {}, false) //not isHungry
        };
    }

    public override async Task Execute(Agent agent)
    {
        // Logical update
        Debug.Log("Eating..!");
        Sim sim = agent.GetComponent<Sim>();
        sim.Eat();
    }
}
