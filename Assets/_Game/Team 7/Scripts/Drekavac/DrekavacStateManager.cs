using System;
using System.Collections.Generic;
using System.Data;

using Core.Shared.StateMachine;
using UnityEngine;
using UnityEngine.AI;

public class DrekavacStateManager : CharacterStateManager<IState>
{
    public Transform? playerLocation;
    public DrekavacAudioController AudioController = null!;
    private EnemyMovementController _enemyMovementController = null!;
    public DrekavacAnimatorController drekavacAnimatorController = null!;
    [SerializeField]private DrekavacStats _drekavacStats = null!;
    private Transform? _grabPoint;
    public GameObject? grabbedObject { get; private set; }
    private Rigidbody? _grabbedObjectRb;
    private bool _grabbedSheepOriginalKinematic;

    [SerializeField]
    private AudioClip? screech, chomp, snarl;

    public void Initialize()
    {
        _enemyMovementController = GetComponent<EnemyMovementController>();
        _movementController = _enemyMovementController;
        drekavacAnimatorController = new DrekavacAnimatorController(GetComponentInChildren<Animator>());
        _animatorController = drekavacAnimatorController;
        
        if (screech is null || chomp is null || snarl is null)
            Debug.LogError("Audio file is missing");
        else
            AudioController = new DrekavacAudioController(GetComponent<AudioSource>(), screech, chomp, snarl);

        InitializeStatesMap();
        CreateGrabPoint();

        // Find player by tag (still needed for dragging/abort)
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            playerLocation = playerObj.transform;
        else
        {
            Debug.LogError("EnemyAI: No object with tag 'Player' found!");
            enabled = false;
            return;
        }
        
        AudioController.PlayScreech();
        SetState<StalkingState>();
    }
    protected override void InitializeStatesMap()
    {
        StatesMap = new Dictionary<Type, IState>
        {
            { typeof(HuntingState), new HuntingState(this, _enemyMovementController) },
            { typeof(StalkingState), new StalkingState(this, _enemyMovementController) },
            { typeof(DraggingState), new DraggingState(this, _enemyMovementController) },
            { typeof(FleeingState), new FleeingState(this, _enemyMovementController) }
        };
    }

    private void Start()
    {
        Initialize();
    }

    protected override void Update()
    {
        base.Update();
        if (_currentState is not FleeingState && Vector3.Distance(transform.position, playerLocation.position) <= _drekavacStats.fleeTriggerDistance)
        {
            Flee();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (_currentState is HuntingState && collision.gameObject.CompareTag("Sheep"))
        {
            GrabSheep(collision.gameObject);
        }
    }
        
    private void GrabSheep(GameObject sheep)
    {
        if (sheep == null) return;

        //IMP02 CODE FOR DISABELING SHEEP AI WHEN GRABBED

        // 

        _enemyMovementController.ResetAgent();

        grabbedObject = sheep;
        _grabbedObjectRb = grabbedObject.GetComponent<Rigidbody>();
        if (_grabbedObjectRb != null)
        {
            _grabbedObjectRb.linearVelocity = Vector3.zero;
            _grabbedObjectRb.angularVelocity = Vector3.zero;
            _grabbedSheepOriginalKinematic = _grabbedObjectRb.isKinematic;
            _grabbedObjectRb.isKinematic = true;
        }

        Collider enemyCollider = GetComponent<Collider>();
        Collider sheepCollider = grabbedObject.GetComponent<Collider>();

        Vector3 position = transform.position;
        if (enemyCollider != null && sheepCollider != null)
        {
            Vector3 direction = (grabbedObject.transform.position - position).normalized;

            float enemyEdge = GetColliderExtentAlongDirection(enemyCollider, direction);
            float sheepEdge = GetColliderExtentAlongDirection(sheepCollider, -direction);

            grabbedObject.transform.position = position + direction * (enemyEdge + sheepEdge);
        }
        else
        {
            grabbedObject.transform.position = _grabPoint.position;
        }

        grabbedObject.transform.SetParent(_grabPoint, true);

        SetState<DraggingState>();
    }
    
    private float GetColliderExtentAlongDirection(Collider col, Vector3 dir)
    {
        dir = col.transform.InverseTransformDirection(dir.normalized);
        Bounds b = col.bounds;
        Vector3 extents = b.extents;

        return Mathf.Abs(dir.x * extents.x) + Mathf.Abs(dir.y * extents.y) + Mathf.Abs(dir.z * extents.z);
    }

    public void CreateGrabPoint()
    {
        // Create a grab point if not assigned
        GameObject gp = new (gameObject.name + "_GrabPoint");
        gp.transform.SetParent(gameObject.transform);
        gp.transform.localPosition = new Vector3(0f, 0.5f, 0.6f);
        _grabPoint = gp.transform;
    }

    public void Despawn()
    {
        Destroy(gameObject);
        Destroy(grabbedObject);
        ReleaseGrabbedObject();
    }
    
    public void ReleaseGrabbedObject()
    {
        if (grabbedObject is null) return;

        grabbedObject.transform.SetParent(null, true);

        if (_grabbedObjectRb is not null)
            _grabbedObjectRb.isKinematic = _grabbedSheepOriginalKinematic;

        grabbedObject = null;
        _grabbedObjectRb = null;
    }
    
    private void Flee()
    {
        SetState<FleeingState>();
        
        // Drop sheep if dragging
        if (grabbedObject is not null)
        {
            ReleaseGrabbedObject();
        }

        Vector3 awayDir = (transform.position - playerLocation.position).normalized;
        Vector3 rawTarget = transform.position + awayDir * Mathf.Max(_drekavacStats.fleeDistance, _drekavacStats.circleRadius * 2f);

        // Find a valid NavMesh point in the intended direction
        if (NavMesh.SamplePosition(rawTarget, out NavMeshHit hit, 10f, NavMesh.AllAreas))
        {
            Vector3 destination = hit.position;
            _enemyMovementController.Agent.isStopped = false;
            _enemyMovementController.ResetAgent();
            _enemyMovementController.Agent.updateRotation = false; // let LookAt handle facing
            _enemyMovementController.ToggleAgent(true);
            _enemyMovementController.MoveTo(destination);
        }
        else
        {
            Debug.LogWarning("DrekavacAI: Could not find valid abort position.");
            // If no path, fallback to despawn directly
            Destroy(gameObject);
        }
    }

    public DrekavacStats GetStats()
    {
        return _drekavacStats;
    }
    
}
