using System.Collections.Generic;
using System.Linq;

public static class ActionGenerator
{
    // Generate all grounded actions from templates and pointers
    public static List<Action> GenerateAllGroundedActions(List<Action> actionTemplates, List<Pointer> pointers)
    {
        List<Action> groundedActions = new List<Action>();

        foreach (var template in actionTemplates)
        {
            int arity = template.actionArgs.Count; // how many arguments this action expects
            if (arity == 0) // Action has no args
            {
                groundedActions.Add(template.CreateNew(new List<Pointer> { }));
                continue;
            }

            // Get all permutations of pointers of length = arity
            var pointerPermutations = GetPermutations(pointers, arity);

            foreach (var args in pointerPermutations)
            {
                // Instantiate new grounded action with these args
                Action groundedAction = template.CreateNew(args.ToList());
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
