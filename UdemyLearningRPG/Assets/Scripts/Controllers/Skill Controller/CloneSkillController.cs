using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloneSkillController : MonoBehaviour
{
    private Player player;
    private SpriteRenderer sr;
    private Animator anim;

    [SerializeField] private float colorLoosingSpeed;

    private float cloneTimer;

    [SerializeField] private Transform attackCheck;
    [SerializeField] private float attackCheckRadius = .8f;

    private Transform closestEnemy;

    private bool canDuplicate;
    private float chanceToDuplicate;
    private int facingDir = 1;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        cloneTimer -= Time.deltaTime;

        if (cloneTimer < 0)
        {
            sr.color = new Color(1, 1, 1, sr.color.a - (Time.deltaTime * colorLoosingSpeed));

            if (sr.color.a <= 0)
            {
                Destroy(gameObject);
            }

        }

    }

    public void SetUpClone(Transform newTransform, float _cloneDuration, bool canAttack, Vector3 _offset, Transform _closestEnemy, bool _canDuplicate, float _chanceToDuplicate, Player _player)
    {
        if (canAttack)
        {
            anim.SetInteger("AttackNumber", Random.Range(1, 3));
        }

        transform.position = newTransform.position + _offset;
        cloneTimer = _cloneDuration;

        closestEnemy = _closestEnemy;
        canDuplicate = _canDuplicate;
        chanceToDuplicate = _chanceToDuplicate;
        player = _player;
        FaceClosestTarget();
    }

    private void AnimationTrigger()
    {
        cloneTimer = -.1f;
    }

    private void AttackTrigger()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(attackCheck.position, attackCheckRadius);

        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Enemy>() != null)
            {
                player.stats.DoDamage(hit.GetComponent<CharacterStats>());

                if (canDuplicate)
                {
                    if(Random.Range(0,100) < chanceToDuplicate)
                    {
                        SkillManager.instance.cloneSkill.CreateClone(hit.transform, new Vector3(.5f * facingDir, 0));
                    }
                }

            }
        }
    }


    private void FaceClosestTarget()
    {       
        if(closestEnemy != null)
        {
            if(transform.position.x > closestEnemy.position.x)
            {
                facingDir = -1;
                transform.Rotate(0, 180, 0);
            }
        }
    }
}
