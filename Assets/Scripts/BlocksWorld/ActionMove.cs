using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class ActionMove : Action
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


    #region Preconditions
    // Preconditions
    public override List<Predicate> InitPreconditions()
    {
        return new List<Predicate>
        {
            new Predicate(PredicateLibrary.isClear, new List<Pointer> {current}), // isClear(current)
            new Predicate(PredicateLibrary.isClear, new List<Pointer> {to}), // isClear(to)
            new Predicate(PredicateLibrary.isOn, new List < Pointer > {current, from}) // isOn(current, from)
        };
    }
    #endregion

    // Effects
    public override List<Predicate> InitEffects()
    {
        return new List<Predicate>
        {
            new Predicate(PredicateLibrary.isClear, new List <Pointer> {from}), // isClear(from)
            new Predicate(PredicateLibrary.isOn, new List <Pointer> {current, to}) // isOn(current, to)
        };
    }


    public override async Task Execute()
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
    }
}
