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
        // FInd current state
        SharedVar A = new SharedVar();
        SharedVar B = new SharedVar();
        SharedVar C = new SharedVar();

        A.value = blockA;
        B.value = blockB;
        C.value = blockC;

        currentState = new WorldState().AddAtoms(
            (PredicateLibrary.isClear, new object[] {C}),
            (PredicateLibrary.isClear, new object[] {B}),
            (PredicateLibrary.isOn, new object[] {B, A})
        );


        // Choose best goal
        currentGoal = new WorldState();
        // Plan!
        currentPlan = planner.MakePlan(currentState, currentGoal);
        // Profit!!
        PrintPlan(currentPlan);
        // Execute !!
        //ExecutePlan(currentPlan);

    }

    public void PrintPlan(List<Action> plan)
    {
        if (plan == null) print("No plan :(");
        else foreach (Action action in plan) action.Print();
    }

    // Somehow choose goal based on some criteria. For now there's only 1 goal and it gets explicitly initialized.


    public void ExecutePlan(List<Action> plan)
    {
        foreach (Action action in plan)
        {
            // is action possible? yes
            action.Execute();
        }
    }
}
