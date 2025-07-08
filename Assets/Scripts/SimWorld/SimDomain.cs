using System.Collections.Generic;
using UnityEngine;
using System;
using Planning;

namespace SimWorld
{
    public static class SimDomain
    {
        // Should be Objects
        public static Pointer FoodArea = new Pointer(GameObject.Find("Food").GetComponent<PlanObject>());
        public static Pointer WaterArea = new Pointer(GameObject.Find("Water").GetComponent<PlanObject>());
        public static Pointer SleepArea = new Pointer(GameObject.Find("Sleep").GetComponent<PlanObject>());
        public static Pointer SpawnArea = new Pointer(GameObject.Find("Spawn").GetComponent<PlanObject>());

        public static List<Pointer> AllPointers = new List<Pointer> {FoodArea, WaterArea, SleepArea, SpawnArea};

        // Predicates
        // Domain declares them
        // Different Planning Objects give their conditions
        // Actions give args and value
        public static Predicate isAt = new Predicate();
        public static Predicate isHungry = new Predicate();
        public static Predicate isSleepy = new Predicate();
        public static Predicate isThirsty = new Predicate();


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
