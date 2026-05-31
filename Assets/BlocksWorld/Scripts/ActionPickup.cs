using System;
using System.Collections.Generic;
using UnityEngine;
using Planning;

namespace BlocksWorld
{
    public class ActionPickup : PlanAction
    {
        private Pointer current = new Pointer(typeof(Block)); // Block to pick up
        private Pointer from = new Pointer(typeof(Block));    // Block underneath

        public ActionPickup()
        {
            actionName = "Pickup";

            actionArgs.Add(current);
            actionArgs.Add(from);

            // Add constraint: the block we pick up must be different from the block underneath
            AddConstraint(Constraints.AllDifferent(current, from));
        }

        public override PlanAction CreateNew(List<Pointer> args)
        {
            ActionPickup a = new ActionPickup();

            // Set values from the passed pointers
            a.current.value = args[0].value;
            a.from.value = args[1].value;

            // Copy the executable and duration if set
            a.AddExecutable(this.executable, this.durationEstimate);

            return a;
        }

        #region Preconditions
        public override List<Predicate> InitPreconditions()
        {
            return new List<Predicate>
            {
                BlockDomain.isClear.Instantiate(new List<Pointer> { current }, true),       // block is clear
                BlockDomain.isOn.Instantiate(new List<Pointer> { current, from }, true),    // block is on 'from'
                BlockDomain.isHandEmpty.Instantiate(new List<Pointer> { }, true)            // hand is empty
            };
        }
        #endregion

        #region Effects
        public override List<Predicate> InitEffects()
        {
            return new List<Predicate>
            {
                BlockDomain.isHolding.Instantiate(new List<Pointer> { current }, true),     // now holding the block
                BlockDomain.isClear.Instantiate(new List<Pointer> { from }, true),         // from is now clear
                BlockDomain.isHandEmpty.Instantiate(new List<Pointer> { }, false),          // hand is no longer empty
                //BlockDomain.isOn.Instantiate(new List<Pointer> { current, from }, false) // current is no longer on from
            };
        }
        #endregion
    }
}
