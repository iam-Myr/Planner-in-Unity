using System;
using System.Collections.Generic;
using System.Linq;

namespace Planning
{
    public static class ActionGenerator
    {
        // Generate all grounded actions from templates and pointers
        public static List<PlanAction> GenerateAllGroundedActions(List<PlanAction> actionTemplates, List<Pointer> pointers)
        {
            List<PlanAction> groundedActions = new List<PlanAction>();

            foreach (var template in actionTemplates)
            {
                var expectedTypes = template.GetArgTypes();
                int arity = expectedTypes.Count;

                if (arity == 0) // Action has no args
                {
                    groundedActions.Add(template.CreateNew(new List<Pointer>()));
                    continue;
                }

                // Get all permutations of the correct length
                var pointerPermutations = GetPermutations(pointers, arity);

                foreach (var args in pointerPermutations)
                {
                    // Check if argument types match the template
                    bool isMatch = true;
                    for (int i = 0; i < arity; i++)
                    {
                        if (!expectedTypes[i].IsAssignableFrom(args[i].GetDeclaredType()))
                        {
                            isMatch = false;
                            break;
                        }
                    }

                    if (!isMatch)
                        continue;

                    // Instantiate new grounded action with these args
                    PlanAction groundedAction = template.CreateNew(args);
                    groundedActions.Add(groundedAction);
                }
            }

            return groundedActions;
        }

        private static List<List<T>> GetPermutations<T>(IEnumerable<T> list, int length)
        {
            if (length == 1)
                return list.Select(t => new List<T> { t }).ToList();

            var perms = GetPermutations(list, length - 1);
            var result = new List<List<T>>();

            foreach (var perm in perms)
            {
                foreach (var item in list)
                {
                    if (!perm.Contains(item))
                    {
                        var newPerm = new List<T>(perm) { item };
                        result.Add(newPerm);
                    }
                }
            }
            return result;
        }
    }
}
