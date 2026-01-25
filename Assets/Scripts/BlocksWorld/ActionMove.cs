using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Planning;

namespace BlocksWorld
{
    public class ActionMove : PlanAction
    {
        private Pointer current = new Pointer(typeof(Block)); // Block to move
        private Pointer from = new Pointer(typeof(Block));    // Block below
        private Pointer to = new Pointer(typeof(Block));      // Block to move onto

        public ActionMove()
        {
            actionName = "Move";

            actionArgs.Add(current);
            actionArgs.Add(to);
            actionArgs.Add(from);
        }

        public override PlanAction CreateNew(List<Pointer> args)
        {
            ActionMove a = new ActionMove();

            a.current.value = args[0].value;
            a.to.value = args[1].value;
            a.from.value = args[2].value;

            // Copy executable and duration if set
            a.AddExecutable(this.executable, this.durationEstimate);

            return a;
        }

        #region Preconditions
        public override List<Predicate> InitPreconditions()
        {
            return new List<Predicate>
            {
                BlockDomain.isClear.Instantiate(new List<Pointer> { current }, true),  // block is clear
                BlockDomain.isClear.Instantiate(new List<Pointer> { to }, true),       // target is clear
                BlockDomain.isOn.Instantiate(new List<Pointer> { current, from }, true) // block is on 'from'
            };
        }
        #endregion

        #region Effects
        public override List<Predicate> InitEffects()
        {
            return new List<Predicate>
            {
                BlockDomain.isOn.Instantiate(new List<Pointer> { current, to }, true),    // now on 'to'
                BlockDomain.isClear.Instantiate(new List<Pointer> { from }, true),        // from is clear
                BlockDomain.isOn.Instantiate(new List<Pointer> { current, from }, false), // no longer on 'from'
                BlockDomain.isClear.Instantiate(new List<Pointer> { to }, false)          // to is not clear anymore
            };
        }
        #endregion
    }
}
