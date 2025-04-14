using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class ActionMove : Action
{
    private SharedVar current, to, from;

    public ActionMove()
    {
        actionName = "Move";

        current = new SharedVar();
        to = new SharedVar();
        from = new SharedVar();

        actionArgs = new List<SharedVar> { current, to, from };

        preconditions.AddRange(InitPreconditions());
        effects.AddRange(InitEffects());
    }

    #region Preconditions
    // Preconditions
    public override List<Predicate> InitPreconditions()
    {
        return new List<Predicate>
        {
            new Predicate(PredicateLibrary.isClear, new List<SharedVar> {current}), // isClear(current)
            new Predicate(PredicateLibrary.isClear, new List<SharedVar> {to}), // isClear(to)
            new Predicate(PredicateLibrary.isOn, new List < SharedVar > {current, from}) // isOn(current, from)
        };
    }
    #endregion

    // Effects
    public override List<Predicate> InitEffects()
    {
        return new List<Predicate>
        {
            new Predicate(PredicateLibrary.isClear, new List <SharedVar> {from}), // isClear(from)
            new Predicate(PredicateLibrary.isOn, new List <SharedVar> {current, to}) // isOn(current, to)
        };
    }

    public override async Task Execute()
    {
        if (to.value is Block toBlock &&
            from.value is Block fromBlock &&
            current.value is Block currentBlock)
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
