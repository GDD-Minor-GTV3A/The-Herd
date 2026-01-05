using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using Core.Shared.StateMachine;
using Core.Shared.Utilities;

using Team_7.Scripts.AI.Drekavac.States;

using UnityEngine;
using UnityEngine.AI;
using Core.AI.Sheep;
using Core.Events;
using Team_7.Scripts.AI.Events;

namespace Team_7.Scripts.AI.Drekavac
{
    /// <summary>
    ///     Manages the behavior of a "Drekavac" type enemy, adding this component to an object makes it behave like a "Drekavac".
    /// </summary>
    [RequireComponent(typeof(EnemyMovementController), typeof(AudioSource))]
    public class DrekavacStateManager : CharacterStateManager<IState>
    {
        [SerializeField][Required] private DrekavacStats _drekavacStats;
        [SerializeField][Required] private GrabPoint _grabPoint;
        private AudioController _audioController;
        private EnemyMovementController _enemyMovementController;
        private DrekavacAnimatorController _drekavacAnimatorController;
        private GameObject _playerObject;
        private GameObject _dogObject;
        private Vector3 _playerLocation;
        private Vector3 _dogLocation;
        private GameObject _grabbedObject;
        private Rigidbody _grabbedObjectRb;
        private bool _grabbedObjectOriginalKinematic;
        private List<GameObject> _sheep = new(); // All the sheep in the scene

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
            _sheep = GameObject.FindGameObjectsWithTag("Sheep").ToList();
            
            EventManager.AddListener<KillSheepEvent>(OnSheepKilled);
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

            if (_currentState is not FleeingState &&
                ( /*Vector3.Distance(transform.position, _playerLocation) <= _drekavacStats.fleeTriggerDistance ||*/
                    Vector3.Distance(transform.position, _dogLocation) <= _drekavacStats.fleeTriggerDistance))
            {
                ReleaseGrabbedObject();
                Flee();
            }
        }
        
        void LateUpdate()
        {
            if (_grabbedObject is not null)
            {
                _grabbedObject.transform.position = _grabPoint.transform.position;
                _grabbedObject.transform.rotation = _grabPoint.transform.rotation;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (_currentState is HuntingState && other.CompareTag("Sheep"))
            {
                if (other.gameObject.TryGetComponent<Grabbed>(out _)) return;
                Debug.Log("triggered");
                GrabObject(other.gameObject);
            }
        }

        private void GrabObject(GameObject grabbedObject)
        {
            if (grabbedObject == null) 
                return;
            
            NavMeshAgent SSM = grabbedObject.GetComponent<NavMeshAgent>();
            SSM.enabled = false;

            /*Rigidbody rb = grabbedObject.GetComponent<Rigidbody>();
            rb.isKinematic = false;*/

            grabbedObject.TryGetComponent<SheepStateManager>(out var sheepManager);
            sheepManager.DisableBehavior();

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
                _grabbedObject.transform.position = _grabPoint.transform.position;
            }

            _grabbedObject.transform.SetParent(_grabPoint.transform, true);
            _grabbedObject.transform.localPosition = Vector3.zero;
            _grabbedObject.transform.localRotation = Quaternion.identity;
            _grabbedObject.AddComponent<Grabbed>();
            SetState<DraggingState>();
        }

        private float GetColliderExtentAlongDirection(Collider col, Vector3 dir)
        {
            dir = col.transform.InverseTransformDirection(dir.normalized);
            Bounds b = col.bounds;
            Vector3 extents = b.extents;

            return Mathf.Abs(dir.x * extents.x) + Mathf.Abs(dir.y * extents.y) + Mathf.Abs(dir.z * extents.z);
        }

        /*public void CreateGrabPoint(GameObject grabbedObject)
        {
            // Create a grab point if not assigned
            GameObject gp = new (gameObject.name + "_GrabPoint");
            Collider grabbedCollider = grabbedObject.GetComponent<Collider>();
            Collider grabberCollider = gameObject.GetComponent<Collider>();
            
            float yOffset = grabberCollider.bounds.extents.y - grabbedCollider.bounds.extents.y - 0.5f;
            float grabbedZ = grabbedCollider.bounds.extents.z;
            float grabberZ = grabberCollider.bounds.extents.z;
            gp.transform.SetParent(gameObject.transform);
            gp.transform.localPosition = new Vector3(0f, yOffset, grabbedZ + grabberZ);
            _grabPoint = gp.transform;
        }*/

        public void ReleaseGrabbedObject()
        {
            if (_grabbedObject is null) return;
            NavMeshAgent SSM = _grabbedObject.GetComponent<NavMeshAgent>();
            SSM.enabled = true;

            _grabbedObject.TryGetComponent<SheepStateManager>(out var sheepManager);
            sheepManager.EnableBehavior();
            
            //Rigidbody rb = _grabbedObject.GetComponent<Rigidbody>();
            //rb.isKinematic = true;
            
            _grabbedObject.transform.SetParent(null, true);

            //if (_grabbedObjectRb is not null)
            //    _grabbedObjectRb.isKinematic = _grabbedObjectOriginalKinematic;

            if (_grabbedObject.TryGetComponent<Grabbed>(out var grabbed))
                Destroy(grabbed);
            
            _grabbedObject = null;
            _grabbedObjectRb = null;
        }
        
        void OnSheepKilled(KillSheepEvent evt)
        {
            _sheep.Remove(evt.Sheep.gameObject);
        }
    
        public void Flee()
        {
            SetState<FleeingState>();
        }

        public DrekavacStats GetStats()
        {
            return _drekavacStats;
        }

        public bool TryGetGrabbedObject([NotNullWhen(true)] out GameObject grabbedObject)
        {
            grabbedObject = _grabbedObject;
            
            return _grabbedObject is not null;
        }

        public Vector3 GetPlayerLocation()
        {
            return _playerLocation;
        }

        public List<GameObject> GetSheep()
        {
            return _sheep;
        }

        public void DestroySelf()
        {
            Destroy(gameObject);
        }
    
    }
}
