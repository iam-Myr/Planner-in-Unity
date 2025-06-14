using UnityEngine;
using System.Collections.Generic;

public class GoalRested : WorldState
{
    public GoalRested()
    {
        AddPredicates(
            new Predicate(SimDomain.isSleepy, new List<Pointer> {}, false)
        );
    }
}
