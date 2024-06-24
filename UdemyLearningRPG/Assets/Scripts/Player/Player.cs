using System.Collections;
using UnityEngine;

public class Player : Entity
{
    public bool isBusy { get; private set; }

    [Header("Move info")]
    public float moveSpeed;
    public float jumpForce;
    public float swordReturnImpact;
    private float defaultMoveSpeed;
    private float defaultJumpForce;


    [Header("Attack info")]
    public Vector2[] attackMovement;
    public float counterAttackDuration = .2f;

    [Header("Dash info")]
    public float dashSpeed;
    private float defaultDashSpeed;
    public float dashDuration;
    public float dashDir { get; private set; }

    public SkillManager skill { get; private set; }

    public GameObject sword { get; private set; }

    #region States
    public PlayerStateMachine stateMachine { get; private set; }
    public PlayerIdleState idleState { get; private set; }
    public PlayerMoveState moveState { get; private set; }
    public PlayerJumpState jumpState { get; private set; }
    public PlayerAirState airState { get; private set; }
    public PlayerDashState dashState { get; private set; }
    public PlayerWallSlideState wallSlideState { get; private set; }
    public PlayerWallJumpState wallJumpState { get; private set; }
    public PlayerPrimaryAttackState primaryAttackState { get; private set; }
    public PlayerCounterAttackState counterAttackState { get; private set; }
    public PlayerAimSwordState aimSwordState { get; private set; }
    public PlayerCatchSwordState catchSwordState { get; private set; }
    public PlayerBlackHoleState blackHoleState { get; private set; }
    public PlayerDeadState deadState { get; private set; }
    #endregion


    protected override void Awake()
    {
        base.Awake();

        stateMachine = new PlayerStateMachine();

        idleState = new PlayerIdleState(this, stateMachine, "Idle");
        moveState = new PlayerMoveState(this, stateMachine, "Move");
        jumpState = new PlayerJumpState(this, stateMachine, "Jump");
        airState = new PlayerAirState(this, stateMachine, "Jump");
        dashState = new PlayerDashState(this, stateMachine, "Dash");
        wallSlideState = new PlayerWallSlideState(this, stateMachine, "WallSlide");
        wallJumpState = new PlayerWallJumpState(this, stateMachine, "Jump");
        primaryAttackState = new PlayerPrimaryAttackState(this, stateMachine, "Attack");
        counterAttackState = new PlayerCounterAttackState(this, stateMachine, "CounterAttack");
        aimSwordState = new PlayerAimSwordState(this, stateMachine, "AimSword");
        catchSwordState = new PlayerCatchSwordState(this, stateMachine, "CatchSword");
        blackHoleState = new PlayerBlackHoleState(this, stateMachine, "Jump");
        deadState = new PlayerDeadState(this, stateMachine, "Die");
    }

    protected override void Start()
    {
        base.Start();

        skill = SkillManager.instance;

        stateMachine.Initialize(idleState);

        defaultMoveSpeed = moveSpeed;
        defaultJumpForce = jumpForce;
        defaultDashSpeed = dashSpeed;
    }

    protected override void Update()
    {
        base.Update();

        stateMachine.currentState.Update();

        CheckForDashInput();

        if (Input.GetKeyDown(KeyCode.F))
        {
            skill.crystalSkill.CanUseSkill();
        }
    }

    public override void SlowEntityBy(float _slowPercentage, float slowDuration)
    {
        moveSpeed *= (1 - _slowPercentage);
        jumpForce *= (1 - _slowPercentage);
        dashSpeed *= (1 - _slowPercentage);
        anim.speed *= (1 - _slowPercentage);

        Invoke("ReturnDefaultSpeed", slowDuration);
    }

    protected override void ReturnDefaultSpeed()
    {
        base.ReturnDefaultSpeed();

        moveSpeed = defaultMoveSpeed;
        jumpForce = defaultJumpForce;
        dashSpeed = defaultDashSpeed;

    }

    public void AssignNewSword(GameObject _newSword)
    {
        sword = _newSword;
    }

    public void CatchTheSword()
    {
        stateMachine.ChangeState(catchSwordState);
        Destroy(sword);
    }


    public void AnimationTrigger() => stateMachine.currentState.AnimationFinishTrigger();

    public IEnumerator BusyFor(float _seconds)
    {
        isBusy = true;

        yield return new WaitForSeconds(_seconds);

        isBusy = false;
    }

    private void CheckForDashInput()
    {
        if (IsWallDetected())
        {
            return;
        }


        if (Input.GetKeyDown(KeyCode.LeftShift) && SkillManager.instance.dashSkill.CanUseSkill())
        {

            dashDir = Input.GetAxisRaw("Horizontal");

            if (dashDir == 0)
            {
                dashDir = facingDir;
            }

            stateMachine.ChangeState(dashState);
        }
    }

    public override void Die()
    {
        base.Die();

       stateMachine.ChangeState(deadState);
    }

}
