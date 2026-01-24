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
        private PlanAction action; // Action that got us here
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
            Debug.Log("================================== ANALYSIS ====================================");
            Debug.Log($"Depth: {depth}");
            Debug.Log("------------------ Previous Action ----------------- ");
            if(parent != null && parent.GetAction()!=null) Debug.Log($"{parent.GetAction()}");
            Debug.Log("------------------ Current Action ----------------- ");
            if (action != null) Debug.Log(action);
            Debug.Log("----------------- Current State ------------------- ");
            if (state != null) state.Print();
            Debug.Log("----------------- Unsatisfied Goals ------------------- ");
            if (unsatisfiedGoals != null) PrintGoals();
            Debug.Log($"Remaining Goals: {unsatisfiedGoals.Count}");
            Debug.Log("================================== END ANALYSIS ====================================");
        }

        public void PrintGoals()
        {
            foreach (Predicate p in unsatisfiedGoals)
            {
                Debug.Log(p.ToString());
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
