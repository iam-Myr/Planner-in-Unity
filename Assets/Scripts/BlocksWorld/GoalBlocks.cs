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
                BlockDomain.isOn.Instantiate(new List<Pointer> { BlockDomain.B, BlockDomain.C}, true)
            );
        }
    }
}
