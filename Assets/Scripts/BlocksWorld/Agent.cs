using UnityEngine;
using System.Linq;
using NUnit.Framework;
using System.Collections.Generic;
using System;

public class Agent : MonoBehaviour
{
    private Planner planner;
    private List<Action> currentPlan;
    private WorldState currentState;
    private WorldState currentGoal;
    private List<Action> actionList;

    public const int MAXSTEPS =  1000000;

    private void Start()
    {
        // Initialize everything from scene
        Problem.InitFromScene();

        // Load actions
        actionList = Domain.GetActions();

        // Ground actions
        List<Action> groundedActions = ActionGenerator.GenerateAllGroundedActions(actionList, Problem.AllPointers);

        // Initialize planner
        planner = new Planner(groundedActions);

        // Create plan
        currentPlan = planner.MakePlan(Problem.InitialState, Problem.GoalState, MAXSTEPS);

        // Print and execute
        PrintPlan(currentPlan);
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
