using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Planning;

namespace SimWorld
{
    public class ActionDrink : PlanAction
    {
        Func<List<object>, bool> isAt, isThirsty;

        public ActionDrink(Func<List<object>, bool> isAt, Func<List<object>, bool> isThirsty)
        {
            this.isAt = isAt;
            this.isThirsty = isThirsty;
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
                new Predicate(isAt, new List<Pointer> {SimDomain.WaterArea}, true), // isAt(sleep)  
                new Predicate(isThirsty, new List<Pointer> {}, true)
            };
        }
        #endregion

        // Effects
        public override List<Predicate> InitEffects()
        {
            return new List<Predicate>
            {
                new Predicate(isThirsty, new List<Pointer> {}, false) //not isSleepy
            };
        }
    }
}
