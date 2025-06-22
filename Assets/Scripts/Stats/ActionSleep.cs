using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Planning;

namespace SimWorld
{
    public class ActionSleep : PlanAction
    {
        public ActionSleep()
        {
            actionName = "Sleep";

            preconditions.AddRange(InitPreconditions());
            effects.AddRange(InitEffects());
        }

        public override PlanAction CreateNew(List<Pointer> args)
        {
            return new ActionSleep();
        }

        #region Preconditions
        // Preconditions
        public override List<Predicate> InitPreconditions()
        {
            return new List<Predicate>
        {
            new Predicate(SimDomain.isAt, new List<Pointer> {SimDomain.Sleep}, true), // isAt(sleep)  
            new Predicate(SimDomain.isSleepy, new List<Pointer> {}, true)
        };
        }
        #endregion

        // Effects
        public override List<Predicate> InitEffects()
        {
            return new List<Predicate>
        {
           new Predicate(SimDomain.isSleepy, new List<Pointer> {}, false) //not isSleepy
        };
        }

        public override async Task Execute(Agent agent)
        {
            // Logical update
            //Debug.Log("Sleeping...");

            if (agent is SimAgent sim) sim.Sleep();
        }
    }
}
