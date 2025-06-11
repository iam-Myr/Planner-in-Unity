using System.Collections.Generic;


// IS SINGLETON?
public static class BlockDomain
{
    // Actions
    public static List<PlanAction> ActionTemplates = new List<PlanAction>
        {
        new ActionMove(),
        new ActionPickup(),
        new ActionDrop()
        };

    // Predicates
    public static bool isClear(List<object> args)
    {
        return true;
    }

    public static bool isOn(List<object> args)
    {
        return true;
    }

    public static bool isHolding(List<object> args)
    {
        return true;
    }

    public static bool isHandEmpty(List<object> args)
    {
        return true;
    }
}

