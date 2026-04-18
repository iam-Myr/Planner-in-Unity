using System.Collections.Generic;
using UnityEngine;
using Planning;

namespace BlocksWorld
{
    public static class BlockDomain
    {
        //  Pointers for all blocks 
        public static Pointer A = new Pointer(GameObject.Find("A").GetComponent<PlanObject>());
        public static Pointer B = new Pointer(GameObject.Find("B").GetComponent<PlanObject>());
        public static Pointer C = new Pointer(GameObject.Find("C").GetComponent<PlanObject>());
        public static Pointer D = new Pointer(GameObject.Find("D").GetComponent<PlanObject>());
        public static Pointer E = new Pointer(GameObject.Find("E").GetComponent<PlanObject>());

        public static List<Pointer> AllPointers = new List<Pointer> { A, B, C, D, E };

        // The agent
        public static BlockAgent agent = GameObject.Find("Agent").GetComponent<BlockAgent>();

        //  Predicates (declared once, logic handled in Block/Agent) 
        public static Predicate isClear = new Predicate("isClear");
        public static Predicate isOn = new Predicate("isOn"); //isOn(A,B) A is on B
        public static Predicate isHolding = new Predicate("isHolding");
        public static Predicate isHandEmpty = new Predicate("isHandEmpty");

        public static bool isClearCondition(List<object> args)
        {
            if (args.Count != 1) return false;
            if (args[0] is not Block block) return false;

            return block.isClear();
        }

        public static bool isOnCondition(List<object> args)
        {
            if (args.Count != 2) return false;
            if (args[0] is not Block top) return false;
            if (args[1] is not Block bottom) return false;

            return top.isOn(bottom);
        }


        public static bool isHoldingCondition(List<object> args)
        {
            if (args.Count == 0 || !(args[0] is Block block)) return false;
            return agent.isHolding(block);
        }

        public static bool isHandEmptyCondition(List<object> args)
        {
            return agent.isHandEmpty();
        }

        static BlockDomain()
        {
            // Set conditions and constraints for predicates
            isClear.SetCondition(isClearCondition);
            isOn.SetCondition(isOnCondition);
            isHandEmpty.SetCondition(isHandEmptyCondition);
            isHolding.SetCondition(isHoldingCondition);

        }

        //  Domain actions 
        public static List<PlanAction> ActionTemplates = new List<PlanAction>
        {
            //new ActionMove(),
            new ActionPickup(),
            new ActionDrop()
        };

        //  Goals 
        public static List<WorldState> GetGoals()
        {
            return new List<WorldState>
            {
                new GoalBlocks()
            };
        }
    }
}
