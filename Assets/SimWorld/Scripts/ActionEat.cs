using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Planning;

namespace SimWorld
{
    public class ActionEat : PlanAction
    {
        
        public ActionEat() 
        {
            actionName = "Eat";
        }

        public override PlanAction CreateNew(List<Pointer> args)
        {
            var a = new ActionEat();
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
                 SimDomain.isAt.Instantiate(new List<Pointer> {SimDomain.FoodArea}, true),
                 SimDomain.isHungry.Instantiate(new List<Pointer> {}, true),
                //new Predicate(isAt, new List<Pointer> {SimDomain.FoodArea}, true), // isAt(sleep)  
                //new Predicate(isHungry, new List<Pointer> {}, true)
            };
        }
        #endregion

        // Effects
        public override List<Predicate> InitEffects()
        {
            return new List<Predicate>
            {
                SimDomain.isHungry.Instantiate(new List<Pointer> {}, false)
            };
        }
    }
}
