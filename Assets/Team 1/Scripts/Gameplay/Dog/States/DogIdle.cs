using System.Collections;
using UnityEngine;

namespace Gameplay.Dog
{
    /// <summary>
    /// Dog is not moving.
    /// </summary>
    public class DogIdle : DogState
    {
        private readonly Transform playerTransform;
        private readonly float distanceToStartFollow;

        private Coroutine delayCoroutine;


        /// <param name="playerTransform">Transform of player object to follow.</param>
        /// <param name="distanceToStartFollow">Distance between dog and player to start move.</param>
        public DogIdle(DogStateManager manager, Transform playerTransform, float distanceToStartFollow) : base(manager)
        {
            this.playerTransform = playerTransform;
            this.distanceToStartFollow = distanceToStartFollow;
        }


        public override void OnStart()
        {
            manager.CurrentCommandTarget.OnValueChanged += OnTargetChanged;
            animator.SetWalking(false);
        }

        public override void OnStop()
        {
            if (delayCoroutine != null)
            {
                manager.StopCoroutine(delayCoroutine);
                delayCoroutine = null;
            }

            manager.CurrentCommandTarget.OnValueChanged -= OnTargetChanged;
        }

        public override void OnUpdate()
        {
            if (manager.HerdZone.IsFreeSheepToHeard())
                manager.SetState<DogMoveToSheep>();


            if (Vector3.Distance(manager.MovementController.transform.position, playerTransform.position) > distanceToStartFollow && delayCoroutine == null)
                delayCoroutine = manager.StartCoroutine(MoveDelayRoutine());
        }


        private IEnumerator MoveDelayRoutine()
        {
            yield return new WaitForSeconds(1.5f);
            delayCoroutine = null;
            manager.SetState<DogFollowPlayer>();
        }


        private void OnTargetChanged()
        {
            manager.SetState<DogMove>();
        }
    }
}