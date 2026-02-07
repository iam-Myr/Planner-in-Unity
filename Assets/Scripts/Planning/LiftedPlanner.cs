using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using SysDiag = System.Diagnostics;

namespace Planning
{
    public class LiftedPlanner
    {
        private List<PlanAction> allActions;
        private List<Node> frontier = new List<Node>();
        private Node initNode;
        private List<Pointer> allPointers { get; }

        public LiftedPlanner(List<PlanAction> allActions, List<Pointer> allPointers)
        {
            Debug.Log("Planner initialized");
            this.allActions = allActions;
            this.allPointers = allPointers;
        }

        public List<PlanAction> MakePlan(WorldState initState, WorldState goalState, int maxSteps)
        {
            //initState.Print();
            //goalState.Print();

            List<Node> visited = new List<Node>();
            SysDiag.Stopwatch stopwatch = SysDiag.Stopwatch.StartNew();

            initNode = new Node(null, initState, null);
            Node rootNode = new Node(null, goalState, null);

            if (rootNode.isGoal(initNode))
            {
                Debug.Log("Goal satisfied already.");
                return null;
            }
            
            frontier.Add(rootNode);
            int step = 0;

            while (frontier.Count > 0 && step < maxSteps)
            {
                frontier.Sort((a, b) => a.GetTotalCost().CompareTo(b.GetTotalCost()));
                Node currentNode = frontier[0];
                frontier.RemoveAt(0);
                currentNode.Print();
                //currentNode.PrintToFile();

                if (!IsLoop(currentNode, visited))
                {
                    List<Node> children = FindChildren(currentNode);

                    foreach (Node child in children)
                    {
                        if (child.isGoal(initNode))
                        {
                            stopwatch.Stop();
                            //Debug.Log($"Goal found in {step} steps and depth {child.GetDepth()}");
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
            //Debug.Log($"Planning stopped after {step} steps.");
            //Debug.Log($"Planning took {stopwatch.ElapsedMilliseconds} ms");
            return null;
        }


        private bool IsLoop(Node node, List<Node> visited)
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

            // For each goal in the current goals
            foreach (Predicate goal in currentGoals)
            {
                // For each available action
                foreach (PlanAction action in allActions)
                {
                    // Clone the action so unification doesn't leak bindings
                    PlanAction actionClone = action.Clone();

                    bool is_useful = false;
                    // For each effect of action
                    foreach (Predicate effect in actionClone.GetEffects())



                        // If it can unify with the goal, the action is useful
                        if (Unification.Unify(effect, goal)) // UNIFICATION HERE, needs banned lists
                        {
                            is_useful = true; // They have unified.
                        }



                    // Unification for the current action has ended
                    if (actionClone.IsRemovingGoal(currentGoals)) is_useful = false;

                    if (is_useful)
                    {
                        // Check if init can unifyyy
                        foreach (Predicate satisfiedInInit in initNode.GetState().GetPredicates())
                        {
                            foreach (Predicate precond in actionClone.GetPreconditions())
                                Unification.Unify(precond, satisfiedInInit);
                        }

                        WorldState newState = new WorldState().AddPredicates(currentState.GetPredicates().ToArray());
                        newState.AddPredicates(actionClone.GetPreconditions().ToArray());
                        newState.RemovePredicates(goal);

                        foreach (Predicate effect in actionClone.GetEffects())
                        {
                            if (currentGoals.Contains(effect) && effect != goal)
                                newState.RemovePredicates(effect);
                        }

                        Node newNode = new Node(currentNode, newState, actionClone);
                        children.Add(newNode);
                    }
                }
            }

            return children;
        }


        private List<PlanAction> ReconstructPlan(Node node)
        {
            List<PlanAction> result = new List<PlanAction>();
            while (node != null && node.GetAction() != null)
            {
                result.Add(node.GetAction());
                node = node.GetParent();
            }
            return result;
        }
    }
}
