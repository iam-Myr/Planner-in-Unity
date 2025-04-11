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
    private List<Action> actionList;

    public Block blockA, blockB, blockC;

    private void Start()
    {
        // ACtion init
        actionList = new List<Action>
        {
            new ActionMove()
        };

        // Init planner with all actions
        planner = new BlockPlanner(actionList);
        
        // Blocks
        SharedVar A = new SharedVar();
        SharedVar B = new SharedVar();
        SharedVar C = new SharedVar();

        A.value = blockA;
        B.value = blockB;
        C.value = blockC;

        // Init state
        currentState = new WorldState().AddAtoms(
            new SharedDelegate(PredicateLibrary.isClear, new List<SharedVar> {C}), // isClear(C)
            new SharedDelegate(PredicateLibrary.isClear, new List<SharedVar> {B}), // isClear(B)
            new SharedDelegate(PredicateLibrary.isOn, new List<SharedVar> {B, A}) // isOn(B, A)
        );

        // Goal state
        currentGoal = new WorldState().AddAtoms(
            new SharedDelegate(PredicateLibrary.isOn, new List<SharedVar> { B, C }) // isOn(B, C)
            //new SharedDelegate(PredicateLibrary.isOn, new List<SharedVar> { A, B }) //isOn(A, B)
        );

        // Plan!
        currentPlan = planner.MakePlan(currentState, currentGoal);
        // Profit!!
        PrintPlan(currentPlan);
        // Execute !!
        ExecutePlan(currentPlan);

    }

    public void PrintPlan(List<Action> plan)
    {
        if (plan == null || plan.Count == 0)
        {
            Debug.Log("No plan :(");
            return;
        }

        Debug.Log("==== PLAN ====");
        for (int i = 0; i < plan.Count; i++)
        {
            Debug.Log($"{i + 1}. ");
            plan[i].Print();  // Assuming this prints nicely to console
        }
        Debug.Log($"==== {plan.Count} steps ====");
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
