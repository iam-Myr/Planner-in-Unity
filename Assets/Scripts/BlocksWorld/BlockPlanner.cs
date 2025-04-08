using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;

// TO DO
// [] Goal Set: A set of unsatisfied goals



public class BlockPlanner
{
    private List<Action> allActions;
    private List<Action> plan = new List<Action>();
    private Stack<Node> frontier = new Stack<Node>();

    public BlockPlanner(List<Action> actions)
    {
        Debug.Log("Planner woke up");
        allActions = actions;
    }

    public List<Action> MakePlan(WorldState initState, WorldState goalState)
    {
        Node rootNode = new Node(null, goalState, null);
        Node goalNode = new Node(null, initState, null);
        Debug.Log("GOAL");
        goalNode.Print();

        frontier.Push(rootNode);
        Node currentNode;

        for(int i = 0; i < 5; i++)
        //while (frontier.Count > 0)
        {
            currentNode = frontier.Pop();
            currentNode.Print();

            // Preprocessing
            Preprocess(currentNode, goalNode);
            Debug.Log("After prepro");
            currentNode.Print();

            if (currentNode.isGoal(goalNode)) // CHECK IF THIS WORKS PLS
            {
                return ReconstructPlan(currentNode);
            }

            FindChildren(currentNode);
            //Debug.Log($"Frontier size: {frontier.Count}");
        }

        return null; // No plan found
    }

    public void Preprocess(Node node, Node init)
    {
        WorldState currentState = node.GetState();
        List<(Func<object[], bool>, SharedVar[])> currentAtoms = currentState.GetAtoms();
        List<(Func<object[], bool>, SharedVar[])> initAtoms = init.GetState().GetAtoms();

        // Remove goals already satisfied by init state
        foreach (var currentAtom in currentAtoms.ToList()) // Use ToList to safely modify the list while iterating
        {
            if (initAtoms.Contains(currentAtom)) // If goal atom is already satisfied by the init state
            {
                Debug.Log($"Found goal satisfied by init");
                currentState.RemoveAtoms(currentAtom); // Remove satisfied atom from the goal list
            }
        }

        // Set new state after processing
        node.SetState(currentState);
    }


    public void FindChildren(Node currentNode)
    {
        WorldState currentState = currentNode.GetState();

        // Choose a goal atom to satisfy, NO CHOOSE AN ACTION THAT SATISFIES GOALS !!!!!!!!!!!!!!!!!!!!
        var goalAtom = ChooseGoal(currentState); // returns (func, args)

        // For all actions
        foreach (Action action in allActions)
        {
            // Copy action for safety
            Action actionCopy = action;

            // For all effects
            foreach (var actionEffect in actionCopy.GetEffects())
            {
                if (canUnify(actionEffect, goalAtom))
                {
                    //Debug.Log($"Found match! Args: {goalAtom.Item2[0]}");

                    // Unify effect and goal args only if null
                    SharedVar[] effectArgs = actionEffect.Item2;
                    SharedVar[] goalArgs = goalAtom.Item2;

                    for (int i = 0; i < effectArgs.Length; i++) effectArgs[i].value ??= goalArgs[i].value; //??= only assigns if value is null

                    // Create new state 
                    WorldState newState = currentState;
                    newState.RemoveAtoms(goalAtom);
                    newState.AddAtoms(actionCopy.GetPreconditions().ToArray());

                    // Create new node
                    frontier.Push(new Node(currentNode, newState, actionCopy));
                    break;
                }
            }
        }
    }

    // Example: on(current, to) with on(B, A) 
    public bool canUnify(
        (Func<object[], bool> func, SharedVar[] args) p1,
        (Func<object[], bool> func, SharedVar[] args) p2)
    {
        //Debug.Log($"comparing {p1.args[0]} AND {p2.args[0]}");
        if (p1.func.Method.Name != p2.func.Method.Name) return false; // Func name is the same
        for (int i = 0; i < p1.args.Count(); i++) // Compare all args
        {
            SharedVar arg1 = p1.args[i] ;
            SharedVar arg2 = p2.args[i];

            // If both have value and that value is not equal
            if (arg1.value != null && arg2.value != null && !arg1.value.Equals(arg2.value)) return false;   
        }
        Debug.Log($"MATCH FOUND: {p1.func.Method.Name.ToString()}");
        return true;
    }

    // Chooses a goal atom. Currently returns the top one
    public (Func<object[], bool>, SharedVar[]) ChooseGoal(WorldState state)
    {
        return state.GetAtoms()[0];
    }

    // Using the final node, traces path back to root to create plan
    private List<Action> ReconstructPlan(Node node)
    {
        List<Action> result = new List<Action>();
        while (node != null && node.GetAction() != null)
        {
            result.Insert(0, node.GetAction());
            node = node.GetParent();
        }
        return result;
    }
}
