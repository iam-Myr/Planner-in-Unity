using System;
using System.Collections.Generic;

public class Pointer
{
    public object value; // Can be a direct value or another Pointer (logical alias)

    public Pointer() { }


    /// Gets the final value by recursively resolving any Pointer chain.
    /// Detects cycles to prevent infinite loops.
    public object Get()
    {
        return Get(new HashSet<Pointer>());
    }

    // Internal recursive version with visited tracking to prevent cycles
    private object Get(HashSet<Pointer> visited)
    {
        if (visited.Contains(this))
            throw new InvalidOperationException("Cycle detected in pointer chain.");

        visited.Add(this);

        if (value is Pointer p)
            return p.Get(visited); // Continue resolving chain

        return value; // Final concrete value
    }

    /// Sets the final value, resolving references if necessary.
    /// Fails if a cycle would be created.
    public void Set(object val)
    {
        Set(val, new HashSet<Pointer>());
    }

    // Internal recursive version with visited tracking to prevent cycles
    private void Set(object val, HashSet<Pointer> visited)
    {
        if (visited.Contains(this))
            throw new InvalidOperationException("Cycle detected while setting value.");

        visited.Add(this);

        if (value is Pointer p)
            p.Set(val, visited); // Delegate down the chain
        else
            value = val; // Assign directly
    }

    /// Checks whether the Pointer is bound to a concrete value.
    public bool IsBound()
    {
        return Get() != null;
    }

    /// Checks whether this Pointer currently holds another Pointer (alias).
    public bool IsPointer()
    {
        return value is Pointer;
    }

    /// Unifies this Pointer with another, making them aliases.
    /// Handles value consistency and prevents cycles.
    public void BindTo(Pointer other)
    {
        if (this == other) return; // Already the same

        if (CreatesCycle(other))
            throw new InvalidOperationException("Binding would create a cycle.");

        if (!this.IsBound())
        {
            // This is free: point to other
            value = other;
        }
        else if (!other.IsBound())
        {
            // Other is free: point it to this
            other.value = this;
        }
        else if (!object.Equals(this.Get(), other.Get()))
        {
            // Both have values, but they conflict
            throw new InvalidOperationException("Conflict during unification.");
        }
    }

    /// Helper to check whether binding this Pointer to another would create a cycle.
    private bool CreatesCycle(Pointer other)
    {
        var visited = new HashSet<Pointer>();
        Pointer current = other;

        while (current is not null)
        {
            if (current == this)
                return true; // Cycle would be created
            if (visited.Contains(current))
                return false; // Already visited, no cycle from here

            visited.Add(current);

            if (current.value is Pointer next)
                current = next;
            else
                break;
        }

        return false;
    }


    /// Checks whether two Pointers ultimately resolve to the same value.
    public bool isSameValue(Pointer p) =>
        object.Equals(this.Get(), p.Get());

    /// Creates a fresh, unbound logical variable (deep copy without value).
    public Pointer Clone() => new Pointer();

    /// Returns the resolved value as a string for debugging.
    public override string ToString() => Get()?.ToString() ?? "null";
}
