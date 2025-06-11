using UnityEngine;
using System.Collections.Generic;

public class GoalHydrated : WorldState
{
    public GoalHydrated()
    {
        AddPredicates(
            new Predicate(Domain.isThirsty, new List<Pointer> { }, false)
        );
    }
}
