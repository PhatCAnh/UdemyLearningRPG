using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySkeletonAnimationTriggers : MonoBehaviour
{
    private EnemySkeleton EnemySkeleton => GetComponentInParent<EnemySkeleton>();

    private void AnimationTrigger()
    {
        EnemySkeleton.AnimationFinishTrigger();
    }

    private void AttackTrigger()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(EnemySkeleton.attackCheck.position, EnemySkeleton.attackCheckRadius);

        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Player>() != null)
            {
                hit.GetComponent<Player>().Damage();
            }
        }
    }

    private void OpenCounterWindow() => EnemySkeleton.OpenCounterAttackWindow();
    private void CloseCounterWindow() => EnemySkeleton.CloseCounterAttackWindow();


}
