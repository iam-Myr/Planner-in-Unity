using System.Collections.Generic;
using UnityEngine;
using SysDiag = System.Diagnostics;

public class BlockPlanner
{
    private List<Action> allActions;
    private Queue<Node> frontier = new Queue<Node>();
    private List<Node> visited = new List<Node>();
    private Node initNode;

    public BlockPlanner(List<Action> groundedActions)
    {
        Debug.Log("Planner initialized (grounded-only)");
        allActions = groundedActions;
    }

    public List<Action> MakePlan(WorldState initState, WorldState goalState, int maxSteps)
    {
        SysDiag.Stopwatch stopwatch = SysDiag.Stopwatch.StartNew();

        Node rootNode = new Node(null, goalState, null);
        initNode = new Node(null, initState, null);

        frontier.Enqueue(rootNode);
        int step = 0;

        while (frontier.Count > 0 && step < maxSteps)
        {
            Node currentNode = frontier.Dequeue();
            currentNode.Print();

            if (currentNode.isGoal(initNode))
            {
                stopwatch.Stop();
                Debug.Log($"Goal found in {step} steps and depth {currentNode.GetDepth()}");
                Debug.Log($"Planning took {stopwatch.ElapsedMilliseconds} ms");
                return ReconstructPlan(currentNode);
            }

            if (!IsLoop(currentNode))
                FindChildren(currentNode);

            visited.Add(currentNode);
            step++;
        }

        stopwatch.Stop();
        Debug.Log($"Planning stopped after {step} steps.");
        Debug.Log($"Planning took {stopwatch.ElapsedMilliseconds} ms");
        return null;
    }

    private bool IsLoop(Node node)
    {
        foreach (Node n in visited)
            if (n.HasSameGoals(node))
                return true;
        return false;
    }

    private void FindChildren(Node currentNode)
    {
        WorldState currentState = currentNode.GetState();
        List<Predicate> currentGoals = currentNode.GetUnsatisfiedGoals();
        int childrenFound = 0;

        foreach (Predicate goal in currentGoals)
        {
            foreach (Action action in allActions)
            {
                // Skip actions that don't achieve the goal exactly
                if (!action.GetEffects().Contains(goal)) continue;

                // Ensure action doesn't remove any current goals
                if (action.IsRemovingGoal(currentGoals)) continue;

                // Build new state by regressing: replace goal with preconditions
                WorldState newState = new WorldState().AddPredicates(currentState.GetPredicates().ToArray());
                newState.AddPredicates(action.GetPreconditions().ToArray());
                newState.RemovePredicates(goal); // remove achieved goal

                // Optionally remove other goals that are also satisfied by this action
                foreach (Predicate effect in action.GetEffects())
                {
                    if (currentGoals.Contains(effect) && effect != goal)
                        newState.RemovePredicates(effect);
                }

                // Create new node
                Node newNode = new Node(currentNode, newState, action);

                if (!IsLoop(newNode))
                {
                    frontier.Enqueue(newNode);
                    childrenFound++;
                }
            }
        }

        Debug.Log($"Found {childrenFound} children.");
    }

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
