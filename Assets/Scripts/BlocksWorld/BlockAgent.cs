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

    public Block blockA, blockB, blockC;

    private void Start()
    {
        // Init planner with all actions
        planner = new BlockPlanner(GetComponents<Action>().ToList());
        
        // Blocks
        SharedVar A = new SharedVar();
        SharedVar B = new SharedVar();
        SharedVar C = new SharedVar();

        A.value = blockA;
        B.value = blockB;
        C.value = blockC;

        // Init state
        currentState = new WorldState().AddAtoms(
            (PredicateLibrary.isClear, new SharedVar[] {C}),
            (PredicateLibrary.isClear, new SharedVar[] {B}),
            (PredicateLibrary.isOn, new SharedVar[] {B, A})
        );

        // Goal state
        currentGoal = new WorldState().AddAtoms(
            (PredicateLibrary.isOn, new SharedVar[] {B, C})
        );

        // Plan!
        currentPlan = planner.MakePlan(currentState, currentGoal);
        // Profit!!
        PrintPlan(currentPlan);
        // Execute !!
        //ExecutePlan(currentPlan);

    }

    public void PrintPlan(List<Action> plan)
    {
        if (plan == null) Debug.Log("No plan :(");
        else foreach (Action action in plan) action.Print();
        Debug.Log($"{plan.Count}");
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
