using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Planning;

// How can we move the agent??
namespace SimWorld
{
    public class ActionMoveTo : PlanAction
    {
        private Pointer area;
        private Pointer from;

        public ActionMoveTo()
        {
            actionName = "MoveTo";

            this.area = new Pointer();
            this.from = new Pointer();

            actionArgs = new List<Pointer> { area, from };

            preconditions.AddRange(InitPreconditions());
            effects.AddRange(InitEffects());
        }

        public ActionMoveTo(List<Pointer> args) : base(args)
        {
            actionName = "MoveTo";

            // Extract meaningful references from the list
            this.area = args[0];
            this.from = args[1];

            preconditions.AddRange(InitPreconditions());
            effects.AddRange(InitEffects());
        }

        public override PlanAction CreateNew(List<Pointer> args)
        {
            return new ActionMoveTo(args);
        }

        #region Preconditions
        // Preconditions
        public override List<Predicate> InitPreconditions()
        {
            return new List<Predicate>
        {
            new Predicate(SimDomain.isAt, new List<Pointer> {from}, true) // isAt(from)  
        };
        }
        #endregion

        // Effects
        public override List<Predicate> InitEffects()
        {
            return new List<Predicate>
            {
               new Predicate(SimDomain.isAt, new List<Pointer> {area}, true), // isAt(area)  
               new Predicate(SimDomain.isAt, new List<Pointer> {from}, false)
            };
        }

        public override async Task Execute(object arg)
        {
            if (arg is IMoveProvider mover && area.Get() is Area target)
            {
                Vector3 destination = target.GetPosition();
                float speed = mover.GetSpeed();
                await mover.MoveTo(destination, speed);
            }
        }
    }
}
