using UnityEngine;
using System.Collections.Generic;

public class GoalBlocks : WorldState
{
    public GoalBlocks()
    {
        AddPredicates(
            new Predicate(BlockDomain.isOn, new List<object> {BlockDomain.B, BlockDomain.E}, true),
            new Predicate(BlockDomain.isOn, new List<object> {BlockDomain.A, BlockDomain.B}, true)
        );
    }
}
