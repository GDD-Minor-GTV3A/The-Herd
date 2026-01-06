using System.Collections;
using Core.Shared.Utilities;
using UnityEngine;

namespace Gameplay.Effects
{
    /// <summary>
    /// Controls visual effect for showing the position of dog move command.
    /// </summary>
    public class DogCommandMarker : MonoBehaviour
    {
        [SerializeField, Tooltip("Particle system of circles on the ground"), Required] 
        private ParticleSystem circleEffect;

        [SerializeField, Tooltip(""), Required] 
        private RectTransform markerObject;

        [SerializeField, Tooltip("Duration of marker jump animation.")] 
        private float markerAnimationDuration = 1.5f;

        [SerializeField, Tooltip("Max height of marker during jump animation.")] 
        private float maxHeight = 5f;


        private Coroutine markerCoroutine;


        /// <summary>
        /// Initialization method.
        /// </summary>
        public void Initialize()
        {
            markerObject.gameObject.SetActive(false);
            circleEffect.gameObject.SetActive(false);
        }


        /// <summary>
        /// Starts effect animation.
        /// </summary>
        /// <param name="worldPosition">New position of marker.</param>
        public void StartEffect(Vector3 worldPosition)
        {
            if (markerCoroutine != null)
                StopCoroutine(markerCoroutine);

            if (circleEffect.isPlaying)
                circleEffect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

            markerCoroutine = StartCoroutine(MarkerAnimationRoutine(worldPosition));
        }

        private IEnumerator MarkerAnimationRoutine(Vector3 worldPosition)
        {
            transform.position = worldPosition;
            markerObject.localPosition = Vector3.zero;
            markerObject.gameObject.SetActive(true);

            circleEffect.gameObject.SetActive(true);
            circleEffect.Play();

            float _halfDuration = (markerAnimationDuration / 2);

            float _t = 0;

            while (_t < markerAnimationDuration)
            {
                float _currentHeight = Mathf.PingPong((_t / _halfDuration) * maxHeight, maxHeight);
                markerObject.localPosition = new Vector3(0, _currentHeight, 0);
                _t += Time.deltaTime;
                yield return null;
            }

            markerObject.gameObject.SetActive(false);
            yield return null;
            markerCoroutine = null;
        }
    }
}