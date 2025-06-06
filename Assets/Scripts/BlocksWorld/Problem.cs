using System.Collections.Generic;
using UnityEngine;

public static class Problem
{
    // Block GameObjects
    private static Block blockA, blockB, blockC, blockD, blockE;

    // Pointers created from those blocks
    public static Pointer A { get; private set; }
    public static Pointer B { get; private set; }
    public static Pointer C { get; private set; }
    public static Pointer D { get; private set; }
    public static Pointer E { get; private set; }

    public static List<Pointer> AllPointers { get; private set; }

    public static WorldState InitialState { get; private set; }
    public static WorldState GoalState { get; private set; }

    public static void InitFromScene()
    {
        // Find the blocks by name or tag
        blockA = GameObject.Find("A").GetComponent<Block>();
        blockB = GameObject.Find("B").GetComponent<Block>();
        blockC = GameObject.Find("C").GetComponent<Block>();
        blockD = GameObject.Find("D").GetComponent<Block>();
        blockE = GameObject.Find("E").GetComponent<Block>();

        // Create pointers
        A = new Pointer(blockA);
        B = new Pointer(blockB);
        C = new Pointer(blockC);
        D = new Pointer(blockD);
        E = new Pointer(blockE);

        AllPointers = new List<Pointer> { A, B, C, D, E };

        // Set initial and goal state
        InitialState = new WorldState().AddPredicates(
            new Predicate(Domain.isClear, new List<Pointer> { B }, true),
            new Predicate(Domain.isClear, new List<Pointer> { C }, true),
            new Predicate(Domain.isClear, new List<Pointer> { E }, true),
            new Predicate(Domain.isOn, new List<Pointer> { B, A }, true),
            new Predicate(Domain.isOn, new List<Pointer> { A, D }, true),
            new Predicate(Domain.isHandEmpty, new List<Pointer>(), true)
        );

        GoalState = new WorldState().AddPredicates(
            new Predicate(Domain.isOn, new List<Pointer> { B, C }, true),
            new Predicate(Domain.isOn, new List<Pointer> { A, B }, true)
        );
    }
}
