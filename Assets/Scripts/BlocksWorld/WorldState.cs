using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.Rendering.VolumeComponent;

public class WorldState 
{
    protected List<Predicate> atoms = new List<Predicate>();

    // Add Atoms if they're not already contained
    public WorldState AddPredicates(params Predicate[] args)
    {
        foreach (Predicate p in args)
        {
            // Avoid adding duplicates (based on isSame)
            if (!atoms.Any(existing => existing.isSame(p)))
            {
                atoms.Add(p);
            }
        }
        return this;
    }


    public WorldState RemovePredicates(params Predicate[] args)
    {
        foreach (var atom in args)
        {
            atoms.Remove(atom);
        }
        return this;
    }

    public void Print()
    {
        foreach (Predicate sP in atoms)
        {
            Debug.Log($"{sP.func.Method.Name}({string.Join(", ", sP.args.Select(a => a?.value?.ToString() ?? "null"))})");
        }
    }

    public bool ContainsAtom(Predicate atom) => atoms.Contains(atom);

    public List<Predicate> GetPredicates() => atoms;

}
