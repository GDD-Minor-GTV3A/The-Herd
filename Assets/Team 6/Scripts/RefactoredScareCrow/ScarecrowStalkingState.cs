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
        // Old StalkingBehavior logic:

        ctx.stalkTimer -= Time.deltaTime;

        bool canSee = false;
        bool isVisible = false;

        var fov = ctx.GetComponent<FieldOfView>();
        var camDet = ctx.GetComponent<NEWInCameraDetector>();

        if (fov != null)
            canSee = fov.canSeePlayer;

        if (camDet != null)
            isVisible = camDet.IsVisible;

        if (canSee && isVisible)
        {
            ctx.visibleTimer += Time.deltaTime;
            if (ctx.visibleTimer >= ctx.timeToVanish && ctx.stalkTimer <= 0f)
            {
                Debug.Log("[Monster1StalkingState] Player stared too long -> teleporting!");
                ctx.TryTeleport(excludeNode: ctx.currentNode, allowDuringAttack: true);
                ctx.visibleTimer = 0f;
                ctx.stalkTimer = ctx.stalkCooldown;
            }
        }
        else
        {
            ctx.visibleTimer = 0f;
        }

        if (ctx.player != null &&
            Vector3.Distance(ctx.player.position, ctx.transform.position) > ctx.playerTooFarDistance &&
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
