using System.Collections.Generic;


// IS SINGLETON?
public static class Domain
{
    // Actions
    public static List<Action> ActionTemplates = new List<Action>
        {
        new ActionMoveTo(),
        new ActionSleep()
        };

    // Predicates
    public static bool isAt(object[] args)
    {
        //Area area = args[0] as Area;
        return true;
    }

    public static bool isSleepy(object[] args)
    {
        return true;
    }
}

