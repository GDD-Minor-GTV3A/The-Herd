using System;
using System.Collections.Generic;
using Core.Shared.StateMachine;
using Core.Shared.Utilities;

using Team_7.Scripts.AI.Drekavac.States;

using UnityEngine;
using UnityEngine.AI;
using Core.AI.Sheep;

namespace Team_7.Scripts.AI.Drekavac
{
    /// <summary>
    ///     Manages the behavior of a "Drekavac" type enemy, adding this component to an object makes it behave like a "Drekavac".
    /// </summary>
    [RequireComponent(typeof(EnemyMovementController), typeof(AudioSource))]
    public class DrekavacStateManager : CharacterStateManager<IState>
    {
        [SerializeField][Required] private DrekavacStats _drekavacStats;
        private AudioController _audioController;
        private EnemyMovementController _enemyMovementController;
        private DrekavacAnimatorController _drekavacAnimatorController;
        private GameObject _playerObject;
        private GameObject _dogObject;
        private Vector3 _playerLocation;
        private Vector3 _dogLocation;
        private Transform _grabPoint = null!;
        private GameObject _grabbedObject;
        private Rigidbody _grabbedObjectRb;
        private bool _grabbedObjectOriginalKinematic;
        private GameObject[] _sheep = { }; // All the sheep in the scene

        public void Initialize()
        {
            _enemyMovementController = GetComponent<EnemyMovementController>();
            _movementController = _enemyMovementController;

            Animator animator = GetComponentInChildren<Animator>();
            if (animator == null)
            {
                Debug.LogWarning("A Drekavac enemy is missing it's Animator component, a new Animator has been created");
                animator = gameObject.AddComponent<Animator>();
            }
            _drekavacAnimatorController = new DrekavacAnimatorController(animator);
            _animatorController = _drekavacAnimatorController;

            if (_drekavacStats.screechSound == null || _drekavacStats.chompSound == null || _drekavacStats.snarlSound == null)
                Debug.LogWarning("A Drekavac enemy is missing one or more audio files.");
            else
            {
                var audioSource = GetComponent<AudioSource>();
                _audioController = new AudioController(audioSource);
                _audioController.PlayClip(_drekavacStats.screechSound);
            }

            InitializeStatesMap();

            _playerObject = GameObject.FindGameObjectWithTag("Player");
            _playerLocation = _playerObject.transform.position;

            _dogObject = GameObject.Find("Dog");
            if (_dogObject != null)
            {
                _dogLocation = _dogObject.transform.position;
            }

            // Find sheep
            // TODO replace this
            _sheep = GameObject.FindGameObjectsWithTag("Sheep");

            SetState<StalkingState>();
        }
        
        protected override void InitializeStatesMap()
        {
            StatesMap = new Dictionary<Type, IState>
            {
                { typeof(HuntingState), new HuntingState(this, _enemyMovementController, _drekavacStats, _drekavacAnimatorController, _audioController) },
                { typeof(StalkingState), new StalkingState(this, _enemyMovementController, _drekavacStats, _drekavacAnimatorController, _audioController) },
                { typeof(DraggingState), new DraggingState(this, _enemyMovementController, _drekavacStats, _drekavacAnimatorController, _audioController) },
                { typeof(FleeingState), new FleeingState(this, _enemyMovementController, _drekavacStats, _drekavacAnimatorController, _audioController) },
            };
        }

        private void Start()
        {
            Initialize();
        }

        protected override void Update()
        {
            base.Update();
            _playerLocation = _playerObject.transform.position;
            if (_dogObject != null)
            {
                _dogLocation = _dogObject.transform.position;
            }
            if (_currentState is not FleeingState && (/*Vector3.Distance(transform.position, _playerLocation) <= _drekavacStats.fleeTriggerDistance ||*/ Vector3.Distance(transform.position, _dogLocation) <= _drekavacStats.fleeTriggerDistance))
                Flee();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (_currentState is HuntingState && other.CompareTag("Sheep"))
            {
                Debug.Log("triggered");
                GrabObject(other.gameObject);
            }
        }

        private void GrabObject(GameObject grabbedObject)
        {
            if (grabbedObject == null) return;
            CreateGrabPoint();
            //CODE FOR DISABELING SHEEP AI WHEN GRABBED
            NavMeshAgent SSM = grabbedObject.GetComponent<NavMeshAgent>();
            SSM.enabled = false;

            Rigidbody rb = grabbedObject.GetComponent<Rigidbody>();
            rb.isKinematic = false;

            _enemyMovementController.ResetAgent();

            _grabbedObject = grabbedObject;
            _grabbedObjectRb = _grabbedObject.GetComponent<Rigidbody>();
            if (_grabbedObjectRb != null)
            {
                _grabbedObjectRb.linearVelocity = Vector3.zero;
                _grabbedObjectRb.angularVelocity = Vector3.zero;
                _grabbedObjectOriginalKinematic = _grabbedObjectRb.isKinematic;
                _grabbedObjectRb.isKinematic = true;
            }

            Collider enemyCollider = GetComponent<Collider>();
            Collider sheepCollider = _grabbedObject.GetComponent<Collider>();

            Vector3 position = transform.position;
            if (enemyCollider != null && sheepCollider != null)
            {
                Vector3 direction = (_grabbedObject.transform.position - position).normalized;

                float enemyEdge = GetColliderExtentAlongDirection(enemyCollider, direction);
                float sheepEdge = GetColliderExtentAlongDirection(sheepCollider, -direction);

                _grabbedObject.transform.position = position + direction * (enemyEdge + sheepEdge);
            }
            else
            {
                _grabbedObject.transform.position = _grabPoint.position;
            }

            _grabbedObject.transform.SetParent(_grabPoint, true);

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
            gp.transform.localPosition = new Vector3(0f, 0.5f, 0.6f); //TODO replace hardcoded values with a variable
            _grabPoint = gp.transform;
        }

        public void Despawn()
        {
            Destroy(_grabbedObject);
            Destroy(gameObject);
        }
    
        public void ReleaseGrabbedObject()
        {
            if (_grabbedObject is null) return;
            NavMeshAgent SSM = _grabbedObject.GetComponent<NavMeshAgent>();
            SSM.enabled = true;

            Rigidbody rb = _grabbedObject.GetComponent<Rigidbody>();
            rb.isKinematic = true;
            _grabbedObject.transform.SetParent(null, true);

            if (_grabbedObjectRb is not null)
                _grabbedObjectRb.isKinematic = _grabbedObjectOriginalKinematic;

            _grabbedObject = null;
            _grabbedObjectRb = null;
        }
    
        private void Flee()
        {
            SetState<FleeingState>();
        }

        public DrekavacStats GetStats()
        {
            return _drekavacStats;
        }

        public GameObject GetGrabbedObject()
        {
            return _grabbedObject;
        }

        public Vector3 GetPlayerLocation()
        {
            return _playerLocation;
        }

        public GameObject[] GetSheep()
        {
            return _sheep;
        }

        public void DestroySelf()
        {
            Destroy(gameObject);
        }
    
    }
}
