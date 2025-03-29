using UnityEngine;
using System.Linq;
using NUnit.Framework;
using System.Collections.Generic;
using System;
public class BlockAgent : MonoBehaviour
{
    private BlockPlanner planner;
    private List<Action> currentPlan;
    private WorldState currentState;
    private WorldState currentGoal;

    private void Start()
    {
        // Init planner with all actions
        planner = new BlockPlanner(GetComponents<Action>().ToList());
        // FInd current state
        currentState = ObserveCurrentState();
        // Choose best goal
        currentGoal = ChooseGoal();
        // Plan!
        currentPlan = planner.MakePlan(currentState, currentGoal);
        // Execute !!
        ExecutePlan(currentPlan);

    }

    // Somehow choose goal based on some criteria. For now there's only 1 goal and it gets explicitly initialized.
    public WorldState ChooseGoal()
    {
        List<KeyValuePair<Func<bool>, Block[]>> goalAtoms = new List<KeyValuePair<Func<bool>, Block[]>>()
        {
        // isOn(B, A)
        new KeyValuePair<Func<bool>, Block[]>(isOn, new Block[] { B, A })
        };
        return new WorldState(goalAtoms);
    }

    public WorldState ObserveCurrentState() {
        List<KeyValuePair<Func<bool>, Block[]>> currentAtoms = new List<KeyValuePair<Func<bool>, Block[]>>()
        {
        // isClear(Block A)
        new KeyValuePair<Func<bool>, Block[]>(isClear, new Block[] { A }),

        // isOn(B,C)
        new KeyValuePair<Func<bool>, Block[]>(isOn, new Block[] { B, C })
        };
        return new WorldState(currentAtoms);

    }

    public void ExecutePlan(List<Action> plan)
    {
        foreach (Action action in plan)
        {
            // is action possible? yes
            action.Execute();
        }
    }
}
