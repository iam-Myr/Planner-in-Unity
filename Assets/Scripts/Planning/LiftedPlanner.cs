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

            // CREATE DUMMY INIT ACTION
            ActionTemplatesList.RemoveAll(a => a is ActionInit);
            ActionTemplatesList.Add(new ActionInit(initState.GetPredicates()));

            while (frontier.Count > 0 && step < maxSteps)
            {
                LiftedNode currentNode = frontier[0];
                frontier.RemoveAt(0);

                if (debug)
                    Debug.Log($"{currentNode.ToString()}");

                if (CanBeGoal(currentNode))
                {
                    stopwatch.Stop();

                    if (currentNode.GetParent() == null)
                        Debug.Log("Goal satisfied already.");

                    Debug.Log($"Planning took {stopwatch.ElapsedMilliseconds} ms");

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



            // Sort goals: goals already satisfied by initNode go to the bottom
            //

            // For every goal in goal list
            for (int i = 0; i < node.GetUnsatGoals().Count(); i++)
            {
                string g = $"GOAL: <b><color=PURPLE> GOAL {node.GetUnsatGoals()[i]}</color></b>.\n";

                // For every action template in actionList
                foreach (PlanAction actionTemplate in ActionTemplatesList)
                {

                    g += $"Exploring <b><color=LIGHTBLUE> ACTION {actionTemplate}</color>\n";
                    // For every effect of this action
                    for (int j = 0; j < actionTemplate.GetEffects().Count(); j++)
                    {

                        LiftedNode currentNode = node.Clone();

                        // CLONE GOAL LIST
                        List<Predicate> clonedGoals = currentNode.GetUnsatGoals();
 
                        // CLONE ACTION
                        PlanAction action = actionTemplate.Clone();

                        // Ban threats
                        action.BanThreats(clonedGoals);

                        // GET CLONED EFFECT
                        Predicate effect = action.GetEffects().ElementAt(j);

                        // Get Cloned goal
                        Predicate goal = clonedGoals.ElementAt(i);

                        g += $" - Exploring effect {effect} of action {action}\n";

                        // Theta-based UNIFICATION HERE
                        Dictionary<Pointer, object> theta = Unification.TryUnify(effect, goal);
                        if (theta != null) // Unification succeded!
                        {
                            g += $"    - Effect {effect} unifies with goal {goal}.\n";
                            // Print theta contents
                            foreach (var kvp in theta) g += $"{kvp.Key} ({kvp.Key.Name}) => {kvp.Value}\n";


                            // Apply bindings
                            Unification.Unify(new List<Predicate> { effect, goal }, theta);
                            g += $"Action <color=GREEN>{action}</color> is USEFUL for goal {goal}!!!\n";

                            // Add action to plan
                            currentNode.AddToPlan(action);

                            // Add goal to node
                            currentNode.SetGoal(goal);

                            // Remove Goals (og and others sat)
                            currentNode.RemoveGoals(action.GetEffects());

                            // Add preconditions as new goals
                            currentNode.AddGoals(action.GetPreconditions());

                            // Add to child list :)
                            children.Add(currentNode);
                        }
                    }
                }

                // Per goal log
                Debug.Log(g);
            }

            return children;
        }


        /*

        private List<GroundNode> FindChildren(GroundNode currentNode)
        {
            List<GroundNode> children = new List<GroundNode>();
            WorldState currentState = currentNode.GetState();
            List<Predicate> currentGoals = currentNode.GetUnsatisfiedGoals();

            // Sort goals: goals already satisfied by initNode go to the bottom
            currentGoals = currentGoals
                .OrderBy(g => initNode.GetState().GetPredicates().Any(f => f.Equals(g))) // true = satisfied by init → goes last
                .ToList();

            // Outer loop: one goal at a time
            for (int i = 0; i < currentGoals.Count(); i++)
            {
                string g = $"GOAL: <b><color=PURPLE> GOAL {currentGoals[i]}</color></b>.\n";

                // Middle loop: all actions
                foreach (PlanAction a in allActions)
                {

                    g += $"Exploring <b><color=LIGHTBLUE> ACTION {a}</color>\n";
                    // Inner loop: all effects of this action
                    for (int j = 0; j < a.GetEffects().Count(); j++)
                    {

                        // CLONE GOAL LIST
                        List<Predicate> clonedGoals = Predicate.CloneList(currentGoals);
                                                                    // Get Cloned goal
                        Predicate goal = clonedGoals.ElementAt(i);

                        // CLONE ACTION
                        PlanAction action = a.Clone();

                        // Ban threats
                        action.BanThreats(clonedGoals);

                        // GET CLONED EFFECT
                        Predicate effect = action.GetEffects().ElementAt(j);

                        g += $" - Exploring effect {effect} of action {action}\n";

                        // Theta-based UNIFICATION HERE
                        Dictionary<Pointer, object> theta = Unification.TryUnify(effect, goal);
                        if (theta != null)
                        {
                            g += $"    - Effect {effect} unifies with goal {goal}.\n";
                            // Print theta contents
                            foreach (var kvp in theta) g += $"{kvp.Key} ({kvp.Key.Name}) => {kvp.Value}\n";


                            // Apply bindings
                            Unification.Unify(new List<Predicate> { effect, goal }, theta);
                            g += $"Action <color=GREEN>{action}</color> is USEFUL for goal {goal}!!!\n";

                            GroundNode newNode = CreateChildNode(currentNode, currentState, action, goal, clonedGoals, g);
                            children.Add(newNode);


                        }
                    }
                }

                // Per goal log
                Debug.Log(g);
            }

            return children;
        } 

        public GroundNode CreateChildNode(
            GroundNode currentNode,
            WorldState currentState,
            PlanAction action,
            Predicate goal,
            List<Predicate> currentGoals,
            string log)
        {
            WorldState newState = new WorldState()
                .AddPredicates(currentState.GetPredicates().ToArray());

            newState.AddPredicates(action.GetPreconditions().ToArray());
            //newState.RemovePredicates(goal);

            // REMOVE GOAL &  OTHER GOALS

            foreach (Predicate effect in action.GetEffects())
            {
                if (currentGoals.Contains(effect))
                    newState.RemovePredicates(effect);
            }

            return new GroundNode(currentNode, newState, action, goal, log);
        } */

        
        private bool CanBeGoal(LiftedNode node)
        {
            List<Predicate> goals = node.GetUnsatGoals();
            List<Predicate> initFacts = initNode.GetUnsatGoals();

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
                        Dictionary<Pointer, object> trialValues =
                            goal.Args.ToDictionary(p => p, p => p.value);

                        Dictionary<Pointer, object> theta =
                            Unification.TryUnify(fact, goal);

                        if (theta != null)
                        {
                            Unification.Unify(new List<Predicate> { goal }, theta);
                            goalSatisfied = true;
                            break;
                        }
                        else
                        {
                            foreach (var kv in trialValues)
                                kv.Key.value = kv.Value;
                        }
                    }

                    if (!goalSatisfied)
                    {
                        foreach (var kv in originalValues)
                            kv.Key.value = kv.Value;

                        return false;
                    }
                }

                return true;
            }
            catch
            {
                foreach (var kv in originalValues)
                    kv.Key.value = kv.Value;

                throw;
            }
        } 
        
        /*
        private List<PlanAction> ReconstructPlan(GroundNode node)
        {
            List<PlanAction> result = new List<PlanAction>();

            while (node != null && node.GetAction() != null)
            {
                PlanAction action = node.GetAction();

                if (!(action is ActionInit))
                    result.Add(action);

                node = node.GetParent();
            }

            return result;
        } 
        */

        private void Append(string s)
        {
            report += s + "\n";
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