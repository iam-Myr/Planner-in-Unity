using System.Collections.Generic;

namespace Planning
{
    public interface IObservableHolder
    {
        public List<Observable> GetObservables();
        void Register();
    }
}
