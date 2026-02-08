using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Planning
{
    public static class ActionGenerator
    {
        /// <summary>
        /// Generate all grounded actions from lifted action templates and available pointers.
        /// </summary>
        public static List<PlanAction> GenerateAllGroundedActions(
            List<PlanAction> actionTemplates, List<Pointer> availablePointers)
        {
            List<PlanAction> groundedActions = new List<PlanAction>();

            foreach (var template in actionTemplates)
            {
                var expectedTypes = template.GetArgTypes();
                int arity = expectedTypes.Count;

                // If action has no args, just clone it
                if (arity == 0)
                {
                    groundedActions.Add(template.CreateNew(new List<Pointer>()));
                    continue;
                }

                // Only unbound pointers need grounding
                var argsToGround = template.actionArgs
                    .Select(p => p.IsBound() ? p : null)
                    .ToList();

                // Get all valid permutations for unbound args
                var pointerPermutations = GetPermutations(
                    availablePointers.Where(p => !p.IsBound()), argsToGround.Count(x => x == null));

                foreach (var perm in pointerPermutations)
                {
                    List<Pointer> newArgs = new List<Pointer>();
                    int permIndex = 0;

                    // Fill in bound args directly, unbound args from permutation
                    foreach (var arg in argsToGround)
                    {
                        if (arg != null)
                            newArgs.Add(arg);
                        else
                            newArgs.Add(perm[permIndex++]);
                    }

                    // Type check
                    bool valid = true;
                    for (int i = 0; i < newArgs.Count; i++)
                    {
                        if (!expectedTypes[i].IsAssignableFrom(newArgs[i].GetDeclaredType()))
                        {
                            valid = false;
                            break;
                        }
                    }

                    if (!valid)
                        continue;

                    // Create grounded action
                    PlanAction grounded = template.CreateNew(newArgs);

                    // Only keep actions that satisfy constraints (e.g., AllDifferent)
                    if (grounded.SatisfiesConstraints())
                        groundedActions.Add(grounded);
                }
            }

            return groundedActions;
        }

        /// <summary>
        /// Get all permutations of a list with specified length, avoiding duplicate items.
        /// </summary>
        private static List<List<T>> GetPermutations<T>(IEnumerable<T> list, int length)
        {
            if (length == 1)
                return list.Select(x => new List<T> { x }).ToList();

            var perms = GetPermutations(list, length - 1);
            var result = new List<List<T>>();

            foreach (var perm in perms)
            {
                foreach (var item in list)
                {
                    if (perm.Contains(item))
                        continue; // Avoid duplicates for AllDifferent
                    var newPerm = new List<T>(perm) { item };
                    result.Add(newPerm);
                }
            }

            return result;
        }
    }
}
