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


        // OTHER dictionary idea
        // dict <string, func>


        // Predicates

        public static Predicate isAt;
        public static Predicate isHungry;
        public static Predicate isSleepy;
        public static Predicate isThirsty;


        //public static Dictionary<string, Predicate> Predicates = new Dictionary<string, Predicate> 
        //{"isAt": new Predicate() }
> 




        // Dictionary Idea
        /*
        public static bool isSleepy(Dictionary<string, object> args)
        {
            if (args.TryGetValue("sleep", out var sleepObj) &&
                args.TryGetValue("threshold", out var thresholdObj) &&
                sleepObj is float sleep &&
                thresholdObj is float threshold)
            {
                return sleep < threshold;
            }

            throw new ArgumentException("Invalid arguments for isSleepy");
        } */

      

        // Goals
        public static List<WorldState> goalList = new List<WorldState>
    {
        //new GoalRested(),
        //new GoalSated(),
        //new GoalHydrated(),
        new GoalHappy()
    };
    }
}
