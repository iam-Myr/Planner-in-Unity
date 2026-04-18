using BlocksWorld;
using Planning;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
using SysDiag = System.Diagnostics;

namespace Planning
{
    public class LiftedPlanner
    {
        private List<PlanAction> ActionTemplatesList;
        private List<LiftedNode> frontier = new List<LiftedNode>();
        private LiftedNode initNode;
        private List<Pointer> allPointers { get; }
        private bool debug;
        private string report = "";

        public LiftedPlanner(List<PlanAction> allActions, List<Pointer> allPointers, bool debug)
        {
            Debug.Log("Planner initialized");
            this.ActionTemplatesList = allActions;
            this.allPointers = allPointers;
            this.debug = debug;
        }

        public PlanResult MakePlan(WorldState initState, WorldState goalState, int maxSteps)
        {

            List<LiftedNode> visited = new List<LiftedNode>();
            SysDiag.Stopwatch stopwatch = SysDiag.Stopwatch.StartNew();

            initNode = new LiftedNode(initState.GetPredicates());
            LiftedNode rootNode = new LiftedNode(goalState.GetPredicates());

            frontier.Clear();
            frontier.Add(rootNode);

            int step = 0;

            Debug.Log($"INIT\n {initState}");

            while (frontier.Count > 0 && step < maxSteps)
            {
                // A* HEURISTIC 
                frontier = frontier.OrderBy(n => n.GetTotalCost()).ToList();

                LiftedNode currentNode = frontier[0];
                frontier.RemoveAt(0);

                if (debug)
                    Debug.Log($"{currentNode.ToString()}");

                if (CanBeGoal(currentNode))
                {
                    stopwatch.Stop();

                    if (currentNode.GetParent() == null)
                        Debug.Log("Goal satisfied already.");

                    Debug.Log($"Planning took {stopwatch.ElapsedMilliseconds} ms");;

                    return new PlanResult(
                        currentNode.GetPlan(),
                        stopwatch.ElapsedMilliseconds,
                        step,
                        currentNode.GetDepth()
                    );
                }

                if (!IsLoop(currentNode, visited))
                {
                    List<LiftedNode> children = FindChildren(currentNode); // Infinite loop here :<

                    foreach (LiftedNode child in children)
                        frontier.Add(child);

                    visited.Add(currentNode);
                    step++;
                }
            }

            stopwatch.Stop();
            return null;
        }

        private bool IsLoop(LiftedNode node, List<LiftedNode> visited)
        {
            foreach (LiftedNode n in visited)
                if (n.isSame(node))
                    return true;

            return false;
        }


        private List<LiftedNode> FindChildren(LiftedNode node)
        {
            List<LiftedNode> children = new List<LiftedNode>();

            // existing plan actions + schema
            List<PlanAction> actions = new List<PlanAction>(node.GetPlan());
            actions.AddRange(ActionTemplatesList);
            actions.Add(new ActionInit(initNode.GetUnsatGoals()));


            // Put goals sat by init at the bottom
            //node.SortbyInit(initNode);

            for (int i = 0; i < node.GetUnsatGoals().Count(); i++)
            {
                string g = $"GOAL: <b><color=PURPLE> GOAL {node.GetUnsatGoals()[i]}</color></b>.\n";

                foreach (PlanAction originalAction in actions)
                {
                    g += $"Exploring <b><color=LIGHTBLUE> ACTION {originalAction}</color>\n";

                    // if they action template is removing goals, skip it.
                    Predicate removedGoal = originalAction.IsRemovingGoal(node.GetUnsatGoals());
                    if (removedGoal != null) 
                    {
                        g += $"    - Skipping action {originalAction} because it removes goal {removedGoal}.\n";
                        continue;
                    }
                    for (int j = 0; j < originalAction.GetEffects().Count(); j++)
                    {
                        // ONE shared map per child
                        var pointerMap = new Dictionary<Pointer, Pointer>();


                        // Clone node + action in SAME universe
                        LiftedNode currentNode = node.Clone(pointerMap);
                        PlanAction action = originalAction.Clone(pointerMap);
                        

                        var clonedGoals = currentNode.GetUnsatGoals();

                        action.BanThreats(clonedGoals);

                        Predicate effect = action.GetEffects().ElementAt(j);
                        Predicate goal = clonedGoals.ElementAt(i);

                        g += $" - Exploring effect {effect} of action {action}\n"; 

                        Dictionary<Pointer, object> theta = Unification.TryUnify(effect, goal);

                        if (theta != null)
                        {
                            g += $"    - Effect {effect} unifies with goal {goal}.\n";

                            foreach (var kvp in theta)
                                g += $"{kvp.Key} ({kvp.Key.Name}) => {kvp.Value}\n";

                            //Debug.Log(g);
                            Unification.Unify(new List<Predicate> { effect, goal }, theta);

                            g += $"Action <color=GREEN>{action}</color> is USEFUL for goal {goal}!!!\n";

                            //Update Node with new info! Make it official Child!
                            currentNode.Update(action, goal, g);
                            
                            children.Add(currentNode);
                        }
                    }
                }

                Debug.Log(g);
            }

            return children;
        }


        public bool CanBeGoal(LiftedNode Node)
        {
            List<Predicate> initFacts = initNode.GetUnsatGoals();

            Dictionary<Pointer, object> originalValues = Node.GetUnsatGoals()
                .SelectMany(g => g.Args)
                .Distinct()
                .ToDictionary(p => p, p => p.value);

            bool success = SolveGoals(Node.GetUnsatGoals(), initFacts, 0);

            if (!success)
            {
                // Only restore if failed
                foreach (var kv in originalValues)
                    kv.Key.value = kv.Value;
            }

            return success;
        }

        private bool SolveGoals(List<Predicate> goals, List<Predicate> initFacts, int index)
        {
            if (index >= goals.Count)
                return true;

            Predicate goal = goals[index];

            foreach (Predicate fact in initFacts)
            {
                // Save state BEFORE trying this match
                Dictionary<Pointer, object> snapshot = goals
                    .SelectMany(g => g.Args)
                    .Distinct()
                    .ToDictionary(p => p, p => p.value);

                var theta = Unification.TryUnify(fact, goal);

                if (theta != null)
                {
                    // Apply substitution
                    Unification.Unify(new List<Predicate> { goal }, theta);

                    // Continue with next goal
                    if (SolveGoals(goals, initFacts, index + 1))
                        return true;
                }

                // Restore state
                foreach (var kv in snapshot)
                    kv.Key.value = kv.Value;
            }

            return false;
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
}