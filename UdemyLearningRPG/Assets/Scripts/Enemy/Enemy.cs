using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Entity
{
    [Header("Stunned info")]
    public float stunDuration;
    public Vector2 stunDirection;
    protected bool canBeStun;
    [SerializeField] protected GameObject counterImage;

    [Header("Move info")]
    public float moveSpeed;
    public float idleTime;
    public float battleTime;
    public float defaultMoveSpeed;

    [Header("Attack info")]
    public float attackDistance;
    public float attackCooldown;
    [HideInInspector] public float lastTimeAttacked;

    [SerializeField] protected LayerMask whatIsPlayer;

    public EnemyStateMachine stateMachine { get; private set; }

    public string lastAnimBoolName {  get; private set; }

    protected override void Awake()
    {
        base.Awake();

        stateMachine = new EnemyStateMachine();

        defaultMoveSpeed = moveSpeed;
    }

    protected override void Update()
    {
        base.Update();

        stateMachine.currentState.Update();
    }

    public virtual void AssignLastAnimName(string _animBoolName) => lastAnimBoolName = _animBoolName;

    public override void SlowEntityBy(float _slowPercentage, float slowDuration)
    {
        moveSpeed *= (1 - _slowPercentage);
        anim.speed *= (1 - _slowPercentage);

        Invoke("ReturnDefaultSpeed", slowDuration);
    }

    protected override void ReturnDefaultSpeed()
    {
        base.ReturnDefaultSpeed();
        moveSpeed = defaultMoveSpeed;
    }

    public virtual void FreezeTimer(bool _timeFrozen)
    {
        if (_timeFrozen)
        {
            moveSpeed = 0;
            anim.speed = 0;
        }
        else
        {
            moveSpeed = defaultMoveSpeed;
            anim.speed = 1;
        }
    }

    protected virtual IEnumerator FreezeTimerFor(float _seconds)
    {
        FreezeTimer(true);

        yield return new WaitForSeconds(_seconds);

        FreezeTimer(false);
    }


    #region Counter Attack Window
    public virtual void OpenCounterAttackWindow()
    {
        canBeStun = true;

        counterImage.SetActive(true);
    }

    public virtual void CloseCounterAttackWindow()
    {
        canBeStun = false;

        counterImage.SetActive(false);
    }
    #endregion

    public virtual bool CanBeStunned()
    {
        if (canBeStun)
        {
            CloseCounterAttackWindow();
            return true;
        }
        return false;
    }

    public virtual void AnimationFinishTrigger() => stateMachine.currentState.AnimationFinishTrigger();

    public virtual RaycastHit2D IsPlayerDetected() => Physics2D.Raycast(wallCheck.position, Vector2.right * facingDir, 50, whatIsPlayer);

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, new Vector3(transform.position.x + attackDistance * facingDir, transform.position.y));
    }
}
