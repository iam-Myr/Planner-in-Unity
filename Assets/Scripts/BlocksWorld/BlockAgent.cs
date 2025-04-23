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

    public Block blockA, blockB, blockC, blockD, blockE;

    public int loops;

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
        Pointer A = new Pointer();
        Pointer B = new Pointer();
        Pointer C = new Pointer();
        Pointer D = new Pointer();
        Pointer E = new Pointer();

        A.value = blockA;
        B.value = blockB;
        C.value = blockC;
        D.value = blockD;
        E.value = blockE;

        // Init state
        currentState = new WorldState().AddPredicates(
            new Predicate(PredicateLibrary.isClear, new List<Pointer> {B}), // isClear(B)
            new Predicate(PredicateLibrary.isClear, new List<Pointer> {C}), // isClear(C)
            new Predicate(PredicateLibrary.isClear, new List<Pointer> {E}), // isClear(E)
            new Predicate(PredicateLibrary.isOn, new List<Pointer> {B, A}), // isOn(B, A)
            new Predicate(PredicateLibrary.isOn, new List<Pointer> {A, D}) // isOn(A, D)
        );

        // Goal state
        currentGoal = new WorldState().AddPredicates(
            new Predicate(PredicateLibrary.isOn, new List<Pointer> {A, B}), // isOn(A, B)
            new Predicate(PredicateLibrary.isOn, new List<Pointer> {B, C}) // isOn(B, C)
        );

        // Plan!
        currentPlan = planner.MakePlan(currentState, currentGoal, loops);
        // Print!!
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
            Debug.Log($"{i + 1}. {plan[i].Print()} ");
        }
        Debug.Log($"==== {plan.Count} steps ====");
    }


    public async void ExecutePlan(List<Action> plan)
    {
        foreach (Action action in plan)
        {
            // is action possible? yes
            await action.Execute();
        }
    }
}
