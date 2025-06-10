using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class ActionPickup : Action
{
    private Pointer current, from;

    public ActionPickup()
    {
        actionName = "Pick Up";

        this.current = new Pointer();
        this.from = new Pointer();

        actionArgs = new List<Pointer> { current, from };

        preconditions.AddRange(InitPreconditions());
        effects.AddRange(InitEffects());
    }

    public ActionPickup(List<Pointer> args) : base(args)
    {
        actionName = "Pickup";

        // Extract meaningful references from the list
        this.current = args[0];
        this.from = args[1];

        preconditions.AddRange(InitPreconditions());
        effects.AddRange(InitEffects());
    }

    public override Action CreateNew(List<Pointer> args)
    {
        return new ActionPickup(args);
    }

    #region Preconditions
    // Preconditions
    public override List<Predicate> InitPreconditions()
    {
        return new List<Predicate>
        {
            new Predicate(BlockDomain.isClear, new List<Pointer> {current}, true), // isClear(current)
            new Predicate(BlockDomain.isOn, new List<Pointer> {current, from}, true), // isOn(current, from)
            new Predicate(BlockDomain.isHandEmpty, new List <Pointer> {}, true) 
        };
    }
    #endregion

    // Effects
    public override List<Predicate> InitEffects()
    {
        return new List<Predicate>
        {
            new Predicate(BlockDomain.isHolding, new List <Pointer> {current}, true), // isClear(from)
            new Predicate(BlockDomain.isClear, new List <Pointer> {from}, true), // isClear(from)
            new Predicate(BlockDomain.isHandEmpty, new List <Pointer> {}, false)
        };
    }


    public override async Task Execute()
    {
        if (from.Get() is Block fromBlock &&
            current.Get() is Block currentBlock)
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
