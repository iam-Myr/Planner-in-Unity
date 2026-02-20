using Planning;
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
        private bool debug;

        private string report = "";


        public LiftedPlanner(List<PlanAction> allActions, List<Pointer> allPointers, bool debug)
        {
            Debug.Log("Planner initialized");
            this.allActions = allActions;
            this.allPointers = allPointers;
            this.debug = debug;
        }

        public PlanResult MakePlan(WorldState initState, WorldState goalState, int maxSteps)
        {
            List<Node> visited = new List<Node>();
            SysDiag.Stopwatch stopwatch = SysDiag.Stopwatch.StartNew();

            initNode = new Node(null, initState, null);
            Node rootNode = new Node(null, goalState, null);

            frontier.Clear();
            frontier.Add(rootNode);

            int step = 0;
            Debug.Log($"{initState.GetPredicates().Count} init predicates");

            // CREATE DUMMY INIT ACTION AND ADD IT TO ACTION LIST
            //PlanAction initAction = new ActionInit(initState.GetPredicates());
            //allActions.Add(initAction);

            while (frontier.Count > 0 && step < maxSteps)
            {
                // A* ordering
                frontier.Sort((a, b) => a.GetTotalCost().CompareTo(b.GetTotalCost()));

                Node currentNode = frontier[0];
                frontier.RemoveAt(0);

                if (debug)
                    currentNode.Print();

                // If the state can Unify with Init and produce a goal
                if (CanBeGoal(currentNode))
                {
                    stopwatch.Stop();

                    if (currentNode.GetParent() == null)
                        Debug.Log("Goal satisfied already.");

                    Debug.Log($"Planning took {stopwatch.ElapsedMilliseconds} ms");

                    return new PlanResult(ReconstructPlan(currentNode), stopwatch.ElapsedMilliseconds, step, currentNode.GetDepth());
                }

                if (!IsLoop(currentNode, visited))
                {
                    List<Node> children = FindChildren(currentNode);

                    foreach (Node child in children)
                    {
                        frontier.Add(child);
                    }
                }

                visited.Add(currentNode);
                step++;
            }

            stopwatch.Stop();
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

            // Outer loop: one goal at a time
            foreach (Predicate goal in currentGoals)
            {
                string g = $"GOAL: <b><color=PURPLE> GOAL {goal}</color></b>.\n";

                // Middle loop: all actions
                foreach (PlanAction a in allActions)
                {
                    // Inner loop: all effects of this action
                    for (int i = 0; i < a.GetEffects().Count; i++)
                    {
                        // Deep clone the action BEFORE unification
                        PlanAction actionInstance = a.Clone();

                        Predicate effectInstance = actionInstance.GetEffects()[i];

                        g += $"Exploring <b><color=BLUE> ACTION {actionInstance}</color></b> with effect {effectInstance}\n";

                        // Try unifying this effect with the current goal
                        if (Unification.Unify(effectInstance, goal))
                        {

                            if (Unification.Unify(effectInstance, goal))
                            {
                                // Check constraints before committing
                                if (!actionInstance.SatisfiesConstraints())
                                {
                                    g += $"Action <color=red>{actionInstance}</color> fails AllDifferent constraint for goal {goal}\n";
                                    continue; // skip this action effect
                                }

                                g += $"Action <color=green>{actionInstance}</color> is USEFUL for goal {goal}!!!\n";
                            }

                            g += $"    - Effect {effectInstance} unifies with goal {goal}.\n";
                            g += $"Action <color=GREEN>{actionInstance}</color> is USEFUL for goal {goal}!!!\n";

                            // Create a new child node using this freshly bound action
                            Node newNode = CreateChildNode(currentNode, currentState, actionInstance, goal, currentGoals);
                            children.Add(newNode);
                        }
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

        private bool CanBeGoal(Node node)
        {
            List<Predicate> goals = node.GetUnsatisfiedGoals();
            List<Predicate> initFacts = initNode.GetState().GetPredicates();

            string log = $"<b><color=purple>Checking if node can be goal:</color></b>\n";

            // Save original pointer values to restore if trial fails
            Dictionary<Pointer, object> originalValues = goals
                .SelectMany(g => g.Args)
                .Distinct()
                .ToDictionary(p => p, p => p.value);

            try
            {
                foreach (Predicate goal in goals)
                {
                    bool goalSatisfied = false;

                    foreach (Predicate fact in initFacts)
                    {
                        // Trial: save current pointer values
                        Dictionary<Pointer, object> trialValues = goal.Args
                            .ToDictionary(p => p, p => p.value);

                        // Attempt unification
                        if (Unification.Unify(goal, fact))
                        {
                            // Check the action's constraints (AllDifferent etc.)
                            if (node.GetAction() != null && !node.GetAction().SatisfiesConstraints())
                            {
                                log += $"<color=red>Goal {goal} unified with {fact} but action constraints FAILED</color>\n";

                                // Restore trial pointer values
                                foreach (var kv in trialValues)
                                    kv.Key.value = kv.Value;

                                continue; // try next fact
                            }

                            // Success → goal satisfied
                            goalSatisfied = true;
                            log += $"<color=green>Goal {goal} unified with init fact {fact}</color>\n";

                            // Log pointer bindings
                            foreach (Pointer p in goal.Args)
                                log += $"    {p} -> {p.Get()}\n";

                            break; // stop trying other facts for this goal
                        }
                        else
                        {
                            // Unification failed → restore trial pointer values
                            foreach (var kv in trialValues)
                                kv.Key.value = kv.Value;
                        }
                    }

                    if (!goalSatisfied)
                    {
                        // At least one goal cannot unify → restore all pointers
                        foreach (var kv in originalValues)
                            kv.Key.value = kv.Value;

                        Debug.Log(log);
                        return false;
                    }
                }

                // All goals unified and action constraints satisfied → keep bindings
                Debug.Log(log);
                return true;
            }
            catch
            {
                // On exception, restore original pointer values
                foreach (var kv in originalValues)
                    kv.Key.value = kv.Value;
                throw;
            }
        }



        private List<PlanAction> ReconstructPlan(Node node)
        {
            List<PlanAction> result = new List<PlanAction>();
            while (node != null && node.GetAction() != null)
            {
                PlanAction action = node.GetAction();
                if (!(action is ActionInit)) // skip Init actions FOR NOW
                    result.Add(action);
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

public class PlanResult
{
    public List<PlanAction> plan;
    public long TimeMs;
    public int Steps;
    public int Depth;

    public PlanResult(List<PlanAction> planActions, long timeMs, int steps, int depth)
    {
        this.plan = planActions;
        this.TimeMs = timeMs;
        this.Steps = steps;
        this.Depth = depth;
    }
}
