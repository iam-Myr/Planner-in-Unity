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
        
        public ActionMoveTo()
        {
            actionName = "MoveTo";
            actionArgs.Add(to);
            actionArgs.Add(from);
        }


        public override PlanAction CreateNew(List<Pointer> args) //[A, B]
        {
            ActionMoveTo a = new ActionMoveTo();
            // Extract meaningful references from the list
            a.to.value = args[0].value;
            a.from.value = args[1].value;
            a.AddExecutable(this.executable);
            return a;
        }

        #region Preconditions
        // Preconditions
        public override List<Predicate> InitPreconditions()
        {
            return new List<Predicate>
            {
                SimDomain.isAt.Instantiate(new List<Pointer> {from}, true)// isAt(from)  
            };
        }
        #endregion

        // Effects
        public override List<Predicate> InitEffects()
        {
            return new List<Predicate>
            {
               SimDomain.isAt.Instantiate(new List <Pointer> {to}, true), // isAt(area)  
               SimDomain.isAt.Instantiate(new List<Pointer> {from}, false)
            };
        }
    }
}
