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

            initNode = new Node(initState);
            Node rootNode = new Node(goalState);

            frontier.Clear();
            frontier.Add(rootNode);

            int step = 0;

            Debug.Log($"INIT\n {initState.ToString()}");

            // CREATE DUMMY INIT ACTION AND ADD IT TO ACTION LIST
            allActions.RemoveAll(a => a is ActionInit);
            allActions.Add(new ActionInit(initState.GetPredicates()));
            //allActions.Insert(0, new ActionInit(initState.GetPredicates()));

            while (frontier.Count > 0 && step < maxSteps)
            {
                // A* ordering
                //frontier.Sort((a, b) => a.GetTotalCost().CompareTo(b.GetTotalCost()));

                Node currentNode = frontier[0];
                frontier.RemoveAt(0);

                if (debug)
                    Debug.Log($"{currentNode.ToString()}");

                // If the state can Unify with Init and produce a goal
                if (currentNode.isGoal(initNode))
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

                    visited.Add(currentNode);
                    step++;
                }
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

            // Sort goals: goals already satisfied by initNode go to the bottom
            currentGoals = currentGoals
                .OrderBy(g => initNode.GetState().GetPredicates().Any(f => f.Equals(g))) // true = satisfied by init → goes last
                .ToList();

            // Outer loop: one goal at a time
            foreach (Predicate goal in currentGoals)
            {
                string g = $"GOAL: <b><color=PURPLE> GOAL {goal}</color></b>.\n";

                // Middle loop: all actions
                foreach (PlanAction a in allActions)
                {
                    // Deep clone the action BEFORE unification
                    PlanAction action = a.Clone();

                    // Ban threats
                    action.BanThreats(currentGoals);

                    g += $"Exploring <b><color=LIGHTBLUE> ACTION {a}</color>\n";
                    // Inner loop: all effects of this action
                    foreach(Predicate effect in action.GetEffects())
                    {
         
                        g += $" - Exploring effect {effect} of action {action}\n";

                        // Theta-based UNIFICATION HERE
                        Dictionary<Pointer, object> theta = Unification.TryUnify(effect, goal);
                        if (theta != null)
                        {
                            g += $"    - Effect {effect} unifies with goal {goal}.\n";
                            // Print theta contents
                            foreach (var kvp in theta)g += $"{kvp.Key} ({kvp.Key.Name}) => {kvp.Value}\n";

                            Predicate tempEffect = effect.Clone();
                            // Apply bindings
                            Unification.Unify(new List<Predicate> { effect, goal}, theta);
                            g += $"Action <color=GREEN>{action}</color> is USEFUL for goal {goal}!!!\n";

                            Node newNode = CreateChildNode(currentNode, currentState, action, goal, currentGoals, g);
                            children.Add(newNode);

                        
                        }
                    }
                }

                Debug.Log(g);
            }

            return children;
        }


        public Node CreateChildNode(Node currentNode, WorldState currentState, PlanAction action, Predicate goal, List<Predicate> currentGoals, string log)
        {
            // Start with a copy of the current state
            WorldState newState = new WorldState().AddPredicates(currentState.GetPredicates().ToArray());

            // Add the preconditions of the action
            newState.AddPredicates(action.GetPreconditions().ToArray());

            // Remove the current goal
            newState.RemovePredicates(goal);

            // Remove effects that are also current goals (except the goal we're regressing)
            foreach (Predicate effect in action.GetEffects())
            {
                if (currentGoals.Contains(effect) && effect != goal)
                    newState.RemovePredicates(effect);
            }

            // Create and return the new child node
            return new Node(currentNode, newState, action, goal, log);
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
                        // Save trial pointer values
                        Dictionary<Pointer, object> trialValues = goal.Args
                            .ToDictionary(p => p, p => p.value);

                        // Attempt unification
                        Dictionary<Pointer, object> theta = Unification.TryUnify(fact, goal);
                        if (theta != null)
                        {
                            // Apply the unification bindings
                            // Apply bindings
                            Unification.Unify(new List<Predicate> {goal}, theta);

                            goalSatisfied = true;
                            log += $"<color=green>Goal {goal} unified with init fact {fact}</color>\n";

                            // Log pointer bindings
                            foreach (Pointer p in goal.Args)
                                log += $"    {p} -> {p.Get()}\n";

                            break; // Stop trying other facts for this goal
                        }
                        else
                        {
                            // Restore trial pointer values if unification fails
                            foreach (var kv in trialValues)
                                kv.Key.value = kv.Value;
                        }
                    }

                    if (!goalSatisfied)
                    {
                        // Restore all original pointers if at least one goal fails
                        foreach (var kv in originalValues)
                            kv.Key.value = kv.Value;

                        return false;
                    }
                }

                // All goals unified → keep bindings
                return true;
            }
            catch
            {
                // Restore all original pointers in case of exception
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
