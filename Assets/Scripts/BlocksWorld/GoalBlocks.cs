using UnityEngine;
using System.Collections.Generic;

public class GoalBlocks : WorldState
{
    public GoalBlocks()
    {
        AddPredicates(
            new Predicate(BlockDomain.isOn, new List<Pointer> {BlockDomain.B, BlockDomain.E}, true),
            new Predicate(BlockDomain.isOn, new List<Pointer> {BlockDomain.A, BlockDomain.B}, true)
        );
    }
}
