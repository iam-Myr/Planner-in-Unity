using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;


// How can we move the agent??
public class ActionMoveTo : PlanAction
{
    private object area;
    private object from;

    public ActionMoveTo()
    {
        actionName = "MoveTo";

        this.area = new object();
        this.from = new object();

        actionArgs = new List<object> {area, from};

        preconditions.AddRange(InitPreconditions());
        effects.AddRange(InitEffects());
    }

    public ActionMoveTo(List<object> args) : base(args)
    {
        actionName = "MoveTo";

        // Extract meaningful references from the list
        this.area = args[0];
        this.from = args[1];

        preconditions.AddRange(InitPreconditions());
        effects.AddRange(InitEffects());
    }

    public override PlanAction CreateNew(List<object> args)
    {
        return new ActionMoveTo(args);
    }

    #region Preconditions
    // Preconditions
    public override List<Predicate> InitPreconditions()
    {
        return new List<Predicate>
        {
            new Predicate(SimDomain.isAt, new List<object> {from}, true) // isAt(from)  
        };
    }
    #endregion

    // Effects
    public override List<Predicate> InitEffects()
    {
        return new List<Predicate>
        {
           new Predicate(SimDomain.isAt, new List<object> {area}, true), // isAt(area)  
           new Predicate(SimDomain.isAt, new List<object> {from}, false)
        };
    }

    public override async Task Execute(Agent agent)
    {
        if (area is Area target && agent is SimAgent simAgent)
        {
            //Debug.Log("Moving to " + target.areaName);
            Vector3 destination = target.GetPosition();
            Transform t = agent.transform;

            while (Vector3.Distance(t.position, destination) > 0.1f)
            {
                t.position = Vector3.MoveTowards(t.position, destination, simAgent.moveSpeed * Time.deltaTime);
                await Task.Yield();  // wait for next frame
            }

            //Debug.Log("Arrived at " + target.areaName);
        }
    }
}
