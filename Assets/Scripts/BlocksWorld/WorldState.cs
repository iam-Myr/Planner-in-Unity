using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.Rendering.VolumeComponent;

public class WorldState 
{
    protected List<SharedDelegate> atoms = new List<SharedDelegate>();

    public WorldState AddAtoms(params SharedDelegate[] args)
    {
        atoms.AddRange(args);
        return this;
    }

    public WorldState RemoveAtoms(params SharedDelegate[] args)
    {
        foreach (var atom in args)
        {
            atoms.Remove(atom);
        }
        return this;
    }

    public void Print()
    {
        foreach (SharedDelegate sP in atoms)
        {
            Debug.Log($"{sP.func.Method.Name}({string.Join(", ", sP.args.Select(a => a?.value?.ToString() ?? "null"))})");
        }
    }

    public bool ContainsAtom(SharedDelegate atom) => atoms.Contains(atom);

    public List<SharedDelegate> GetAtoms() => atoms;

}
