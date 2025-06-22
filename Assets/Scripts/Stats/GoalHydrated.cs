using UnityEngine;
using System.Collections.Generic;
using Planning;

namespace SimWorld
{
    public class GoalHydrated : WorldState
    {
        public GoalHydrated()
        {
            AddPredicates(
                new Predicate(SimDomain.isThirsty, new List<Pointer> { }, false)
            );
        }
    }
}
