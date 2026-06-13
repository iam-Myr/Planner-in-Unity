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
               SimDomain.isThirsty.Instantiate(new List<Pointer> {}, false),
               SimDomain.isHungry.Instantiate(new List<Pointer> {}, false),
               SimDomain.isSleepy.Instantiate(new List<Pointer> {}, false)
           );
        }
    }
}
