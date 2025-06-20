using UnityEngine;
using System.Collections.Generic;

public class GoalSated : WorldState
{
    public GoalSated()
    {
        AddPredicates(
            new Predicate(SimDomain.isHungry, new List<object> { }, false)
        );
    }
}
