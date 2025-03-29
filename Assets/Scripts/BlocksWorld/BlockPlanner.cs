using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BlockPlanner : MonoBehaviour
{
    private List<Action> allActions;
    private List<Action> currentPlan;
    private Stack<WorldState> frontier = new Stack<WorldState>();

    public BlockPlanner(List<Action> actions)
    {
        allActions = actions;
    }
    public List<Action> MakePlan(WorldState initState, WorldState goalState)
    {
        frontier.Push(initState);
        WorldState currentState;

        while (frontier.Count > 0) //Frontier isn't Empty
        {
            currentState = frontier.Pop();
            if (currentState.isGoal(goalState)) return currentPlan;
            findStates(currentState);
        }

        return null;
    }

    public void findStates(WorldState currentState)
    {
        foreach (Action action in allActions)
        {
            if (action.isValid())
            {
                // calculate new state after action is executed
            }
        }
    }
}
