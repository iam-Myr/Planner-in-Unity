using UnityEngine;
using System.Collections.Generic;

public class GoalSated : WorldState
{
    public GoalSated()
    {
        AddPredicates(
            new Predicate(Domain.isHungry, new List<Pointer> { }, false)
        );
    }
}
