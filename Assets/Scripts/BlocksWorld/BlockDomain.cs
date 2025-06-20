using System.Collections.Generic;
using UnityEngine;



// IS SINGLETON?
public static class BlockDomain
{
    

    // Pointers created from blocks
    public static Block A = GameObject.Find("A").GetComponent<Block>();
    public static Block B = GameObject.Find("B").GetComponent<Block>();
    public static Block C = GameObject.Find("C").GetComponent<Block>();
    public static Block D = GameObject.Find("D").GetComponent<Block>();
    public static Block E = GameObject.Find("E").GetComponent<Block>();

    public static List<object> AllObjects = new List<object> { A, B, C, D, E };

    public static List<Predicate> InitialState = new List<Predicate> {
            new Predicate(BlockDomain.isClear, new List<object> { B }, true),
            new Predicate(BlockDomain.isClear, new List<object> { C }, true),
            new Predicate(BlockDomain.isClear, new List<object> { E }, true),
            new Predicate(BlockDomain.isOn, new List<object> { B, A }, true),
            new Predicate(BlockDomain.isOn, new List<object> { A, D }, true),
            new Predicate(BlockDomain.isHandEmpty, new List<object>(), true)
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


    // Actions
    public static List<PlanAction> ActionTemplates = new List<PlanAction>
    {
        new ActionMove(),
        new ActionPickup(),
        new ActionDrop()
    };

    public static List<WorldState> goalList = new List<WorldState>
    {
        new GoalBlocks()
    };
}

