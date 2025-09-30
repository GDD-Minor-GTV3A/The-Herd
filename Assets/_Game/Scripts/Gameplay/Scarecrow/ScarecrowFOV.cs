using System.Collections;

using UnityEngine;

namespace Gameplay.Scarecrow
{
    public class ScarecrowFOV : MonoBehaviour
    {
        [SerializeField] protected float _radius;
        [SerializeField][Range(0, 359)] protected float _angle;
        [SerializeField] protected LayerMask _targetLayer;
        [SerializeField] protected LayerMask _obstructionLayer;

        protected virtual void Start()
        {
            StartCoroutine(FOVRoutine());
        }

        private IEnumerator FOVRoutine()
        {
            WaitForSeconds _wait = new(0.2f);

            while (true)
            {
                yield return _wait;
                FieldOfViewCheck();
            }
        }

        protected virtual void FieldOfViewCheck() { }
    }
}
