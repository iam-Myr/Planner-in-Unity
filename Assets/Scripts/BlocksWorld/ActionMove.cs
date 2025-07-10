using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Planning;

namespace BlocksWorld
{

    public class ActionMove : PlanAction
    {
        private Pointer current, to, from;

        public ActionMove()
        {
            actionName = "Move";

            this.current = new Pointer();
            this.to = new Pointer();
            this.from = new Pointer();

            actionArgs = new List<Pointer> { current, to, from };

            preconditions.AddRange(InitPreconditions());
            effects.AddRange(InitEffects());
        }

        public ActionMove(List<Pointer> args)
        {
            actionName = "Move";

            // Extract meaningful references from the list
            this.current = args[0];
            this.to = args[1];
            this.from = args[2];

            preconditions.AddRange(InitPreconditions());
            effects.AddRange(InitEffects());
        }

        public override PlanAction CreateNew(List<Pointer> args)
        {
            return new ActionMove(args);
        }

        #region Preconditions
        // Preconditions
        public override List<Predicate> InitPreconditions()
        {
            return new List<Predicate>
        {
            new Predicate(BlockDomain.isClear, new List<Pointer> {current}, true), // isClear(current)
            new Predicate(BlockDomain.isClear, new List<Pointer> {to}, true), // isClear(to)
            new Predicate(BlockDomain.isOn, new List <Pointer> {current, from}, true) // isOn(current, from)
        };
        }
        #endregion

        // Effects
        public override List<Predicate> InitEffects()
        {
            return new List<Predicate>
        {
            new Predicate(BlockDomain.isClear, new List <Pointer> {from}, true), // isClear(from)
            new Predicate(BlockDomain.isOn, new List <Pointer> {current, to}, true), // isOn(current, to)
            new Predicate(  BlockDomain.isOn, new List <Pointer> {current, from}, false),
            new Predicate(BlockDomain.isClear, new List <Pointer> {to}, false)
        };
        }

        /*public override async Task Execute(object args)
        {
            if (to.Get() is Block toBlock &&
                from.Get() is Block fromBlock &&
                current.Get() is Block currentBlock)
            {
                // Logical update
                toBlock.SetAbove(currentBlock);
                fromBlock.SetAbove(null);
                currentBlock.SetBelow(toBlock);

                // Visual update
                Vector3 newPos = toBlock.transform.position + Vector3.up * 1.1f;
                await currentBlock.MoveToAsync(newPos); // Async movement
            }
        }*/
    }
}
