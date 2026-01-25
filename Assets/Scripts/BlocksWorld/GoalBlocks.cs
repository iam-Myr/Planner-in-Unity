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
                BlockDomain.isOn.Instantiate(new List<Pointer> { BlockDomain.A, BlockDomain.B }, true)
            );
        }
    }
}
