using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections.LowLevel.Unsafe;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

// TO DO
// [X] Goal Set: A set of unsatisfied goals
// [X] Different action matches: All possible actions for a state
// [X] Rewrite findChildren
// [X] Visited: Prevent Loops ASAP
// [] BFS and DFS using IFrontier
// [] AllDifferent
// [] Node cost: For heuristic, could be number of goals satisfied
// [] Scriptable Objects

public class BlockPlanner
{
    private List<Action> allActions;
    private Queue<Node> frontier = new Queue<Node>();
    List<Node> visited = new List<Node>();
    Node initNode;

    public BlockPlanner(List<Action> actions)
    {
        Debug.Log("Planner woke up");
        allActions = actions;
    }

    public List<Action> MakePlan(WorldState initState, WorldState goalState, int maxSteps)
    {

        Node rootNode = new Node(null, goalState, null);
        initNode = new Node(null, initState, null);
        //Debug.Log("GOAL");
        //goalNode.Print();

        frontier.Enqueue(rootNode);
        Node currentNode;
        int step = 0;

        while (frontier.Count > 0 && step < maxSteps)
        {
            currentNode = frontier.Dequeue();
            currentNode.Print();

            if (currentNode.isGoal(initNode)) 
            {
                Debug.Log($"Goal in {step} steps and {currentNode.GetDepth()} depth.");
                return ReconstructPlan(currentNode); }

            if (!isLoopGoals(currentNode))
                FindChildren(currentNode);
                //Debug.Log($"Frontier size: {frontier.Count}");
                visited.Add(currentNode);
                step++;
        }

        return null; // No plan found
    }

    // NEXT: make it keep shortest path
    public bool isLoop(Node node)
    {
        // Node is loop if it has the same state as a previously visited node
        foreach (Node n in visited)
            if (n.HasSameState(node))
            {
                Debug.Log("Found loop!");
                return true;
            }
        return false;
    }

    public bool isLoopGoals(Node node)
    {
        // Node is loop if it has the same unastisfied goals as a previously visited node
        foreach (Node n in visited)
            if (n.HasSameGoals(node))
            {
                Debug.Log("Found loop!");
                return true;
            }
        return false;
    }

    #region Init

    public void RemoveSatisfiedByInit(Node node, Node init)
    {
        WorldState currentState = node.GetState();
        List<Predicate> currentPredicates = currentState.GetPredicates();
        List<Predicate> initAtoms = init.GetState().GetPredicates();

        // Remove goals already satisfied by init state
        foreach (Predicate currentPredicate in currentPredicates.ToList()) // Use ToList to safely modify the list while iterating
        {
            foreach(Predicate initAtom in initAtoms.ToList())
            if (currentPredicate.isSame(initAtom)) // If goal atom is already satisfied by the init state
            {
                Debug.Log($"Found goal satisfied by init: {currentPredicate.ToString()}");
                node.RemoveGoal(currentPredicate); // Remove satisfied predicate from the goal list
            }
        }
    }

    public void UnifyWithInit(Node node, Node init)
    {
        List<Predicate> currentPredicates = node.GetUnsatisfiedGoals();
        List<Predicate> initAtoms = init.GetState().GetPredicates();

        foreach (Predicate p in currentPredicates)
        {
            foreach (Predicate init_p in initAtoms) {
                if (canUnify(p, init_p))
                    Unify(p, init_p);
            }
        }
    }

    #endregion


    // !!! SPAGHETTI CODE WARNING !!!!
    public void FindChildren(Node currentNode)
    {
        WorldState currentState = currentNode.GetState();
        int childrenFound = 0;

        // For all goals
        foreach (Predicate goalPredicate in currentNode.GetUnsatisfiedGoals())
            
        {
             // For all actions
            foreach (Action action in allActions)
            {
                // DEEP COPY action for safety
                Action actionCopy = action.CreateNew();
                List<Predicate> goalsToBeRemoved = new List<Predicate>();

                // if action can unify with goal

                // For all effect predicates
                foreach (Predicate actionEffect in actionCopy.GetEffects())
                {
                    if (canUnify(actionEffect, goalPredicate))
                    {
                        //Debug.Log($"Found unifying action:");
                        //actionCopy.Print();

                        // Unify effect and goal args 
                        Unify(actionEffect, goalPredicate);

                        // NEXT: Search if action has INSTANTIATED effect that satisfies goal
                        goalsToBeRemoved.Add(goalPredicate);

                        // Create new state 
                        WorldState newState = new WorldState().AddPredicates(currentState.GetPredicates().ToArray()); // Copy current state
                                                                                                                      // MAKE IT SO YOU DON'T ADD INIT PREDICATES
                        newState.AddPredicates(actionCopy.GetPreconditions().ToArray()); // Add action preconditions
                        newState.RemovePredicates(goalsToBeRemoved.ToArray()); // Remove previous goal atom

                        // Create new node
                        Node newNode = new Node(currentNode, newState, actionCopy);

                        //Init Preprocessing
                        UnifyWithInit(newNode, initNode);
                        RemoveSatisfiedByInit(newNode, initNode);

                        if(!isLoopGoals(newNode) && !newNode.isContradiction())
                        {
                            frontier.Enqueue(newNode);
                            childrenFound++;
                        }
                    }     
                }
            }
        }

        Debug.Log($"Found {childrenFound} children!");
    }

    // Example: on(current, to) with on(B, A) 
    public bool canUnify(
    Predicate p1,
    Predicate p2)
    {
        // If the function names do not match, return false
        if (p1.func.Method.Name != p2.func.Method.Name) return false;

        // If the argument lengths are different, return false
        if (p1.args.Count != p2.args.Count) return false;

        // If both predicates already have values
        if (p1.IsInstantiated() && p2.IsInstantiated()) return false;

        // Compare all arguments
        for (int i = 0; i < p1.args.Count; i++)
        {
            SharedVar arg1 = p1.args[i];
            SharedVar arg2 = p2.args[i];

            // If both have values and those values don't match, return false
            if (arg1.value != null && arg2.value != null && !arg1.value.Equals(arg2.value)) return false;
        }

        // Log the match if found
        Debug.Log($"MATCH FOUND: {p1.ToString()} and {p2.ToString()}");

        return true;
    }


    public void Unify(Predicate p1, Predicate p2)
    {
        List<SharedVar> p1Args = p1.args;
        List<SharedVar> p2Args = p2.args;

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
    public Predicate ChooseGoalAtom(Node currentNode)
    {
       return currentNode.GetUnsatisfiedGoals()[0];
    }

    // Using the final node, traces path back to root to create plan
    private List<Action> ReconstructPlan(Node node)
    {
        List<Action> result = new List<Action>();
        while (node != null && node.GetAction() != null)
        {
            result.Add(node.GetAction());
            node = node.GetParent();
        }
        return result;
    }
}
