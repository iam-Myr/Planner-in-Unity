using UnityEngine;
using System.Collections.Generic;
using Planning;

namespace SimWorld
{
    public class GoalSated : WorldState
    {
        public GoalSated()
        {
            AddPredicates(
                new Predicate(SimDomain.isHungry, new List<Pointer> { }, false)
            );
        }
    }
}
