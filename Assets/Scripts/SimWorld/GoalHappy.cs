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
               SimDomain.isThirsty.CreateNew(new List<Pointer> {}, false),
               SimDomain.isHungry.CreateNew(new List<Pointer> {}, false),
               SimDomain.isSleepy.CreateNew(new List<Pointer> {}, false)
           );
        }
    }
}
