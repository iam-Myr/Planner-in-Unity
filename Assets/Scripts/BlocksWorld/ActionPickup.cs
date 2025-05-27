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


    #region Preconditions
    // Preconditions
    public override List<Predicate> InitPreconditions()
    {
        return new List<Predicate>
        {
            new Predicate(PredicateLibrary.isClear, new List<Pointer> {current}, true), // isClear(current)
            new Predicate(PredicateLibrary.isOn, new List<Pointer> {current, from}, true), // isOn(current, from)
            new Predicate(PredicateLibrary.isHandEmpty, new List < Pointer > {}, true) 
        };
    }
    #endregion

    // Effects
    public override List<Predicate> InitEffects()
    {
        return new List<Predicate>
        {
            new Predicate(PredicateLibrary.isHolding, new List <Pointer> {current}, true), // isClear(from)
            new Predicate(PredicateLibrary.isClear, new List <Pointer> {from}, true), // isClear(from)
            new Predicate(PredicateLibrary.isHandEmpty, new List < Pointer > {}, false)
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
