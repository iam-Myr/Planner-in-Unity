using System.Collections.Generic;

namespace Planning
{
    public static class ObservationManager
    {
        private static readonly List<Predicate> predicates = new();

        public static void Register(IObservableHolder holder)
        {
            predicates.AddRange(holder.GetObservablePredicates());
        }

        /*
        public static void Unregister(IObservableHolder holder)
        {
            var toRemove = holder.GetPredicates();
            predicates.RemoveAll(p => toRemove.Contains(p));
        }
        */

        public static WorldState Observe()
        {
            WorldState currentWorldState = new WorldState();

            foreach (Predicate p in predicates)
            {
                bool result = p.Observe(); 
                Predicate observedPredicate = new Predicate(p.TheFunc, p.Args, result);
                currentWorldState.AddPredicates(observedPredicate);
            }

            return currentWorldState;
        }

    }
}
