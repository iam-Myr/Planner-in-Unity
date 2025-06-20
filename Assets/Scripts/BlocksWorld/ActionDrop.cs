using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class ActionDrop : PlanAction
{
    private object current, to;

    public ActionDrop()
    {
        actionName = "Drop";

        this.current = new object();
        this.to = new object();

        actionArgs = new List<object> { current, to};

        preconditions.AddRange(InitPreconditions());
        effects.AddRange(InitEffects());
    }

    public ActionDrop(List<object> args) : base(args)
    {
        actionName = "Drop";

        // Extract meaningful references from the list
        this.current = args[0];
        this.to = args[1];

        preconditions.AddRange(InitPreconditions());
        effects.AddRange(InitEffects());
    }

    public override PlanAction CreateNew(List<object> args)
    {
        return new ActionDrop(args);
    }


    #region Preconditions
    // Preconditions
    public override List<Predicate> InitPreconditions()
    {
        return new List<Predicate>
        {
            new Predicate(BlockDomain.isHolding, new List<object> {current}, true), // isClear(current)
            new Predicate(BlockDomain.isClear, new List<object> {to}, true), // isClear(to)
        };
    }
    #endregion

    // Effects
    public override List<Predicate> InitEffects()
    {
        return new List<Predicate>
        {
            new Predicate(BlockDomain.isOn, new List <object> {current, to}, true), // isOn(current, to)
            new Predicate(BlockDomain.isHolding, new List<object> {current}, false),
            new Predicate(BlockDomain.isHandEmpty, new List < object > {}, true),
            new Predicate(BlockDomain.isClear, new List<object> {to}, false), // isClear(to)
        };
    }

    public override async Task Execute(Agent agent)
    {
        if (to is Block toBlock &&
            current is Block currentBlock)
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
