using System;
using System.Collections.Generic;
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

    // Preconditions
    public override List<SharedDelegate> InitPreconditions()
    {
        return new List<SharedDelegate>
        {
            new SharedDelegate(PredicateLibrary.isClear, new List<SharedVar> {current}), // isClear(current)
            new SharedDelegate(PredicateLibrary.isClear, new List<SharedVar> {to}), // isClear(to)
            new SharedDelegate(PredicateLibrary.isOn, new List < SharedVar > { current, from }) // isOn(current, from)
        };
    }

    // Effects
    public override List<SharedDelegate> InitEffects()
    {
        return new List<SharedDelegate>
        {
            new SharedDelegate(PredicateLibrary.isClear, new List < SharedVar > { from }), // isClear(from)
            new SharedDelegate(PredicateLibrary.isOn, new List < SharedVar > { current, to }) // isOn(current, to)
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
