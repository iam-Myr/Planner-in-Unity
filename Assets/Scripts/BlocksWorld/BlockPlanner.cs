using System.Collections.Generic;
using UnityEngine;
using SysDiag = System.Diagnostics;

public class BlockPlanner
{
    private List<Action> allActions;
    private List<Node> frontier = new List<Node>();
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

        initNode = new Node(null, initState, null);
        Node rootNode = new Node(null, goalState, null);

        frontier.Add(rootNode);
        int step = 0;

        while (frontier.Count > 0 && step < maxSteps)
        {
            frontier.Sort((a, b) => a.GetTotalCost().CompareTo(b.GetTotalCost()));
            Node currentNode = frontier[0];
            frontier.RemoveAt(0);
            currentNode.Print();

            if (!IsLoop(currentNode))
            {
                List<Node> children = FindChildren(currentNode);

                foreach (Node child in children)
                {
                    if (child.isGoal(initNode))
                    {
                        stopwatch.Stop();
                        Debug.Log($"Goal found in {step} steps and depth {child.GetDepth()}");
                        Debug.Log($"Planning took {stopwatch.ElapsedMilliseconds} ms");
                        return ReconstructPlan(child);
                    }

                    frontier.Add(child);
                }
            }

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

    private List<Node> FindChildren(Node currentNode)
    {
        var children = new List<Node>();
        WorldState currentState = currentNode.GetState();
        List<Predicate> currentGoals = currentNode.GetUnsatisfiedGoals();

        foreach (Predicate goal in currentGoals)
        {
            foreach (Action action in allActions)
            {
                if (!action.GetEffects().Contains(goal)) continue; // If action is not useful
                if (action.IsRemovingGoal(currentGoals)) continue;

                WorldState newState = new WorldState().AddPredicates(currentState.GetPredicates().ToArray());
                newState.AddPredicates(action.GetPreconditions().ToArray());
                newState.RemovePredicates(goal);

                foreach (Predicate effect in action.GetEffects())
                {
                    if (currentGoals.Contains(effect) && effect != goal)
                        newState.RemovePredicates(effect);
                }

                Node newNode = new Node(currentNode, newState, action);

                children.Add(newNode);
                
            }
        }

        return children;
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
