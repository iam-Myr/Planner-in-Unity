using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using UnityEngine;

namespace Planning
{
    public class Node
    {
        private Node parent;
        private WorldState state;
        private PlanAction action; // Action applied to this state
        private List<Predicate> unsatisfiedGoals; // init might not actually achieve it
        protected int depth;

        // TXT Logger path
        private static readonly string logPath =
            Path.Combine(Application.persistentDataPath, "planner_nodes.txt");

        public Node(Node parent, WorldState state, PlanAction action)
        {
            this.parent = parent;
            this.state = state;
            this.action = action;

            // Init goals
            unsatisfiedGoals = new List<Predicate>(state.GetPredicates());

            depth = parent == null ? 0 : parent.depth + 1;
        }

        // Node is goal if the current state atoms are a subset of the init state
        public bool isGoal(Node initNode)
        {
            List<Predicate> initPreds = initNode.GetState().GetPredicates();
            List<Predicate> nodePreds = this.state.GetPredicates();

            foreach (Predicate p in nodePreds)
            {
                if (!ContainsPredicate(initPreds, p))
                    return false;
            }

            return true;
        }

        public bool ContainsPredicate(List<Predicate> pList, Predicate p)
        {
            foreach (Predicate p_list in pList)
            {
                if (p.Equals(p_list))
                    return true;
            }
            return false;
        }

        public bool HasSameState(Node other)
        {
            var thisPreds = this.GetState().GetPredicates();
            var otherPreds = other.GetState().GetPredicates();

            if (thisPreds.Count != otherPreds.Count)
                return false;

            foreach (Predicate p in thisPreds)
            {
                if (!otherPreds.Any(a => a.Equals(p)))
                    return false;
            }

            return true;
        }

        public bool HasSameGoals(Node other)
        {
            if (unsatisfiedGoals.Count != other.GetUnsatisfiedGoals().Count)
                return false;

            foreach (Predicate g in unsatisfiedGoals)
            {
                if (!other.GetUnsatisfiedGoals().Any(a => a.Equals(g)))
                    return false;
            }
            return true;
        }

        // ------------------------- Console Print -------------------------
        public void Print()
        {
            string s = "";

            s += "================================== " +
            $"<b><color=#00FFFF>ANALYSIS - {(action != null ? action.ToString() : "ROOT")}</color></b> " +
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

            //s += "\n<color=#AAAAAA>-----------------</color> " +
                //"<b><color=#1E90FF>Current State</color></b> " +
                 //"<color=#AAAAAA>-------------------</color>\n";
            //if (state != null)
            //    s += state.ToString() + "\n";

            s += "\n-----------------" +
                 "<b><color=#1E90FF>Unsatisfied Goals</color></b> " +
                 "-------------------\n";
            if (unsatisfiedGoals != null)
            {
               foreach (Predicate p in unsatisfiedGoals)
                    s += p.ToString() + "\n";
            }

            s += $"\n<b><color=#FF69B4>Remaining Goals:</color></b> {unsatisfiedGoals.Count}\n";

            s += "\n<color=#AAAAAA>==================================</color> " +
                 "<b><color=#00FFFF>END ANALYSIS</color></b> " +
                 "<color=#AAAAAA>===================================</color>";

            Log(s);
        }


        void Log(string msg) => Debug.Log($"{msg}");

        public void PrintGoals()
        {
            foreach (Predicate p in unsatisfiedGoals)
            {
                Log(p.ToString());
            }
        }

        public void RemoveGoal(Predicate goal)
        {
            unsatisfiedGoals.Remove(goal);
        }

        public void SetState(WorldState state)
        {
            this.state = state;
        }

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
        public WorldState GetState() => state;
        public PlanAction GetAction() => action;
        public Node GetParent() => parent;
        public List<Predicate> GetUnsatisfiedGoals() => unsatisfiedGoals;

        // ------------------------- TXT Logging -------------------------
        private void LogToFile(string text)
        {
            File.AppendAllText(logPath, text + "\n");
        }

        public void PrintToFile()
        {
            var sb = new StringBuilder();

            sb.AppendLine("================================== ANALYSIS ====================================");
            sb.AppendLine($"Depth: {depth}");
            sb.AppendLine("------------------ Previous Action ----------------- ");
            if (parent != null && parent.GetAction() != null) sb.AppendLine(parent.GetAction().ToString());

            sb.AppendLine("------------------ Current Action ----------------- ");
            if (action != null) sb.AppendLine(action.ToString());

            sb.AppendLine("----------------- Current State ------------------- ");
            if (state != null)
                sb.AppendLine(state.ToString());

            sb.AppendLine("----------------- Unsatisfied Goals ------------------- ");
            if (unsatisfiedGoals != null)
            {
                foreach (Predicate p in unsatisfiedGoals)
                    sb.AppendLine(p.ToString());
            }

            sb.AppendLine($"Remaining Goals: {unsatisfiedGoals.Count}");
            sb.AppendLine("================================== END ANALYSIS ====================================");
            sb.AppendLine();

            LogToFile(sb.ToString());
        }
    }
}
