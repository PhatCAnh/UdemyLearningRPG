using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackHoleSkill : Skill
{
    [SerializeField] private int amountOfAttacks;
    [SerializeField] private float cloneCooldown;
    [SerializeField] private float skillDuration;
    [Space]
    [SerializeField] private GameObject blackHolePrefab;
    [SerializeField] private float maxSize;
    [SerializeField] private float growSpeed;
    [SerializeField] private float shrinkSpeed;

    BlackHoleSkillController currentBlackHole;

    public override bool CanUseSkill()
    {
        return base.CanUseSkill();
    }

    public override void UseSkill()
    {
        base.UseSkill();

        GameObject newBlackHole = Instantiate(blackHolePrefab, player.transform.position, Quaternion.identity);

        currentBlackHole = newBlackHole.GetComponent<BlackHoleSkillController>();

        currentBlackHole.SetUpBlackHole(maxSize, growSpeed, shrinkSpeed, amountOfAttacks, cloneCooldown, skillDuration);

    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
    }

    public bool SkillCompleted()
    {
        if (!currentBlackHole) return false;

        if (currentBlackHole.playerCanExitState)
        {
            currentBlackHole = null;
            return true;
        }

        return false;
    }

    public float GetBlackHoleRadius()
    {
        return maxSize / 2;
    }

}
