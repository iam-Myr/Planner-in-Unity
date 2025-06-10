using System.Collections.Generic;
using UnityEngine;

// IS SINGLETON?

public static class Problem
{
    // Pointers created from blocks
    public static Pointer Food = new Pointer(GameObject.Find("Food").GetComponent<Area>());
    public static Pointer Water = new Pointer(GameObject.Find("Water").GetComponent<Area>());
    public static Pointer Sleep = new Pointer(GameObject.Find("Sleep").GetComponent<Area>());
    public static Pointer Spawn = new Pointer(GameObject.Find("Spawn").GetComponent<Area>());


    public static List<Pointer> AllPointers = new List<Pointer> { Food, Water, Sleep, Spawn};

    public static WorldState InitialState = new WorldState().AddPredicates(
            new Predicate(Domain.isAt, new List<Pointer> {Spawn}, true),
            new Predicate(Domain.isSleepy, new List<Pointer> {}, true)
        );

    public static List<WorldState> goalList = new List<WorldState>
        {
        new GoalRested()
        };
}
