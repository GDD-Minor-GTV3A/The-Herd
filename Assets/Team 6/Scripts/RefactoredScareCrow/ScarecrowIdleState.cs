using UnityEngine;

public class Monster1IdleState : IMonster1State
{
    private readonly ScareCrowStateMachine ctx;

    public Monster1IdleState(ScareCrowStateMachine ctx)
    {
        this.ctx = ctx;
    }

    public void Enter()
    {
        // Nothing special yet – sound is handled in SwitchState.
        Debug.Log("[Monster1IdleState] Enter");
    }

    public void Tick()
    {
        // Old IdleBehavior:
        if (ctx.player != null &&
            Vector3.Distance(ctx.player.position, ctx.transform.position) < ctx.detectionRange)
        {
            ctx.SwitchState(ctx.StalkingState);
        }
    }

    public void Exit()
    {
        Debug.Log("[Monster1IdleState] Exit");
    }
}
