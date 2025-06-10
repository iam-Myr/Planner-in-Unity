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

}
