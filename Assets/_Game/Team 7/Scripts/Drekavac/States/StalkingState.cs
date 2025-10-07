using Core.Shared.StateMachine;

using UnityEngine;
using UnityEngine.AI;

public class StalkingState : IState
{
    private DrekavacStateManager _manager;
    private EnemyMovementController _movement;
    private DrekavacAnimatorController _animator;
    private Vector3 _circleCenter;
    private float _angleOffset;                         // Angle used for circling calculations
    private int _circleDirection;                       // 1 or -1 for clockwise/counterclockwise
    private float _nextSwitchTime;                      // Next time to switch circling direction
    private bool _isSettled;                    // True when enemy reached circle radius
    private float _stalkEndTime;                        // When to stop stalking and start hunting
    float _directionSwitchInterval = 5f;         // Time between direction switches while circling
    private Vector2 _stalkDurationRange = new (10f, 20f); // Randomized stalking duration
    private float _circleSpeed = 7f;
    
    public StalkingState(DrekavacStateManager manager, EnemyMovementController movement)
    {
        _manager = manager;
        _movement = movement;

        _animator = manager.drekavacAnimatorController;
        if (_animator is null)
            Debug.LogWarning("StalkingState: drekavacAnimatorController is null!");
    }
    
    public void OnStart()
    {
        //_movement.Agent.updatePosition = true;
        //_movement.Agent.updateRotation = true;
        _movement.SetMovementSpeed(3.5f);
        
        // --- Compute initial circle center from sheep ---
        GameObject[] sheepObjects = GameObject.FindGameObjectsWithTag("Sheep");
        if (sheepObjects.Length > 0)
        {
            Vector3 avgPos = Vector3.zero;
            foreach (GameObject sheep in sheepObjects)
                avgPos += sheep.transform.position;
            avgPos /= sheepObjects.Length;
            _circleCenter = avgPos;
        }
        else
        {
            if (_manager.playerLocation is null)
            {
                Debug.LogError("StalkingState: playerLocation is null!");
                _circleCenter = Vector3.zero; // or some fallback
            }
            // Fallback to player if no sheep exist
            _circleCenter = _manager.playerLocation.position;
        }
        
        // Position enemy at nearest point on the circle radius around the center
        Vector3 toEnemy = (_manager.transform.position - _circleCenter).normalized;
        Vector3 nearestPoint = _circleCenter + toEnemy * _manager.GetStats().circleRadius;

        if (NavMesh.SamplePosition(nearestPoint, out NavMeshHit hit, 2f, NavMesh.AllAreas))
            _movement.MoveTo(hit.position);

        // Compute initial angleOffset based on enemy relative to circle center
        _angleOffset = Mathf.Atan2(toEnemy.z, toEnemy.x) * Mathf.Rad2Deg;
        _circleDirection = Random.value > 0.5f ? 1 : -1;  // Random initial circling direction

        ScheduleNextDirectionSwitch();
        _stalkEndTime = Time.time + Random.Range(_stalkDurationRange.x, _stalkDurationRange.y);
    }

    public void OnUpdate()
    {
        // Compute the average position of all sheep to use as circle center
        GameObject[] sheepObjects = GameObject.FindGameObjectsWithTag("Sheep");
        if (sheepObjects.Length > 0)
        {
            Vector3 avgPos = Vector3.zero;
            foreach (GameObject sheep in sheepObjects)
                avgPos += sheep.transform.position;
            avgPos /= sheepObjects.Length;
            _circleCenter = avgPos; // Update circle center each frame
        }

        if (!_isSettled)
        {
            // Always recalc nearest point dynamically in case sheep have moved
            Vector3 toEnemy = (_manager.transform.position - _circleCenter).normalized;
            Vector3 nearestPoint = _circleCenter + toEnemy * _manager.GetStats().circleRadius;

            if (NavMesh.SamplePosition(nearestPoint, out NavMeshHit hit, 2f, NavMesh.AllAreas))
                _movement.MoveTo(hit.position);

            float distToCenter = Vector3.Distance(_manager.transform.position, _circleCenter);
            if (Mathf.Abs(distToCenter - _manager.GetStats().circleRadius) < 0.5f)
            {
                _isSettled = true; // Enemy reached the circle radius
                _manager.AudioController.PlaySnarl();
            }
            else
            {
                // Face the herd center while moving to radius
                _movement.LookAt(_circleCenter);
                return;
            }
        }

        
        // Switch circling direction at intervals
        if (Time.time >= _nextSwitchTime)
        {
            _circleDirection *= -1;
            ScheduleNextDirectionSwitch();
        }

        // Update circling angle
        _angleOffset += _circleSpeed * _circleDirection * Time.deltaTime;

        // Calculate target position on circle
        float radians = _angleOffset * Mathf.Deg2Rad;
        Vector3 offset = new Vector3(Mathf.Cos(radians), 0f, Mathf.Sin(radians)) * _manager.GetStats().circleRadius;
        Vector3 targetPos = _circleCenter + offset;

        if (NavMesh.SamplePosition(targetPos, out NavMeshHit hit2, 2f, NavMesh.AllAreas))
            _movement.MoveTo(hit2.position);

        // Face the herd center while circling
        _movement.LookAt(_circleCenter);
        
        _animator.SetStalking(true);
        //animator.SetBool("Stalking", true);  // Play stalking animation

        if (Time.time >= _stalkEndTime)
        {
            _manager.SetState<HuntingState>();
            //animator.SetBool("Stalking", false);
        }
    }

    public void OnStop()
    {
        _animator.SetStalking(false);
        _movement.Agent.updateRotation = false;
    }
    
    private void ScheduleNextDirectionSwitch()
    {
        float randomFactor = Random.Range(0.5f, 1.5f);
        _nextSwitchTime = Time.time + _directionSwitchInterval * randomFactor;
    }
}
