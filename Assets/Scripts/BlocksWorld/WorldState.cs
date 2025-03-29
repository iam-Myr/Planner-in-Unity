using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class WorldState : MonoBehaviour
{
    private WorldState previousState; // TURN THIS INTO TREE NODE
    // List of atoms owo?
    // KeyValuPair is like isClear(), [Block A]
    private List<KeyValuePair<Func<bool>, Block[]>> atoms { get;}

    public WorldState(List<KeyValuePair<Func<bool>, Block[]>> atoms)
    {
        this.atoms = atoms;
    }

    // State is goal if the goal's atoms are a subset of these atoms
    public bool isGoal(WorldState goalState)
    {
        return goalState.atoms.All(i => atoms.Contains(i));
    }

    public void ApplyAction(Action action)
    {
       
    }
}
