using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Planning;

namespace SimWorld
{
    public class ActionSleep : PlanAction
    {

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
                SimDomain.isAt.CreateNew(new List<Pointer> {SimDomain.SleepArea}, true),
                //new Predicate(isAt, new List<Pointer> {SimDomain.SleepArea}, true), // isAt(sleep)  
                SimDomain.isSleepy.CreateNew(new List<Pointer> {}, true)
                //new Predicate(isSleepy, new List<Pointer> {}, true) 
            };
        }
        #endregion

        // Effects
        public override List<Predicate> InitEffects()
        {
            return new List<Predicate>
            {
                SimDomain.isSleepy.CreateNew(new List<Pointer> {}, false) //not isSleepy
            };
        }
    }
}
