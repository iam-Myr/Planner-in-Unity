using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;

public class WorldState 
{
    protected List<(Func<object[], bool>, object[])> atoms = new List<(Func<object[], bool>, object[])>();

    // State is goal if the goal's atoms are a subset of current state atoms
    //public bool isGoal(WorldState goalState)
    //{
    //    return goalState.atoms.All(i => atoms.Contains(i));
    // }

    public WorldState AddAtoms(params (Func<object[], bool>, object[])[] args)
    {
        atoms.AddRange(args);
        return this;
    }
    public List<(Func<object[], bool>, object[])> GetAtoms() => atoms;

}
