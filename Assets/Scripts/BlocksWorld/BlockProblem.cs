using System.Collections.Generic;
using UnityEngine;

// IS SINGLETON?

public static class BlockProblem
{
    // Pointers created from blocks
    public static Pointer A = new Pointer(GameObject.Find("A").GetComponent<Block>());
    public static Pointer B = new Pointer(GameObject.Find("B").GetComponent<Block>());
    public static Pointer C = new Pointer(GameObject.Find("C").GetComponent<Block>());
    public static Pointer D = new Pointer(GameObject.Find("D").GetComponent<Block>());
    public static Pointer E = new Pointer(GameObject.Find("E").GetComponent<Block>());

    public static List<Pointer> AllPointers = new List<Pointer> { A, B, C, D, E};

    public static WorldState InitialState = new WorldState().AddPredicates(
            new Predicate(BlockDomain.isClear, new List<Pointer> { B }, true),
            new Predicate(BlockDomain.isClear, new List<Pointer> { C }, true),
            new Predicate(BlockDomain.isClear, new List<Pointer> { E }, true),
            new Predicate(BlockDomain.isOn, new List<Pointer> { B, A }, true),
            new Predicate(BlockDomain.isOn, new List<Pointer> { A, D }, true),
            new Predicate(BlockDomain.isHandEmpty, new List<Pointer>(), true)
        );

    public static List<WorldState> goalList = new List<WorldState>
        {
        new GoalBlocks()
        };
}
