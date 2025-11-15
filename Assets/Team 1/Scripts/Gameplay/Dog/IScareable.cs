using UnityEngine;

namespace Gameplay.Dog
{
    public interface IScareable
    {
        void OnScared(Vector3 fromPosition, float intensity, ScareType scareType);
    }
}
