using UnityEngine;
using System.Collections.Generic;

public class GoalHydrated : WorldState
{
    public GoalHydrated()
    {
        AddPredicates(
            new Predicate(SimDomain.isThirsty, new List<object> { }, false)
        );
    }
}
