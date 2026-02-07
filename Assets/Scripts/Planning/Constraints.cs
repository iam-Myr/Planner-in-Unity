using Planning;
using System;
using System.Collections.Generic;



public static class Constraints
{
    public static Func<List<Pointer>, bool> AllDifferent(params Pointer[] pointers)
    {
        return (args) =>
        {
            // check all pointers are bound
            for (int i = 0; i < pointers.Length; i++)
            {
                for (int j = i + 1; j < pointers.Length; j++)
                {
                    if (args[i].IsBound() && args[j].IsBound())
                        if (args[i].Get().Equals(args[j].Get()))
                            return false;
                }
            }
            return true;
        };
    }
}
