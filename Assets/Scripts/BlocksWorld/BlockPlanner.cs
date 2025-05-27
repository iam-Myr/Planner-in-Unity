using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using SysDiag = System.Diagnostics;



// TO DO
// [X] Goal Set: A set of unsatisfied goals
// [X] Rewrite findChildren
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
        SysDiag.Stopwatch stopwatch = SysDiag.Stopwatch.StartNew();


        Node rootNode = new Node(null, goalState, null);
        initNode = new Node(null, initState, null);

        frontier.Enqueue(rootNode);
        Node currentNode;
        int step = 0;

        while (frontier.Count > 0 && step < maxSteps)
        {
            currentNode = frontier.Dequeue();
            currentNode.Print();

            if (currentNode.isGoal(initNode))
            {
                stopwatch.Stop();
                Debug.Log($"Goal found in {step} steps and {currentNode.GetDepth()} depth.");
                Debug.Log($"Planning took {stopwatch.ElapsedMilliseconds} ms");
                return ReconstructPlan(currentNode);
            }

            if (!isLoopGoals(currentNode))
                FindChildren(currentNode);

            visited.Add(currentNode);
            step++;
        }

        stopwatch.Stop();
        Debug.Log($"Planning stopped after {step} steps.");
        Debug.Log($"Planning took {stopwatch.ElapsedMilliseconds} ms");

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


    public void UnifyWithInit(Node node, Node init)
    {
        List<Predicate> currentPredicates = node.GetUnsatisfiedGoals();
        List<Predicate> initAtoms = init.GetState().GetPredicates();

        foreach (Predicate p in currentPredicates)
        {
            foreach (Predicate init_p in initAtoms) {
                if (Unification.CanUnify(p, init_p))
                    Unification.Unify(p, init_p);
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

                // if action can unify with goal (is useful)
                foreach (Predicate actionEffect in actionCopy.GetEffects())
                {
                    if (Unification.CanUnify(actionEffect, goalPredicate)) // action is useful
                    {

                        // Unify effect and goal args 
                        Unification.Unify(actionEffect, goalPredicate);
                        goalsToBeRemoved.Add(goalPredicate);
                        break;
                    }
                }

                // **NEW CHECK: skip action if it removes any goal**
                if (actionCopy.IsRemovingGoal(currentNode.GetUnsatisfiedGoals()))
                {
                    // Skip this action since it removes a goal
                    continue;
                }

                // Create new state 
                WorldState newState = new WorldState().AddPredicates(currentState.GetPredicates().ToArray()); // Copy current state
                                                                                                                      // MAKE IT SO YOU DON'T ADD INIT PREDICATES
                newState.AddPredicates(actionCopy.GetPreconditions().ToArray()); // Add action preconditions

                // Unify state with init too
                newState.UnifyWith(initNode.GetState());


                // CHECK IF THE USEFUL ACTION CAN SATISFY ANY OTHER FULLY INSTANTIATED GOALS (THIS IS VERY IMPORTANT!)
                goalsToBeRemoved.AddRange(actionCopy.SatisfyOtherGoals(currentNode.GetUnsatisfiedGoals()));

                newState.RemovePredicates(goalsToBeRemoved.ToArray()); 

                // Create new node
                Node newNode = new Node(currentNode, newState, actionCopy);

                if (!isLoopGoals(newNode))// && !newNode.isContradiction())
                {
                    frontier.Enqueue(newNode);
                    childrenFound++;
                }    
            }
        }
        Debug.Log($"Found {childrenFound} children!");
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
