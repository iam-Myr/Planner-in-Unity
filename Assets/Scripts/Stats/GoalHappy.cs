using UnityEngine;
using System.Collections.Generic;

public class GoalHappy : WorldState
{
    public GoalHappy()
    {
        AddPredicates(
           new Predicate(Domain.isThirsty, new List<Pointer> { }, false),
           new Predicate(Domain.isHungry, new List<Pointer> { }, false),
           new Predicate(Domain.isSleepy, new List<Pointer> { }, false)
       );
    }

}
