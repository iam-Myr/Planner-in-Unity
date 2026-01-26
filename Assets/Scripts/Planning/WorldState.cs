using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

namespace Planning
{
    public class WorldState
    {
        // Dict of type [Func: [Predicate1, Predicate2]... ex. [On: [On(A,B), on(C,D)]
        protected Dictionary<string, List<Predicate>> knowledge_base =
    new Dictionary<string, List<Predicate>>();


        // Add Atoms if they're not already contained
        public WorldState AddPredicates(params Predicate[] args)
        {
            foreach (Predicate p in args)
            {
                if (!knowledge_base.TryGetValue(p.Name, out List<Predicate> list))
                {
                    list = new List<Predicate>();
                    knowledge_base[p.Name] = list;
                }

                if (!list.Any(existing => existing.Equals(p)))
                {
                    list.Add(p);
                }
            }
            return this;
        }

        public WorldState RemovePredicates(params Predicate[] args)
        {
            foreach (Predicate atom in args)
            {
                if (knowledge_base.TryGetValue(atom.Name, out List<Predicate> list))
                {
                    list.RemoveAll(p => p.Equals(atom));
                    if (list.Count == 0)
                        knowledge_base.Remove(atom.Name);
                }
            }
            return this;
        }

        public bool ContainsAtom(Predicate atom)
        {
            return knowledge_base.TryGetValue(atom.Name, out List<Predicate> list) && list.Any(p => p.Equals(atom));
        }


        // Return all predicates as a flat list
        public List<Predicate> GetPredicates()
        {
            return knowledge_base.Values.SelectMany(list => list).ToList();
        }

        // Print all predicates
        public void Print()
        {
            foreach (List<Predicate> list in knowledge_base.Values)
            {
                foreach (Predicate p in list)
                {
                    Debug.Log(p.ToString());
                }
            }
        }
    }
}
