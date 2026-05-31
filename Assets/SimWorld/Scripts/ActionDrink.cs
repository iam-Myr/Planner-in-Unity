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
        }
        public override PlanAction CreateNew(List<Pointer> args)
        {
            var a = new ActionDrink();
            a.actionArgs = new List<Pointer>(args);
            a.AddExecutable(this.executable, this.durationEstimate);
            return a;
        }


        #region Preconditions
        // Preconditions
        public override List<Predicate> InitPreconditions()
        {
            return new List<Predicate>
            {
                SimDomain.isAt.Instantiate(new List <Pointer> {SimDomain.WaterArea}, true), // isAt(sleep)  
                SimDomain.isThirsty.Instantiate(new List <Pointer> { }, true)
            };
        }
        #endregion

        // Effects
        public override List<Predicate> InitEffects()
        {
            return new List<Predicate>
            {
                SimDomain.isThirsty.Instantiate(new List <Pointer> { }, false) //not isSleepy
            };
        }
    }
}
