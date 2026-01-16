using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Planning
{
    public class WorldState
    {
        // CHANGE THIS INTO HASH TABLE
        protected List<Predicate> knowledge_base = new List<Predicate>();

        // Add Atoms if they're not already contained
        public WorldState AddPredicates(params Predicate[] args)
        {
            foreach (Predicate p in args)
            {
                // Avoid adding duplicates (based on isSame)
                if (!knowledge_base.Any(existing => existing.Equals(p)))
                {
                    knowledge_base.Add(p);
                }
            }
            return this;
        }

        public WorldState RemovePredicates(params Predicate[] args)
        {
            foreach (var atom in args)
            {
                knowledge_base.RemoveAll(p => p.Equals(atom));
            }
            return this;
        }

        /*
        public bool IsContradiction()
        {
            for (int i = 0; i < predicates.Count; i++)
            {
                var p1 = predicates[i];
                if (p1.func.Method.Name != "isOn") continue;
                var a1 = p1.args[0].value;
                var b1 = p1.args[1].value;

                for (int j = i + 1; j < predicates.Count; j++)
                {
                    var p2 = predicates[j];
                    if (p2.func.Method.Name != "isOn") continue;
                    var a2 = p2.args[0].value;
                    var b2 = p2.args[1].value;

                    if (a1 != null && b1 != null && a2 != null && b2 != null &&
                        a1.Equals(b2) && b1.Equals(a2))
                    {
                        return true; // Found on(A,B) and on(B,A)
                    }
                }
            }

            return false;
        } 

        public void UnifyWith(WorldState other)
        {
            foreach (Predicate p in this.predicates)
            {
                foreach (Predicate otherP in other.predicates)
                {
                    if (Unification.CanUnify(p, otherP))
                    {
                        Unification.Unify(p, otherP);
                    }
                }
            }
        } */

        public void Print()
        {
            foreach (Predicate sP in knowledge_base)
            {
                Debug.Log($"{sP.ToString()}");
            }
        }

        public bool ContainsAtom(Predicate atom) => knowledge_base.Contains(atom);

        public List<Predicate> GetPredicates() => knowledge_base;

    }
}
