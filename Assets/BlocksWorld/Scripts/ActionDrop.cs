using System;
using System.Collections.Generic;
using Planning;

namespace BlocksWorld
{
    public class ActionDrop : PlanAction
    {
        private Pointer current = new Pointer(typeof(Block)); // block to drop
        private Pointer to = new Pointer(typeof(Block));      // block to drop onto

        public ActionDrop()
        {
            actionName = "Drop";

            actionArgs.Add(current);
            actionArgs.Add(to);
        }

        public override PlanAction CreateNew(List<Pointer> args) //[current, to]
        {
            ActionDrop a = new ActionDrop();

            // Set values from arguments
            a.current.value = args[0].value;
            a.to.value = args[1].value;

            // Copy the executable and duration if already set
            a.AddExecutable(this.executable, this.durationEstimate);

            return a;
        }

        #region Preconditions
        public override List<Predicate> InitPreconditions()
        {
            return new List<Predicate>
            {
                BlockDomain.isHolding.Instantiate(new List<Pointer> { current }, true), // agent is holding the block
                BlockDomain.isClear.Instantiate(new List<Pointer> { to }, true)        // target block is clear
            };
        }
        #endregion

        #region Effects
        public override List<Predicate> InitEffects()
        {
            return new List<Predicate>
            {
                BlockDomain.isOn.Instantiate(new List<Pointer> { current, to }, true),     // current is now on target
                BlockDomain.isHolding.Instantiate(new List<Pointer> { current }, false),   // agent no longer holding
                BlockDomain.isHandEmpty.Instantiate(new List<Pointer> { }, true),         // agent hand empty
                BlockDomain.isClear.Instantiate(new List<Pointer> { to }, false)           // target is no longer clear
            };
        }
        #endregion
    }
}
