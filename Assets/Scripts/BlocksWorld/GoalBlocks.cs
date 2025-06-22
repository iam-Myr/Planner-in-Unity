using UnityEngine;
using System.Collections.Generic;
using Planning;

namespace BlocksWorld
{

    public class GoalBlocks : WorldState
    {
        public GoalBlocks()
        {
            AddPredicates(
                new Predicate(BlockDomain.isOn, new List<Pointer> { BlockDomain.B, BlockDomain.A }, true),
                new Predicate(BlockDomain.isOn, new List<Pointer> { BlockDomain.A, BlockDomain.B }, true)
            );
        }
    }
}
