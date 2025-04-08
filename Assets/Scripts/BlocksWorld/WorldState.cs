using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.Rendering.VolumeComponent;

public class WorldState 
{
    protected List<(Func<object[], bool>, SharedVar[])> atoms = new List<(Func<object[], bool>, SharedVar[])>();

    public WorldState AddAtoms(params (Func<object[], bool>, SharedVar[])[] args)
    {
        atoms.AddRange(args);
        return this;
    }

    public WorldState RemoveAtoms(params (Func<object[], bool>, SharedVar[])[] args)
    {
        foreach (var atom in args)
        {
            atoms.Remove(atom);
        }
        return this;
    }

    public void Print()
    {
        foreach (var (predicate, args) in atoms)
        {
            Debug.Log($"{predicate.Method.Name}({string.Join(", ", args.Select(a => a?.value?.ToString() ?? "null"))})");
        }
    }

    public List<(Func<object[], bool>, SharedVar[])> GetAtoms() => atoms;

}
