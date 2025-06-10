using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class ActionMoveTo : Action
{
    private Pointer area;
    private Pointer from;

    public ActionMoveTo()
    {
        actionName = "MoveTo";

        this.area = new Pointer();
        this.from = new Pointer();

        actionArgs = new List<Pointer> {area, from};

        preconditions.AddRange(InitPreconditions());
        effects.AddRange(InitEffects());
    }

    public ActionMoveTo(List<Pointer> args) : base(args)
    {
        actionName = "MoveTo";

        // Extract meaningful references from the list
        this.area = args[0];
        this.from = args[1];

        preconditions.AddRange(InitPreconditions());
        effects.AddRange(InitEffects());
    }

    public override Action CreateNew(List<Pointer> args)
    {
        return new ActionMoveTo(args);
    }

    #region Preconditions
    // Preconditions
    public override List<Predicate> InitPreconditions()
    {
        return new List<Predicate>
        {
            new Predicate(Domain.isAt, new List<Pointer> {from}, true) // isAt(from)  
        };
    }
    #endregion

    // Effects
    public override List<Predicate> InitEffects()
    {
        return new List<Predicate>
        {
           new Predicate(Domain.isAt, new List<Pointer> {area}, true), // isAt(area)  
           new Predicate(Domain.isAt, new List<Pointer> {from}, false)
        };
    }

    public override async Task Execute()
    {
        if (area.Get() is Area target)
        {
            // Logical update
            Debug.Log("Moving to " + target.areaName + " at " + target.GetPosition().ToString());
        }
    }
}
