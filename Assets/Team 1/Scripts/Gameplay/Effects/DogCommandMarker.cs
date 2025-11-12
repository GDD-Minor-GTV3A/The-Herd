using System.Collections;
using UnityEngine;

public class DogCommandMarker : MonoBehaviour
{
    [SerializeField] private ParticleSystem _circleEffect;
    [SerializeField] private RectTransform _markerObject;
    [SerializeField] private float _animationDuration = 2f;
    [SerializeField] private float _maxHeight = 5f;


    private Coroutine _markerCoroutine;


    public void Initialize()
    {
        _markerObject.gameObject.SetActive(false);
        _circleEffect.gameObject.SetActive(false);
    }


    public void StartEffect(Vector3 worldPosition)
    {
        if (_markerCoroutine != null)
            StopCoroutine(_markerCoroutine);

        if (_circleEffect.isPlaying)
            _circleEffect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        _markerCoroutine = StartCoroutine(MarkerAnimation(worldPosition));
    }


    private IEnumerator MarkerAnimation(Vector3 worldPosition)
    {
        transform.position = worldPosition;
        _markerObject.localPosition = Vector3.zero;
        _markerObject.gameObject.SetActive(true);

        _circleEffect.gameObject.SetActive(true);
        _circleEffect.Play();

        float halfDuration = (_animationDuration / 2);

        float t = 0;

        while (t < _animationDuration)
        {
             float currentHeight = Mathf.PingPong((t / halfDuration) * _maxHeight, _maxHeight);
            _markerObject.localPosition = new Vector3(0, currentHeight, 0);
            t += Time.deltaTime;
            yield return null;
        }

        _markerObject.gameObject.SetActive(false);
        yield return null;
        _markerCoroutine = null;
    }
}
