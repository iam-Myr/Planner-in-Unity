using UnityEngine;
using System.Collections.Generic;
using Planning;

namespace SimWorld
{
    public class GoalRested : WorldState
    {
        public GoalRested()
        {
            AddPredicates(
                new Predicate(SimDomain.isSleepy, new List<Pointer> { }, false)
            );
        }
    }
}
