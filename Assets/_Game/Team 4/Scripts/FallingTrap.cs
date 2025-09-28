using System.Collections;
using UnityEngine;

public class FallingTrap : MonoBehaviour
{
    private bool _isFalling = false;
    private float _downSpeed = 0;
    private Vector3 _startPosition;

    /// <summary>
    /// Initialize start position
    /// </summary>
    void Initialize()
    {
        this._startPosition = transform.position;
    }

    /// <summary>
    /// Handles collision with trap
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "Sheep" && !_isFalling)
        {
            // Temp method to store start position
            this._startPosition = transform.position;
            // Start falling, delete sheep after 4 seconds, move platform back
            _isFalling = true;
            Destroy(collision.gameObject, 4f);
            StartCoroutine(ResetTrap());
        }
    }

    /// <summary>
    /// Update position if falling
    /// </summary>
    private void Update()
    {
        if (_isFalling)
        {
            // Increase falling speed over time and move down
            _downSpeed += Time.deltaTime/10;
            transform.position -= new Vector3(0, _downSpeed, 0);
        }
    }

    /// <summary>
    /// Reset trap position after delay
    /// </summary>
    /// <returns></returns>
    private IEnumerator ResetTrap()
    {
        yield return new WaitForSeconds(10f);

        // Reset position and state
        transform.position = _startPosition;
        _downSpeed = 0;
        _isFalling = false;
    }
}
