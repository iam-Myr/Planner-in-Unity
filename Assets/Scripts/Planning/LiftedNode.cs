using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using UnityEngine;
using System;
using NUnit.Framework;

namespace Planning
{
    public class LiftedNode
    {
        private LiftedNode parent = null;
        private PlanAction action; // Action applied to this state
        private List<PlanAction> plan = new List<PlanAction>();
        private List<PlanAction> debugPlan = new List<PlanAction>();
        private Predicate goalSatisfied;
        private List<Predicate> unsatisfiedGoals;
        private string logs;
        protected int depth = 0;

        private string ID = "0";
        private int childIndex = 0;

        // used for root
        public LiftedNode(List<Predicate> predicates)
        {
            this.unsatisfiedGoals = predicates;
        }

        // used for children
        public LiftedNode(LiftedNode parent, List<Predicate> unsatGoals, List<PlanAction> plan, string l)
        {
            this.parent = parent;
            this.unsatisfiedGoals = unsatGoals;
            this.plan = plan;
            this.logs = l;

            depth = parent == null ? 0 : parent.depth + 1;
        }

        // IT THINS pickup(A, ?v1) and pickup(A, ?v2) ARE THE SAME

        public void Update(PlanAction action, Predicate goal, string l)
        {
            if (!isActioninPlan(action) && action is not ActionInit){
                plan.Add(action);
                SetGoal(goal);

                RemoveGoals(action.GetEffects());
                AddGoals(action.GetPreconditions());
            }
            else
            {
                // Remove goals but don't add the preconditions
                SetGoal(goal);
                RemoveGoals(action.GetEffects());
            }

            this.action = action;
            debugPlan.Add(action);

            this.logs = l;
            ComputeID();
        }

        // Node is goal if the current state atoms are a subset of the init state
        public bool isGoal(LiftedNode initNode)
        {

            List<Predicate> initPreds = initNode.GetUnsatGoals();


            foreach (Predicate p in unsatisfiedGoals)
            {
                if (!Predicate.ContainsPredicate(initPreds, p))
                    return false;
            }

            return true;
        }

        public void SortbyInit(LiftedNode initNode)
        {
            // Sort goals: goals already satisfied by initNode go to the bottom
            unsatisfiedGoals = unsatisfiedGoals
                .OrderBy(g => initNode.GetUnsatGoals().Any(f => f.Equals(g))) // true = satisfied by init → goes last
                .ToList();
        }

        public LiftedNode Clone(Dictionary<Pointer, Pointer> pointerMap)
        {
            var newGoals = unsatisfiedGoals
                .Select(g => g.Clone(pointerMap))
                .ToList();

            var newPlan = plan?
                .Select(a => a.Clone(pointerMap))
                .ToList();

            var newDebugPlan = debugPlan?
                .Select(a => a.Clone(pointerMap))
                .ToList();

            LiftedNode clone = new LiftedNode(this, newGoals, newPlan, logs);

            clone.debugPlan = newDebugPlan;

            if (action != null)
                clone.action = action.Clone(pointerMap);

            if (goalSatisfied != null)
                clone.goalSatisfied = goalSatisfied.Clone(pointerMap);

            return clone;
        }

        public void AddGoals(List<Predicate> goals)
        {
            foreach (Predicate g in goals)
            {
                if (!unsatisfiedGoals.Any(goal => goal.Equals(g)))
                    unsatisfiedGoals.Add(g);
            }
            //unsatisfiedGoals.AddRange(goals);
        }

        public void RemoveGoals(List<Predicate> satGoals)
        {
            unsatisfiedGoals.RemoveAll(ug =>
                satGoals.Any(g => ug.Equals(g))
            );
        }


        // ------------------------- Console Print -------------------------
        public string ToString()
        {
            string s = "";

            s += "================================== " +
            $"<b><color=#00FFFF>{depth}. {(action != null ? action.ToString() : "ROOT")}</color></b>" +
            $" - <color=GREY>({ID}) </color>" +
            "===================================\n";

            s += $"\n<b><color=#FFD700>Depth:</color></b> {depth}\n";

            s += "\n------------------ " +
                 "<b><color=#FFA500>Previous Action</color></b> " +
                 "-----------------\n";
            if (parent != null && parent.GetAction() != null)
                s += $"{parent.GetAction()}\n";

            //s += "\n------------------ " +
            //"<b><color=#00FF00>Current Action</color></b> " +
            //"------------------\n";
            //if (action != null)
            //s += $"{action}\n";

            s += "\n<color=#AAAAAA>-----------------</color> " +
                "<b><color=#1E90FF>Satisfied Goal</color></b> " +
                 "<color=#AAAAAA>-------------------</color>\n";
            if (goalSatisfied != null)
                s += goalSatisfied.ToString() + "\n";

            s += "\n<color=#AAAAAA>-----------------</color> " +
                "<b><color=#1E90AA>Plan</color></b> " +
                 "<color=#AAAAAA>-------------------</color>\n";
            if (plan != null)
            {
                foreach (PlanAction a in plan)
                    s += "- " + a.ToString() + "\n";
            }

            s += "\n<color=#AAAAAA>-----------------</color> " +
                "<b><color=#1E90AA>Debug Plan</color></b> " +
                 "<color=#AAAAAA>-------------------</color>\n";
            if (debugPlan != null)
            {
                foreach (PlanAction a in debugPlan)
                    s += "- " + a.ToString() + "\n";
            }


            s += "\n-----------------" +
                 "<b><color=#1E90FF>Unsatisfied Goals</color></b> " +
                 "-------------------\n";
            if (unsatisfiedGoals != null)
            {
                foreach (Predicate p in unsatisfiedGoals)
                    s += p.ToString() + "\n";
            }

            s += $"\n<b><color=#FF69B4>Remaining Goals:</color></b> {unsatisfiedGoals.Count}\n";

            s += $"\n<b><color=#ADFF2F>How we got here:</color></b>\n";

            if (logs != null)
                s += logs + "\n";

            s += "\n<color=#AAAAAA>==================================</color> " +
                 "<b><color=#00FFFF>END ANALYSIS</color></b> " +
                 "<color=#AAAAAA>===================================</color>";

            return s;
        }


        void Log(string msg) => Debug.Log($"{msg}");


        // GETTERS

        // Cost Function g(n)
        public int GetCost() => depth;

        // Heuristic Function h(n)
        public int GetHeuristic()
        {
            // Simple heuristic: number of unsatisfied goals
            return unsatisfiedGoals.Count;
        }

        // f(n)
        public int GetTotalCost() => GetCost() + GetHeuristic();

        public int GetDepth() => depth;
        public PlanAction GetAction() => action;
        public void SetAction(PlanAction a) => action = a;
        public LiftedNode GetParent() => parent;
        public List<Predicate> GetUnsatGoals() => unsatisfiedGoals;


        internal List<PlanAction> GetPlan()
        {
            // Has to be reversed cause we're doing regression!
            List<PlanAction> realPlan = new List<PlanAction>(plan);
            realPlan.Reverse();


            // Remove Inits
            realPlan.RemoveAll(a => a is ActionInit);
            return realPlan;
        }


        internal void SetGoal(Predicate goal) => this.goalSatisfied = goal;

        internal bool isSame(LiftedNode other)
        {
            if (unsatisfiedGoals.Count != other.GetUnsatGoals().Count)
                return false;

            foreach (Predicate g in unsatisfiedGoals)
            {
                if (!other.GetUnsatGoals().Any(a => a.Equals(g)))
                    return false;
            }
            return true;
        }

        public bool isActioninPlan(PlanAction a)
        {
            return plan.Any(p => p.Equals(a));
        }

        public void ComputeID()
        {
            if (parent != null)
            {
                parent.childIndex++;
                ID = parent.ID + "." + parent.childIndex;
            }
        }
    }
}
