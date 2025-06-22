using UnityEngine;
using System;
using System.Threading.Tasks;

namespace Planning
{
    public interface IMoveProvider
    {
        Task MoveTo(Vector3 destination, float speed);
        Vector3 GetPosition();
        float GetSpeed();
    }
}

