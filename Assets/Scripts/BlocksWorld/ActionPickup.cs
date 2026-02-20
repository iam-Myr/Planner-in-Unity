using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class ActionPickup : PlanAction
{
    private object current, from;

    public ActionPickup()
    {
        actionName = "Pick Up";

        this.current = new object();
        this.from = new object();

        actionArgs = new List<object> { current, from };

        preconditions.AddRange(InitPreconditions());
        effects.AddRange(InitEffects());
    }

    public ActionPickup(List<object> args) : base(args)
    {
        actionName = "Pickup";

        // Extract meaningful references from the list
        this.current = args[0];
        this.from = args[1];

        preconditions.AddRange(InitPreconditions());
        effects.AddRange(InitEffects());
    }

    public override PlanAction CreateNew(List<object> args)
    {
        return new ActionPickup(args);
    }

    #region Preconditions
    // Preconditions
    public override List<Predicate> InitPreconditions()
    {
        return new List<Predicate>
        {
            new Predicate(BlockDomain.isClear, new List<object> {current}, true), // isClear(current)
            new Predicate(BlockDomain.isOn, new List<object> {current, from}, true), // isOn(current, from)
            new Predicate(BlockDomain.isHandEmpty, new List <object> {}, true) 
        };
    }
    #endregion

<<<<<<< Updated upstream
    // Effects
    public override List<Predicate> InitEffects()
    {
        return new List<Predicate>
=======
            actionArgs.Add(current);
            actionArgs.Add(from);

            // Add constraint: the block we pick up must be different from the block underneath
            AddConstraint(args => AllDifferent(args));
        }

        public override PlanAction CreateNew(List<Pointer> args)
>>>>>>> Stashed changes
        {
            new Predicate(BlockDomain.isHolding, new List <object   > {current}, true), // isClear(from)
            new Predicate(BlockDomain.isClear, new List <object> {from}, true), // isClear(from)
            new Predicate(BlockDomain.isHandEmpty, new List <object> {}, false)
        };
    }


    public override async Task Execute(Agent agent)
    {
        if (from is Block fromBlock &&
            current is Block currentBlock)
        {
            // Logical update
            
            fromBlock.SetAbove(null);
            currentBlock.SetBelow(null);

            // Visual update
            Vector3 newPos = currentBlock.transform.position + Vector3.up * 1.1f;
            await currentBlock.MoveToAsync(newPos); // Async movement
        }
    }
}
