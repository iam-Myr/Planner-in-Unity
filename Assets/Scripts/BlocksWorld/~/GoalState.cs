using System;
using System.Collections.Generic;
using UnityEngine;

public class GoalState : WorldState
{
    public Block B;
    public Block Y;
    public GoalState()
    {
        atoms = new List<(Func<object[], bool>, object[])>
        {(PredicateLibrary.isOn, new object[] {B, Y})};
    }

  
}
