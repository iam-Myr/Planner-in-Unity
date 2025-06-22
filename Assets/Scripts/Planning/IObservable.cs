using System.Collections.Generic;

namespace Planning
{
    public interface IObservable
    {
        List<Predicate> GetState();
        void Register();
    }
}
