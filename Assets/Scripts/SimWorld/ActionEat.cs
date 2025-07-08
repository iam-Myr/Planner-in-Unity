using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Planning;

namespace SimWorld
{
    public class ActionEat : PlanAction
    {
        Func<List<object>, bool> isAt, isHungry;

        public ActionEat(Func<List<object>, bool> isAt, Func<List<object>, bool> isHungry)
        {
            this.isAt = isAt;
            this.isHungry = isHungry;
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
                new Predicate(isAt, new List<Pointer> {SimDomain.FoodArea}, true), // isAt(sleep)  
                new Predicate(isHungry, new List<Pointer> {}, true)
            };
        }
        #endregion

        // Effects
        public override List<Predicate> InitEffects()
        {
            return new List<Predicate>
            {
                new Predicate(isHungry, new List<Pointer> {}, false) //not isSleepy
            };
        }
    }
}
