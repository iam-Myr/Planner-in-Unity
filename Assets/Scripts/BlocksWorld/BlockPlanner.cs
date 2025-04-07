using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class BlockPlanner 
{
    private List<Action> allActions;
    private List<Action> plan = new List<Action>();
    

    public BlockPlanner(List<Action> actions)
    {
        Debug.Log("Planner woke up");
        allActions = actions;
    }


    public List<Action> MakePlan(WorldState initState, WorldState goalState)
    {
        //Check if current state is goal
        

        // Choose a goal atom
        (Func<object[], bool> goalPredicate, object[] goalArgs) = ChooseGoal(goalState);

        // Find an action with an effect matching the goal predicate
        foreach (Action action in allActions)
        {
            Debug.Log("Trying action");
            action.Print();

            foreach ((Func<object[], bool> effectPredicate, object[] effectArgs) in action.GetEffects())
            {
                Debug.Log("Searching...");
                if (SamePredicate((effectPredicate, effectArgs), (goalPredicate, goalArgs)))
                {
                    Debug.Log($"Found match! ARgs: {goalArgs}");

                    // IS THIS HOW YOU COPY ACTIONS ????????????
                    Action actionCopy = action;

                    // Unify SharedVars in effect with goal arguments
                    for (int i = 0; i < effectArgs.Length; i++)
                    {
                        SharedVar sharedVar = effectArgs[i] as SharedVar;
                        if (sharedVar != null)
                        {
                            sharedVar.value = goalArgs[i];
                        }
                    }

                    Debug.Log("COpy action");
                    actionCopy.Print();

                    Debug.Log($"Bound {effectPredicate.Method.Name}({string.Join(", ", goalArgs)}) to action {action.GetType().Name}");


                    plan.Add(actionCopy);
                    return plan; // Return early for now
                }
            }
        }

        return plan;
    }

    public bool SamePredicate(
    (Func<object[], bool> func, object[] args) p1,
    (Func<object[], bool> func, object[] args) p2)
    {
        Debug.Log($"comparing {p1.args[0]} AND {p2.args[0]}");
        return p1.func.Method.Name == p2.func.Method.Name;
    }


    // Chooses a Goal from the goal set.
    // For now returns the first Goal.
    public (Func<object[], bool>, object[]) ChooseGoal(WorldState state)
    {
        return state.GetAtoms()[0];
    }


    //public List<Action> MakePlan(WorldState initState, WorldState goalState)
    //{
    //    frontier.Push(initState);
    //   WorldState currentState;

    //   while (frontier.Count > 0) //Frontier isn't Empty
    //  {
    //      currentState = frontier.Pop();
    //      if (currentState.isGoal(goalState)) return currentPlan;
    //      findStates(currentState);
    //  }

    //   return null;
    // }


}
