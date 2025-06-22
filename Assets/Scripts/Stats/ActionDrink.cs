using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Planning;


namespace SimWorld
{
    public class ActionDrink : PlanAction
    {
        public ActionDrink()
        {
            actionName = "Drink";

            preconditions.AddRange(InitPreconditions());
            effects.AddRange(InitEffects());
        }

        public override PlanAction CreateNew(List<Pointer> args)
        {
            return new ActionDrink();
        }

        #region Preconditions
        // Preconditions
        public override List<Predicate> InitPreconditions()
        {
            return new List<Predicate>
        {
            new Predicate(SimDomain.isAt, new List<Pointer> {SimDomain.Water}, true), // isAt(food)  
            new Predicate(SimDomain.isThirsty, new List<Pointer> {}, true)
        };
        }
        #endregion

        // Effects
        public override List<Predicate> InitEffects()
        {
            return new List<Predicate>
        {
           new Predicate(SimDomain.isThirsty, new List<Pointer> {}, false) //not isHungry
        };
        }

        public override async Task Execute(Agent agent)
        {
            // Logical update
            //Debug.Log("Drinking!!!");
            if (agent is SimAgent sim) sim.Drink();
        }
    }
}
