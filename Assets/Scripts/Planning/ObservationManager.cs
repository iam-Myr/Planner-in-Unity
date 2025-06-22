using System.Collections.Generic;

namespace Planning
{
    public static class ObservationManager
    {
        private static readonly List<IObservable> observables = new();

        public static void Register(IObservable observable)
        {
            if (!observables.Contains(observable))
                observables.Add(observable);
        }

        public static void Unregister(IObservable observable)
        {
            if (observables.Contains(observable))
                observables.Remove(observable);
        }

        public static WorldState Observe()
        {
            WorldState currentWorldState = new WorldState();

            foreach (IObservable observable in observables)
            {
                currentWorldState.AddPredicates(observable.GetState().ToArray());
            }

            //currentWorldState.Print();
            return currentWorldState;
        }
    }
}
