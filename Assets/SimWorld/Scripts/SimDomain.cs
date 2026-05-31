using System.Collections.Generic;
using UnityEngine;
using System;
using Planning;
using static UnityEngine.RuleTile.TilingRuleOutput;

namespace SimWorld
{
    public static class SimDomain
    {
        // Should be Objects
        public static Pointer FoodArea = new Pointer(GameObject.Find("Food").GetComponent<PlanObject>());
        public static Pointer WaterArea = new Pointer(GameObject.Find("Water").GetComponent<PlanObject>());
        public static Pointer SleepArea = new Pointer(GameObject.Find("Sleep").GetComponent<PlanObject>());
        public static Pointer SpawnArea = new Pointer(GameObject.Find("Spawn").GetComponent<PlanObject>());
        public static Pointer Dummy = new Pointer(42); // To test that dummy is not used!

        public static List<Pointer> AllPointers = new List<Pointer> {FoodArea, WaterArea, SleepArea, SpawnArea, Dummy};

        public static SimAgent agent = GameObject.Find("Agent").GetComponent<SimAgent>();

        // Predicates
        // Domain declares them
        // Different Planning Objects give their conditions
        // Actions give args and value
        public static Predicate isAt = new Predicate("isAt");
        public static Predicate isHungry = new Predicate("isHungry");
        public static Predicate isSleepy = new Predicate("isSleepy");
        public static Predicate isThirsty = new Predicate("isThirsty");


        static SimDomain()
        {
            isAt.SetCondition(isAtCondition);
            isHungry.SetCondition(isHungryCondition);
            isSleepy.SetCondition(isSleepyCondition);
            isThirsty.SetCondition(isThirstyCondition);
        }

        // Conditions
        public static bool isAtCondition(List<object> args)
        {
            if (args.Count == 0 || !(args[0] is Area a))
                return false;
            return agent.isAt(a);
        }

        public static bool isHungryCondition(List<object> args)
        {
            return agent.isHungry();
        }

        public static bool isSleepyCondition(List<object> args)
        {
            return agent.isSleepy();
        }

        public static bool isThirstyCondition(List<object> args)
        {
            return agent.isThirsty();
        }

        // Goals
        public static List<WorldState> GetGoals()
        {
            return new List<WorldState>
            {
                new GoalHappy()
            };
        }

    }
}
