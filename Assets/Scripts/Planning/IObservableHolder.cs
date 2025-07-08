using System.Collections.Generic;

namespace Planning
{
    public interface IObservableHolder
    {
        List<Predicate> GetPredicates();
        void Register();
    }
}
