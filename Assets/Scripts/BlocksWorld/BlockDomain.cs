using System.Collections.Generic;


// IS SINGLETON?
public static class BlockDomain
{
    // Actions
    public static List<Action> ActionTemplates = new List<Action>
        {
        new ActionMove(),
        new ActionPickup(),
        new ActionDrop()
        };

    // Predicates
    public static bool isClear(object[] args)
    {
        Block block = args[0] as Block;
        return block != null && block.isClear();
    }

    public static bool isOn(object[] args)
    {
        return true;
    }

    public static bool isHolding(object[] args)
    {
        return true;
    }

    public static bool isHandEmpty(object[] args)
    {
        return true;
    }
}

