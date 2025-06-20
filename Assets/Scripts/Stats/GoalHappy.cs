using UnityEngine;
using System.Collections.Generic;

public class GoalHappy : WorldState
{
    public GoalHappy()
    {
        AddPredicates(
           new Predicate(SimDomain.isThirsty, new List<object> { }, false),
           new Predicate(SimDomain.isHungry, new List<object> { }, false),
           new Predicate(SimDomain.isSleepy, new List<object> { }, false)
       );
    }
}
