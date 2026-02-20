using UnityEngine;
using System.Collections.Generic;

public class GoalBlocks : WorldState
{
    public GoalBlocks()
    {
<<<<<<< Updated upstream
        AddPredicates(
            new Predicate(BlockDomain.isOn, new List<object> {BlockDomain.B, BlockDomain.E}, true),
            new Predicate(BlockDomain.isOn, new List<object> {BlockDomain.A, BlockDomain.B}, true)
        );
=======
        public GoalBlocks()
        {
            AddPredicates(
                BlockDomain.isOn.Instantiate(new List<Pointer> { BlockDomain.A, BlockDomain.E}, true)
            );
        }
>>>>>>> Stashed changes
    }
}
