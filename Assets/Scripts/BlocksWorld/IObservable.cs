using System.Collections.Generic;

public interface IObservable
{
    List<Predicate> GetState();
    void Register();
}
