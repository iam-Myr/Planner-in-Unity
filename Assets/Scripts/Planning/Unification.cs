using Planning;
using System.Collections.Generic;

public static class Unification
{
    public static bool Unify(object x, object y)
    {
        // Both null or same value
        if (x == null && y == null) return true;
        if (x == null || y == null) return false;

        // If they are both equal primitive values
        if (x.Equals(y)) return true;

        // VARIABLE?(x)
        if (x is Pointer px) return Unify_Pointer(px, y);

        // VARIABLE?(y)
        if (y is Pointer py) return Unify_Pointer(py, x);

        // COMPOUND?(x) and COMPOUND?(y)
        if (x is Predicate p1 && y is Predicate p2)
        {
            // if different names, arg countss, polarities -> fail
            if ((p1.TheFunc.Method.Name != p2.TheFunc.Method.Name) || (p1.Args.Count != p2.Args.Count))
                return false;

            if (p1.Value.HasValue && p2.Value.HasValue &&
                p1.Value.Value != p2.Value.Value)
                return false;


            // unify arguments recursively
            for (int i = 0; i < p1.Args.Count; i++)
            {
                if (!Unify(p1.Args[i], p2.Args[i]))
                    return false;
            }

            return true;
        }

        // Failure
        return false;
    }

    private static bool Unify_Pointer(Pointer point, object x)
    {
        // if var already bound -> unify its value
        if (point.IsBound()) return Unify(point.Get(), x);

        // if x is a pointer already bound -> unify with its value
        if (x is Pointer px && px.IsBound()) return Unify(point, px.Get());

        // bind pointer
        if (x is Pointer p)
            point.BindTo(p);
        else
            point.Set(x);

        return true;
    }
}
