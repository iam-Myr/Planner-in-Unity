using System.Collections.Generic;
using UnityEngine;
using System;

public static class Domain
{
    // Actions
    public static List<PlanAction> ActionTemplates = new List<PlanAction>
    {
        new ActionMoveTo(),
        new ActionSleep(),
        new ActionEat(),
        new ActionDrink()
    };

    // Goals
    public static List<WorldState> goalList = new List<WorldState>
    {
        //new GoalRested(),
        //new GoalSated(),
        //new GoalHydrated(),
        new GoalHappy()
    };

    // Objects
    public static Pointer Food = new Pointer(GameObject.Find("Food").GetComponent<Area>());
    public static Pointer Water = new Pointer(GameObject.Find("Water").GetComponent<Area>());
    public static Pointer Sleep = new Pointer(GameObject.Find("Sleep").GetComponent<Area>());
    public static Pointer Spawn = new Pointer(GameObject.Find("Spawn").GetComponent<Area>());


    public static List<Pointer> AllPointers = new List<Pointer> { Food, Water, Sleep, Spawn };

    // Atoms
    public static bool isSleepy(List<object> args)
    {

        if (args[0] is float sleep && args[1] is float threshold)
            return sleep < threshold;

        throw new ArgumentException("isSleepy wrong args.");
    }

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


    public static bool isHungry(List<object> args)
    {
        if (args[0] is float hungry && args[1] is float threshold)
            return hungry < threshold;

        throw new ArgumentException("isHungry wrong args.");
    }

    public static bool isThirsty(List<object> args)
    {
        if (args[0] is float water && args[1] is float threshold)
            return water < threshold;

        throw new ArgumentException("isThirsty wrong args.");
    }

    public static bool isAt(List<object> args)
    {
        if (args[0] is Sim sim && args[1] is Area area)
            return area.Contains(sim.transform);

        throw new ArgumentException("isAt wrong args.");
    }
}
