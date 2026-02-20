using System.Collections.Generic;
using System.Linq;

public static class ActionGenerator
{
    // Generate all grounded actions from templates and pointers
    public static List<PlanAction> GenerateAllGroundedActions(List<PlanAction> actionTemplates, List<object> pointers)
    {
        List<PlanAction> groundedActions = new List<PlanAction>();

        foreach (var template in actionTemplates)
        {
            int arity = template.actionArgs.Count; // how many arguments this action expects
            if (arity == 0) // Action has no args
            {
<<<<<<< Updated upstream
                groundedActions.Add(template.CreateNew(new List<object> { }));
                continue;
=======
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
                    //if (grounded.SatisfiesConstraints())
                        //groundedActions.Add(grounded);
                }
>>>>>>> Stashed changes
            }

            // Get all permutations of pointers of length = arity
            var pointerPermutations = GetPermutations(pointers, arity);

            foreach (var args in pointerPermutations)
            {
                // Instantiate new grounded action with these args
                PlanAction groundedAction = template.CreateNew(args.ToList());
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
