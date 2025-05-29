using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class ActionDrop : Action
{
    private Pointer current, to;

    public ActionDrop()
    {
        actionName = "Drop";

        this.current = new Pointer();
        this.to = new Pointer();

        actionArgs = new List<Pointer> { current, to};

        preconditions.AddRange(InitPreconditions());
        effects.AddRange(InitEffects());
    }

    public ActionDrop(List<Pointer> args) : base(args)
    {
        actionName = "Drop";

        // Extract meaningful references from the list
        this.current = args[0];
        this.to = args[1];

        preconditions.AddRange(InitPreconditions());
        effects.AddRange(InitEffects());
    }

    public override Action CreateNew(List<Pointer> args)
    {
        return new ActionDrop(args);
    }


    #region Preconditions
    // Preconditions
    public override List<Predicate> InitPreconditions()
    {
        return new List<Predicate>
        {
            new Predicate(PredicateLibrary.isHolding, new List<Pointer> {current}, true), // isClear(current)
            new Predicate(PredicateLibrary.isClear, new List<Pointer> {to}, true), // isClear(to)
        };
    }
    #endregion

    // Effects
    public override List<Predicate> InitEffects()
    {
        return new List<Predicate>
        {
            new Predicate(PredicateLibrary.isOn, new List <Pointer> {current, to}, true), // isOn(current, to)
            new Predicate(PredicateLibrary.isHolding, new List<Pointer> {current}, false),
            new Predicate(PredicateLibrary.isHandEmpty, new List < Pointer > {}, true)
        };
    }

    public override async Task Execute()
    {
        if (to.Get() is Block toBlock &&
            current.Get() is Block currentBlock)
        {
            // Logical update
            toBlock.SetAbove(currentBlock);
            currentBlock.SetBelow(toBlock);

            // Visual update
            Vector3 newPos = toBlock.transform.position + Vector3.up * 1.1f;
            await currentBlock.MoveToAsync(newPos); // Async movement
        }
    }
}
