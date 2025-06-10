using System.Collections.Generic;
using UnityEngine;

public static class Domain
{
    // Actions
    public static List<Action> ActionTemplates = new List<Action>
        {
        new ActionMoveTo(),
        new ActionSleep()
        };

    // Goals
    public static List<WorldState> goalList = new List<WorldState>
        {
        new GoalRested()
        };

    // Predicates

    public static bool isSleepy(List<object> args)
    {
        if (args[0] is float sleep && args[1] is float t)
            return sleep < t;
        return false;
    }

    public static bool isAt(List<object> args)
    {
        if (args[0] is Agent agent && args[1] is Area area)
        {
            return area.Contains(agent.transform);
        }
        return false;
    }

    // Pointers 
    public static Pointer Food = new Pointer(GameObject.Find("Food").GetComponent<Area>());
    public static Pointer Water = new Pointer(GameObject.Find("Water").GetComponent<Area>());
    public static Pointer Sleep = new Pointer(GameObject.Find("Sleep").GetComponent<Area>());
    public static Pointer Spawn = new Pointer(GameObject.Find("Spawn").GetComponent<Area>());


    public static List<Pointer> AllPointers = new List<Pointer> { Food, Water, Sleep, Spawn };

}
