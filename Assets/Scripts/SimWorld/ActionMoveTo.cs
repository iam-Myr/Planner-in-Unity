using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Planning;

namespace SimWorld
{
    public class ActionMoveTo : PlanAction
    {
        private Pointer to = new Pointer();
        private Pointer from = new Pointer();

        Func<List<object>, bool> isAt;

        public ActionMoveTo(Func<List<object>, bool> isAt)
        {
            this.isAt = isAt;
        }

        public override PlanAction CreateNew(List<Pointer> args) //[A, B]
        {
            ActionMoveTo a = new ActionMoveTo(isAt);
            // Extract meaningful references from the list
            this.to.value = args[0].value;
            this.from.value = args[1].value;
            return a;
        }

        #region Preconditions
        // Preconditions
        public override List<Predicate> InitPreconditions()
        {
            return new List<Predicate>
            {
                new Predicate(isAt, new List<Pointer> {from}, true) // isAt(from)  
            };
        }
        #endregion

        // Effects
        public override List<Predicate> InitEffects()
        {
            return new List<Predicate>
            {
               new Predicate(isAt, new List<Pointer> {to}, true), // isAt(area)  
               new Predicate(isAt, new List<Pointer> {from}, false)
            };
        }
    }
}
