using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using SysDiag = System.Diagnostics;

#if false
namespace Planning
{
    public class LiftedPlannerr
    {
        private List<PlanAction> allActions;
        private List<Node> frontier = new List<Node>();
        private Node initNode;
        private List<Pointer> allPointers { get; }
        private bool debug;

        private string report = "";


        public LiftedPlannerr(List<PlanAction> allActions, List<Pointer> allPointers, bool debug)
        {
            Debug.Log("Planner initialized");
            this.allActions = allActions;
            this.allPointers = allPointers;
            this.debug = debug;
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
                // Sorts frontier for A* USE INSERTION SORT AT SOME POINT

                frontier.Sort((a, b) => a.GetTotalCost().CompareTo(b.GetTotalCost()));
                Node currentNode = frontier[0];
                frontier.RemoveAt(0);
                if (debug) currentNode.Print();
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
            List<Node> children = new List<Node>();
            WorldState currentState = currentNode.GetState();
            List<Predicate> currentGoals = currentNode.GetUnsatisfiedGoals();

            // For each goal in the current goals
            foreach (Predicate goal in currentGoals)
            {
                string g = $"Exploring<b><color=PURPLE> GOAL {goal.ToString()}</color></b>.\n";
                
                // For each available action
                foreach (PlanAction a in allActions)
                {
                    // Clone the action so unification doesn't leak bindings
                    PlanAction action = a.Clone();

                    g += $"Exploring <b><color=BLUE> ACTION {action.ToString()}</color></b>.\n";

                    bool is_useful = false;
                    // For each effect of action
                    foreach (Predicate effect in action.GetEffects())
                    {
                        if (!action.hasNullValues()) break;
                        // If even one can unify with the goal, the action is useful
                        if (Unification.Unify(effect, goal))
                        {// UNIFICATION HERE, needs banned lists
                            is_useful = true; // They have unified.
                            g += $"   - Unified predicates: {effect.ToString()} and {goal.ToString()}\n ({action.ToString()})";
                        }
                    }

                    g += $"After unification, action {action.ToString()} was useful - {is_useful}\n";

                    if (is_useful)
                    {
                        // Check if init can unifyyy
                        foreach (Predicate satisfiedInInit in initNode.GetState().GetPredicates())
                        {
                            if (!action.hasNullValues()) break;
                            foreach (Predicate precond in action.GetPreconditions())
                            {
                                if (Unification.Unify(precond, satisfiedInInit)) {
                                    g += $"   - Unified predicates: {precond.ToString()} and {satisfiedInInit.ToString()} ({action.ToString()})\n";
                                }
                            }
                        }
                        g += $"After init, action {action.ToString()}\n";
                    }



                    // IF THERE ARE NULL VALUES

                    if (is_useful && action.hasNullValues())
                    {
                        g += $"Action {action.ToString()} has null values after unification.\n";
                        //for (int i = 0; i < actionClone.actionArgs.Count; i++)
                        //{
                        //   if (actionClone.actionArgs[i] == null)
                        //  {
                        // Assign a random pointer from the pool
                        //     int randIndex = UnityEngine.Random.Range(0, allPointers.Count);
                        //     actionClone.actionArgs[i] = allPointers[randIndex];
                        // }
                        //}
                        //is_useful = false;
                    }

                    // Unification for the current action has ended
                    if (!action.SatisfiesConstraints())
                    {
                        g += $"Action {action.ToString()} doesn't satisfy AllDifferent Constraint\n";
                        is_useful = false;
                    }


                    if (action.IsRemovingGoal(currentGoals))
                    {
                        g += $"Action {action.ToString()} is Removing a Goal\n";
                        is_useful = false;
                    }


                    if (is_useful)
                    {
                        g += $"Action <color=GREEN>{action.ToString()}</color> is useful for goal {goal}\n";
                        Node newNode = CreateChildNode(currentNode, currentState, action, goal, currentGoals);
                        children.Add(newNode);
                    }
                }

                Debug.Log(g);
            }

            return children;
        }

        public Node CreateChildNode(Node currentNode, WorldState currentState, PlanAction actionClone, Predicate goal, List<Predicate> currentGoals)
        {
            // Start with a copy of the current state
            WorldState newState = new WorldState().AddPredicates(currentState.GetPredicates().ToArray());

            // Add the preconditions of the action
            newState.AddPredicates(actionClone.GetPreconditions().ToArray());

            // Remove the current goal
            newState.RemovePredicates(goal);

            // Remove effects that are also current goals (except the goal we're regressing)
            foreach (Predicate effect in actionClone.GetEffects())
            {
                if (currentGoals.Contains(effect) && effect != goal)
                    newState.RemovePredicates(effect);
            }

            // Create and return the new child node
            return new Node(currentNode, newState, actionClone);
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

        private void Append(string s)
        {
            report += s + "\n";
        }

    }
}
#endif