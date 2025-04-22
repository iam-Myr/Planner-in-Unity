using System;
using System.Collections.Generic;

public class Pointer
{
    public object value; // can be a direct value OR another Pointer (alias)

    public object Get()
    {
        return Get(new HashSet<Pointer>());
    }

    private object Get(HashSet<Pointer> visited)
    {
        if (visited.Contains(this))
            throw new InvalidOperationException("Cycle detected in pointer chain.");

        visited.Add(this);

        if (value is Pointer p)
            return p.Get(visited);

        return value;
    }


    public void Set(object val)
    {
        Set(val, new HashSet<Pointer>());
    }

    private void Set(object val, HashSet<Pointer> visited)
    {
        if (visited.Contains(this))
            throw new InvalidOperationException("Cycle detected while setting value.");

        visited.Add(this);

        if (value is Pointer p)
            p.Set(val, visited);
        else
            value = val;
    }


    public bool IsBound()
    {
        return Get() != null;
    }

    public bool IsPointer()
    {
        return value is Pointer;
    }

    public void BindTo(Pointer other)
    {
        if (this == other) return;

        if (CreatesCycle(other))
            throw new InvalidOperationException("Binding would create a cycle.");

        if (!this.IsBound())
            value = other;
        else if (!other.IsBound())
            other.value = this;
        else if (!object.Equals(this.Get(), other.Get()))
            throw new InvalidOperationException("Conflict during unification.");
    }

    // Helper to check if binding this → other would create a cycle
    private bool CreatesCycle(Pointer other)
    {
        var visited = new HashSet<Pointer>();
        Pointer current = other;

        while (current is not null)
        {
            if (visited.Contains(current))
                return true;
            visited.Add(current);

            if (current.value is Pointer next)
                current = next;
            else
                break;
        }

        return false;
    }


    public bool isSameValue(Pointer p) =>
    object.Equals(this.Get(), p.Get());


    public Pointer Clone() => new Pointer(); // fresh logical variable, no value

    public override string ToString() => Get()?.ToString() ?? "null";
}
