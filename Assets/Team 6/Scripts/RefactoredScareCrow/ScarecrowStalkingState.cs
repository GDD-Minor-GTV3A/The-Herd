using UnityEngine;

public class Monster1StalkingState : IMonster1State
{
    private readonly ScareCrowStateMachine ctx;

    public Monster1StalkingState(ScareCrowStateMachine ctx)
    {
        this.ctx = ctx;
    }

    public void Enter()
    {
        Debug.Log("[Monster1StalkingState] Enter");
        // Reset anything if needed
    }

    public void Tick()
    {
        ctx.stalkTimer -= Time.deltaTime;

        bool canSee = false;
        bool isVisible = false;

        var fov = ctx.GetComponent<FieldOfView>();
        var camDet = ctx.GetComponent<NEWInCameraDetector>();

        if (fov != null)
            canSee = fov.canSeePlayer;

        if (camDet != null)
            isVisible = camDet.IsVisible;


        if (ctx.player != null && !ctx.isTeleporting)
        {
            Vector3 dir = ctx.player.position - ctx.transform.position;
            dir.y = 0f;

            if (dir.sqrMagnitude > 0.001f)
            {
                dir.Normalize();
                float facingDot = Vector3.Dot(ctx.transform.forward, dir); // 1=looking at player, -1=backwards
                bool facingPlayerEnough = facingDot > 0.5f; // ~60 degrees cone

                // rotate if not visible OR visible but facing wrong way
                if (!isVisible || !facingPlayerEnough)
                {
                    Quaternion targetRot = Quaternion.LookRotation(dir);
                    ctx.transform.rotation = Quaternion.Slerp(
                        ctx.transform.rotation,
                        targetRot,
                        ctx.rotationSpeed * Time.deltaTime
                    );
                }
            }
        }

        //if (canSee && isVisible)
        //{
        //    ctx.visibleTimer += Time.deltaTime;
        //    if (ctx.visibleTimer >= ctx.timeToVanish && ctx.stalkTimer <= 0f)
        //    {
        //        Debug.Log("[Monster1StalkingState] Player stared too long -> teleporting!");
        //        ctx.TryTeleport(excludeNode: ctx.currentNode, allowDuringAttack: true);
        //        ctx.visibleTimer = 0f;
        //        ctx.stalkTimer = ctx.stalkCooldown;
        //    }
        //}
        //else
        //{
        //    ctx.visibleTimer = 0f;
        //}

        float tooFarSqr = ctx.playerTooFarDistance * ctx.playerTooFarDistance;
        if (ctx.player != null &&
            (ctx.player.position - ctx.transform.position).sqrMagnitude > tooFarSqr &&
            ctx.stalkTimer <= 0f)
        {
            Debug.Log("[Monster1StalkingState] Player is too far -> teleporting closer.");
            ctx.TryTeleport(excludeNode: ctx.currentNode, allowDuringAttack: false);
            ctx.stalkTimer = ctx.stalkCooldown;
        }
    }

    public void Exit()
    {
        Debug.Log("[Monster1StalkingState] Exit");
    }
}
