using System;
using System.Collections.Generic;
using UnityEngine;

public class ActionMove : Action
{
    private SharedVar current;
    private SharedVar to;
    private SharedVar from;

    public ActionMove()
    {
        actionName = "Move";

        current = new SharedVar();
        to = new SharedVar();
        from = new SharedVar();

        actionArgs = new SharedVar[] {current, to, from};
    }

    private void Awake()
    {
        preconditions.AddRange(InitPreconditions());
        effects.AddRange(InitEffects());
    }

    public override List<(Func<object[], bool>, object[])> InitPreconditions()
    {
        return new List<(Func<object[], bool>, object[])>
        {
            (PredicateLibrary.isClear, new object[] {current}),
            (PredicateLibrary.isClear, new object[] {to}),
            (PredicateLibrary.isOn, new object[] {current, from})
        };
    }

    public override List<(Func<object[], bool>, object[])> InitEffects()
    {
        return new List<(Func<object[], bool>, object[])>
        {
            (PredicateLibrary.isClear, new object[] {from}),
            (PredicateLibrary.isOn, new object[] {current, to})
        };
    }


    public override void Execute()
    {
        // Do positions but for now just logic
        //to.SetAbove(current);
        //from.SetAbove(null);
        //current.SetBelow(to);
    }
}
