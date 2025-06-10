using UnityEngine;
using System.Collections.Generic;

public class GoalBlocks : WorldState
{
    public GoalBlocks()
    {
        AddPredicates(
            new Predicate(BlockDomain.isOn, new List<Pointer> {BlockProblem.B, BlockProblem.E}, true),
            new Predicate(BlockDomain.isOn, new List<Pointer> {BlockProblem.A, BlockProblem.B}, true)
        );
    }
}
