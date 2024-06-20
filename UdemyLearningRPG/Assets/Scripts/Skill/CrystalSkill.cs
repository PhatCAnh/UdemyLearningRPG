using System.Collections.Generic;
using UnityEngine;

public class CrystalSkill : Skill
{
    [SerializeField] private GameObject crystalPrefab;
    private GameObject currentCrystal;

    [SerializeField] private float crystalDuration;

    [Header("Explosive info")]
    [SerializeField] private bool canExplode;


    [Header("Moving info")]
    [SerializeField] private bool canMoveToEnemy;
    [SerializeField] private float moveSpeed;

    [Header("Multi stacking crystal")]
    [SerializeField] private bool canUseMultiStacks;
    [SerializeField] private int amountOfStacks;
    [SerializeField] private float multiStackCooldown;
    [SerializeField] private float useTimeWindow;
    [SerializeField] private List<GameObject> crystalLeftList = new List<GameObject>();

    public override void UseSkill()
    {
        base.UseSkill();

        if (CanUseMultiCrystal()) return;

        if (currentCrystal == null)
        {
            currentCrystal = Instantiate(crystalPrefab, player.transform.position, Quaternion.identity);
            CrystalSkillController currentCrystalScript = currentCrystal.GetComponent<CrystalSkillController>();

            currentCrystalScript.SetUpCrystal(crystalDuration, canExplode, canMoveToEnemy, moveSpeed, FindClosestEnemy(currentCrystal.transform));
        }
        else
        {
            if (canMoveToEnemy) return;

            Vector2 playerPos = player.transform.position;

            player.transform.position = currentCrystal.transform.position;
            currentCrystal.transform.position = playerPos;

            currentCrystal.GetComponent<CrystalSkillController>()?.FinishCrystal();
        }

    }

    private bool CanUseMultiCrystal()
    {
        if (canUseMultiStacks)
        {
            if (crystalLeftList.Count > 0)
            {
                if(crystalLeftList.Count == amountOfStacks)
                {
                    Invoke("ResetAbility", useTimeWindow);
                }

                cooldown = 0;
                GameObject crystalToSpawn = crystalLeftList[crystalLeftList.Count - 1];
                GameObject newCrystal = Instantiate(crystalToSpawn, player.transform.position, Quaternion.identity);

                crystalLeftList.Remove(crystalToSpawn);
                newCrystal.GetComponent<CrystalSkillController>().SetUpCrystal(crystalDuration, canExplode, canMoveToEnemy, moveSpeed, FindClosestEnemy(newCrystal.transform));

                if (crystalLeftList.Count <= 0)
                {
                    cooldown = multiStackCooldown;
                    RefilCrystal();
                }

                return true;
            }
        }

        return false;
    }

    private void ResetAbility()
    {
        if (cooldownTimer > 0) return;
        cooldownTimer = multiStackCooldown;
        RefilCrystal();
    }

    private void RefilCrystal()
    {
        int amountToAdd = amountOfStacks - crystalLeftList.Count;

        for (int i = 0; i < amountToAdd; i++)
        {
            crystalLeftList.Add(crystalPrefab);
        }
    }


}
