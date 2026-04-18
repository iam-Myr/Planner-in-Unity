using System.Collections.Generic;
using SysDiag = System.Diagnostics;
using Unity.VisualScripting;
using UnityEngine;
using System.Drawing;
using System.Linq;


namespace Planning
{
    public static class Unification
    {

        public static Dictionary<Pointer, object> TryUnify(Predicate p1, Predicate p2)
        {
            Dictionary<Pointer, object> theta = new Dictionary<Pointer, object>();
            return TryUnifyInternal(p1, p2, theta);
        }

        private static Dictionary<Pointer, object> TryUnifyInternal(object x, object y, Dictionary<Pointer, object> theta)//, List<object> banned_x, List<object> banned_y)
        {
            // Both null or same value
            if (x == null && y == null) return theta;
            if (x == null || y == null) return null;

            // If they are both equal primitive values
            if (x.Equals(y)) return theta;

            // VARIABLE?(x) y needs to be valid
            if (x is Pointer px) return TryUnifyPointer(px, y, theta);

            // VARIABLE?(y) x needs to be valid
            if (y is Pointer py) return TryUnifyPointer(py, x, theta);

            // COMPOUND?(x) and COMPOUND?(y)
            if (x is Predicate p1 && y is Predicate p2)
            {
                // if different names, arg countss, polarities -> fail
                if (!(p1.Name.Equals(p2.Name)) || (p1.Args.Count != p2.Args.Count))
                    return null;

                if (p1.Value.HasValue && p2.Value.HasValue &&
                    p1.Value.Value != p2.Value.Value)
                    return null;

                // unify arguments recursively
                for (int i = 0; i < p1.Args.Count; i++)
                {
                    if (TryUnifyInternal(p1.Args[i], p2.Args[i], theta) == null)
                        return null;
                }


                //Debug.Log("Unified predicates: " + p1.ToString() + " and " + p2.ToString());
                return theta;
            }

            // Failure
            return null;
        }

        private static Dictionary<Pointer, object> TryUnifyPointer(Pointer point, object x, Dictionary<Pointer, object> theta)
        {
            // if var already bound -> try unify its value with x
            if (point.IsBound()) return TryUnifyInternal(point.Get(), x, theta);

            // if x is a pointer already bound -> try unify point with x's value
            if (x is Pointer px && px.IsBound()) return TryUnifyInternal(point, px.Get(), theta);

            // BAN CHECK
            //point.PrintBanList();
            //Debug.Log($"Is {x} banned?");
            if (point.IsBanned(x))
                return null;

            if (!theta.ContainsKey(point))
            {
                theta[point] = x;
            }
            return theta;
        }

        public static void Unify(List<Predicate> preds, Dictionary<Pointer, object> theta)
        {
            if (theta == null) return;

            foreach (Predicate pred in preds)
            {
                foreach (Pointer arg in pred.Args)
                {
                    // Only proceed if the pointer exists in theta
                    if (theta.TryGetValue(arg, out object x))
                    {
                        arg.BindTo(x);

                    }
                    // else: skip pointers not in theta
                }
            }
        }
    }

}