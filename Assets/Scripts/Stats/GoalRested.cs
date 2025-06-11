using UnityEngine;
using System.Collections.Generic;

public class GoalRested : WorldState
{
    public GoalRested()
    {
        AddPredicates(
            new Predicate(Domain.isSleepy, new List<Pointer> {}, false)
        );
    }
}
