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

        //  Predicates (declared once, logic handled in Block/Agent) 
        public static Predicate isClear = new Predicate();
        public static Predicate isOn = new Predicate(); //isOn(A,B) A is on B
        public static Predicate isHolding = new Predicate();
        public static Predicate isHandEmpty = new Predicate();

        //  Domain actions 
        public static List<PlanAction> ActionTemplates = new List<PlanAction>
        {
            new ActionMove(),
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

       
        public static bool IsOfType<T>(object obj)
        {
            return obj is T;
        }
    }
}
