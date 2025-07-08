using System.Collections.Generic;

namespace Planning
{
    public static class ObservationManager
    {
        private static readonly List<Observable> observables = new();

        public static void Register(IObservableHolder holder)
        {
                observables.AddRange(holder.GetObservables()); // Return list of key,value pairs (string, observable)
        }

        /*
        public static void Unregister(IObservableHolder observable)
        {
            if (observables.Contains(observable))
                observables.Remove(observable);
        }
        */

        public static WorldState Observe()
        {
            WorldState currentWorldState = new WorldState();

            foreach (Observable observable in observables)
            {
                Predicate p = new Predicate(observable, observable.Observe());
                currentWorldState.AddPredicates(p);
            }

            //currentWorldState.Print();
            return currentWorldState;
        }
    }
}
