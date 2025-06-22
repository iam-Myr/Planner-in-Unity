using UnityEngine;
using System.Collections.Generic;
using Planning;

namespace SimWorld
{
    public class GoalHappy : WorldState
    {
        public GoalHappy()
        {
            AddPredicates(
               new Predicate(SimDomain.isThirsty, new List<Pointer> { }, false),
               new Predicate(SimDomain.isHungry, new List<Pointer> { }, false),
               new Predicate(SimDomain.isSleepy, new List<Pointer> { }, false)
           );
        }

    }
}
