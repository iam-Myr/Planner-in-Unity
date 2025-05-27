using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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

    public bool IsRemovingGoal(Action action, List<Predicate> goals)
    {
        foreach (Predicate g in goals)
        {
            
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
                    if (canUnify(actionEffect, goalPredicate)) // action is useful
                    {

                        // Unify effect and goal args 
                        Unify(actionEffect, goalPredicate);
                        goalsToBeRemoved.Add(goalPredicate);
                        break;
                    }
                }
         
                // Create new state 
                WorldState newState = new WorldState().AddPredicates(currentState.GetPredicates().ToArray()); // Copy current state
                                                                                                                      // MAKE IT SO YOU DON'T ADD INIT PREDICATES
                newState.AddPredicates(actionCopy.GetPreconditions().ToArray()); // Add action preconditions

                // Unify state with init too
                foreach (Predicate p in newState.GetPredicates())
                {
                    foreach (Predicate p_i in initNode.GetState().GetPredicates())
                    {
                        if (canUnify(p, p_i)) // action is useful
                        {
                            // Unify effect and goal args 
                            Unify(p, p_i);
                        }
                    }
                }

                // check new goals that can be satisfied
                 // if action can unify with goal (is useful)
                foreach (Predicate p_e in actionCopy.GetEffects())
                {
                    foreach (Predicate p_g in currentNode.GetUnsatisfiedGoals())
                    if (p_e.isSame(p_g)) 
                    {
                        goalsToBeRemoved.Add(p_g);
                    }
                }

                newState.RemovePredicates(goalsToBeRemoved.ToArray()); // Remove previous goal atom

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

    // Example: on(current, to) with on(B, A) 
    public bool canUnify(Predicate p1, Predicate p2)
    {
        if (p1.func.Method.Name != p2.func.Method.Name) return false;
        if (p1.args.Count != p2.args.Count) return false;

        for (int i = 0; i < p1.args.Count; i++)
        {
            var a = p1.args[i].Get();
            var b = p2.args[i].Get();

            if (a != null && b != null && !a.Equals(b))
                return false;
        }

        return true;
    }


    public void Unify(Predicate p1, Predicate p2)
    {
        for (int i = 0; i < p1.args.Count; i++)
        {
            Pointer a = p1.args[i];
            Pointer b = p2.args[i];

            // If they are different, unify them by reference
            if (a != b)
            {
                a.BindTo(b);
            }
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
