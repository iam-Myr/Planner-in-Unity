using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections.LowLevel.Unsafe;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

// TO DO
// [] Goal Set: A set of unsatisfied goals

public class BlockPlanner
{
    private List<Action> allActions;
    private Stack<Node> frontier = new Stack<Node>();
    private WorldState initState;

    public BlockPlanner(List<Action> actions)
    {
        Debug.Log("Planner woke up");
        allActions = actions;
    }

    public List<Action> MakePlan(WorldState initState, WorldState goalState)
    {
        this.initState = initState;

        Node rootNode = new Node(null, goalState, null);
        Node goalNode = new Node(null, initState, null);
        //Debug.Log("GOAL");
        //goalNode.Print();

        frontier.Push(rootNode);
        Node currentNode;

        for(int i = 0; i < 5; i++)
        //while (frontier.Count > 0)
        {
            currentNode = frontier.Pop();
            currentNode.Print();

            // Preprocessing
            Preprocess(currentNode, goalNode);
            //Debug.Log("After prepro");
            //currentNode.Print();

            if (currentNode.isGoal(goalNode)) 
            {
                Debug.Log("GOAL");
                return ReconstructPlan(currentNode); }

            FindChildren(currentNode);
            //Debug.Log($"Frontier size: {frontier.Count}");
        }

        return null; // No plan found
    }

    #region Preprocessing

    public void Preprocess(Node node, Node init)
    {
        WorldState currentState = node.GetState();
        List<SharedDelegate> currentAtoms = currentState.GetAtoms();
        List<SharedDelegate> initAtoms = init.GetState().GetAtoms();

        // Remove goals already satisfied by init state
        foreach (SharedDelegate currentAtom in currentAtoms.ToList()) // Use ToList to safely modify the list while iterating
        {
            if (initAtoms.Contains(currentAtom)) // If goal atom is already satisfied by the init state
            {
                Debug.Log($"Found goal satisfied by init");
                node.RemoveGoal(currentAtom); // Remove satisfied atom from the goal list
            }
        }

        // Set new state after processing
        node.SetState(currentState);
    }

    #endregion

    public void FindChildren(Node currentNode)
    {
        WorldState currentState = currentNode.GetState();

        // Choose a goal atom to satisfy, NO, CHOOSE AN ACTION THAT SATISFIES GOALS !!!!!!!!!!!!!!!!!!!!
        SharedDelegate goalAtom = ChooseGoalAtom(currentNode); // returns (func, args[])

        // For all actions
        foreach (Action action in allActions)
        {
            // DEEP COPY action for safety
            Action actionCopy = action.Clone();

            // For all effects
            foreach (SharedDelegate actionEffect in actionCopy.GetEffects())
            {
                if (canUnify(actionEffect, goalAtom))
                {
                    // Unify effect and goal args 
                    Unify(actionEffect.args, goalAtom.args);

                    //Debug.Log("ACTIONSSS");
                    //actionCopy.Print();
                    //action.Print();

                    // Create new state POSSIBLY COPY HERE TOO
                    WorldState newState = new WorldState().AddAtoms(currentState.GetAtoms().ToArray()); // Copy current state
                    newState.RemoveAtoms(goalAtom); // Remove previous goal atom
                    newState.AddAtoms(actionCopy.GetPreconditions().ToArray()); // Add action preconditions

                    // Create new node
                    frontier.Push(new Node(currentNode, newState, actionCopy));
                    break;
                }
            }
        }
    }

    // Example: on(current, to) with on(B, A) 
    public bool canUnify(
        SharedDelegate p1,
        SharedDelegate p2)
    {
        //Debug.Log($"comparing {p1.args[0]} AND {p2.args[0]}");
        if (p1.func.Method.Name != p2.func.Method.Name) return false; // Func name is not the same
        if (p1.args.Count != p2.args.Count) return false; // Arg length is not the same

        for (int i = 0; i < p1.args.Count; i++) // Compare all args
        {
            SharedVar arg1 = p1.args[i];
            SharedVar arg2 = p2.args[i];

            // If both have value and that value is not equal
            if (arg1.value != null && arg2.value != null && !arg1.value.Equals(arg2.value)) return false;   
        }
        Debug.Log($"MATCH FOUND: {p1.func.Method.Name.ToString()}");
        return true;
    }

    public void Unify(List<SharedVar> p1Args, List<SharedVar> p2Args)
    {
        for (int i = 0; i < p1Args.Count; i++)
        {
            // ??= only assigns if value is null MAYBE on(B, null) on(null, C)
            if (p1Args[i].value == null) 
                p1Args[i].value = p2Args[i].value; 
            else if (p2Args[i].value == null)
                p2Args[i].value = p1Args[i].value;
        }
    }

    // Chooses a goal atom. Currently returns the top one
    public SharedDelegate ChooseGoalAtom(Node currentNode)
    {
       return currentNode.GetUnsatisfiedGoals()[0];
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
