using System.Collections.Generic;
using SysDiag = System.Diagnostics;
using Unity.VisualScripting;
using UnityEngine;


namespace Planning
{
    public static class Unification
    {

        public static bool Unify(object x, object y)//, List<object> banned_x, List<object> banned_y)
        {
            // Both null or same value
            if (x == null && y == null) return true;
            if (x == null || y == null) return false;

            // If they are both equal primitive values
            if (x.Equals(y)) return true;

            // VARIABLE?(x) y needs to be valid
            if (x is Pointer px) return Unify_Pointer(px, y);

            // VARIABLE?(y) x needs to be valid
            if (y is Pointer py) return Unify_Pointer(py, x);

            // COMPOUND?(x) and COMPOUND?(y)
            if (x is Predicate p1 && y is Predicate p2)
            {
                // if different names, arg countss, polarities -> fail
                if (!(p1.Name.Equals(p2.Name)) || (p1.Args.Count != p2.Args.Count))
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

                
                //Debug.Log("Unified predicates: " + p1.ToString() + " and " + p2.ToString());
                return true;
            }

            // Failure
            return false;
        }

        private static bool Unify_Pointer(Pointer point, object x)
        {
            // if var already bound -> try unify its value with x
            if (point.IsBound()) return Unify(point.Get(), x);

            // if x is a pointer already bound -> try unify point with x's value
            if (x is Pointer px && px.IsBound()) return Unify(point, px.Get());

            // BAN CHECK
            point.PrintBanList();
            Debug.Log($"Is {x} banned?");
            if (point.IsBanned(x))
                return false;

            // if point not bound, bind to x (either to pointer or to value)
            // THIS IS WHERE BINDING HAPPENS
            // EITHER BIND TO POINTER OR VALUE
            if (x is Pointer p)
            { // and x is valid 
                //Debug.Log($"<color=ORANGE>Binding pointer {point.Name} to POINTER {p.Name}</color>");
                point.BindTo(p); //if x is an unbound pointer, bind to it
            }
            else
            {
                Debug.Log($"<color=YELLOW>Binding pointer {point.Name} to VALUE {x}</color>");
                point.Set(x); // else set pointer value to x
            }
            return true;
        }
    }
}