using System;
using System.Collections.Generic;
using UnityEngine;

public class ActionMove : Action
{
    private Block current;
    private Block from;
    private Block to;

    public ActionMove(Block current, Block from, Block to)
    {
        this.current = current;
        this.from = from;
        this.to = to;
    }

    private void Start()
    {
        preconditions.AddRange(GetPreconditions());
    }

    public override List<Func<bool>> GetPreconditions()
    {
        return new List<Func<bool>>
        {
            () => isOn(current, from),
            () => isClear(to),
            () => isClear(current)
        };
    }


    // PRECONDITIONS & Effects
    public bool isClear(Block x)
    {
        return x.isClear();
    }

    public bool isOn(Block x, Block y)
    {
        return x.GetBelow() == y && y.GetAbove() == x;
    }

    public override void Execute()
    {
        // Do positions but for now just logic
        to.SetAbove(current);
        from.SetAbove(null);
        current.SetBelow(to);
    }
}
