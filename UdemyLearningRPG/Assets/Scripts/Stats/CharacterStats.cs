using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    EntityFX fx;

    [Header("Major stats")]
    public Stats strength; // 1 point increase damage by 1 and crit.power by 1%
    public Stats agility; // 1 point increase evasion by 1% and crit.chance by 1%
    public Stats intelligence; // 1 point increase magic by 1% and magic resistance by 3
    public Stats vitality; // 1 point increase health 3-5 points

    [Header("Offensive stats")]
    public Stats damage;
    public Stats critChance;
    public Stats critPower; // default value 150%

    [Header("Defensive stats")]
    public Stats maxHealth;
    public Stats armor;
    public Stats evasion;
    public Stats magicResistance;

    [Header("Magic stats")]
    public Stats fireDamage;
    public Stats iceDamage;
    public Stats lightingDamage;

    [Header("Status stats")]
    public bool isIgnited; // does damage over time
    public bool isChilled; // reduce armor by 20%
    public bool isShocked; // reduce accuracy by 20%

    [SerializeField] private float ailmentsDuration = 4;
    private float ignitedTimer, ignitedDamageCooldown = .3f, ignitedDamageTimer;
    private int igniteDamage;

    private float chilledTimer;

    private float shockedTimer;
    [SerializeField] private GameObject thunderStrikePrefab;
    private int shockDamage;

    [Space]
    public int currentHealth;
    protected bool isDead;

    public System.Action onHealthChanged;
    public int GetMaxHealthValue()
    {
        return maxHealth.GetValue() + vitality.GetValue() * 5;
    }

    protected virtual void Start()
    {
        currentHealth = GetMaxHealthValue();
        critPower.SetDefaultValue(150);

        fx = GetComponent<EntityFX>();
    }

    protected virtual void Update()
    {
        ignitedTimer -= Time.deltaTime;
        chilledTimer -= Time.deltaTime;
        shockedTimer -= Time.deltaTime;

        ignitedDamageTimer -= Time.deltaTime;

        if (ignitedTimer < 0)
        {
            isIgnited = false;
        }
        if (chilledTimer < 0)
        {
            isChilled = false;
        }
        if (shockedTimer < 0)
        {
            isShocked = false;
        }


        if (isIgnited)
        {
            ApplyIgniteDamage();
        }
       

    }

    public virtual void TakeDamage(int _damage)
    {
        DecreaseHealthBy(_damage);

        GetComponent<Entity>().DamageImpact();
        fx.StartCoroutine("FlashFX");

        if (currentHealth <= 0 && !isDead)
        {
            Die();
        }
    }

    public virtual void DoDamage(CharacterStats _targetStats)
    {
        if (TargetCanAvoidAttack(_targetStats)) return;

        int totalDamage = damage.GetValue() + strength.GetValue();

        if (CanCrit())
        {
            totalDamage = CalculateCriticalDamage(totalDamage);
        }

        totalDamage = CheckTargetArmor(_targetStats, totalDamage);
        _targetStats.TakeDamage(totalDamage);

        //if inventory current weapon has fire effect
        //then DoMagicDamage(_targetStats);
    }

    #region Magical damage and ailments

    public virtual void DoMagicDamage(CharacterStats _targetStats)
    {
        int _fireDamage = fireDamage.GetValue();
        int _iceDamage = iceDamage.GetValue();
        int _lightingDamage = lightingDamage.GetValue();

        int totalMagicDamage = _fireDamage + _iceDamage + _lightingDamage + intelligence.GetValue();

        totalMagicDamage = CheckTargetResistance(_targetStats, totalMagicDamage);
        _targetStats.TakeDamage(totalMagicDamage);

        if (Mathf.Max(_fireDamage, _iceDamage, _lightingDamage) <= 0) return;


        AttemptyToApplyAilments(_targetStats, _fireDamage, _iceDamage, _lightingDamage);

    }

    private void AttemptyToApplyAilments(CharacterStats _targetStats, int _fireDamage, int _iceDamage, int _lightingDamage)
    {
        bool canApplyIgnite = _fireDamage > _iceDamage && _fireDamage > _lightingDamage;
        bool canApplyChill = _iceDamage > _fireDamage && _iceDamage > _lightingDamage;
        bool canApplyShock = _lightingDamage > _iceDamage && _lightingDamage > _fireDamage;

        while (!canApplyIgnite && !canApplyChill && !canApplyShock)
        {
            if (Random.value < .5f && _fireDamage > 0)
            {
                canApplyIgnite = true;
                _targetStats.ApplyAilments(canApplyIgnite, canApplyChill, canApplyShock);
                return;
            }
            if (Random.value < .5f && _iceDamage > 0)
            {
                canApplyChill = true;
                _targetStats.ApplyAilments(canApplyIgnite, canApplyChill, canApplyShock);
                return;
            }
            if (Random.value < .5f && _lightingDamage > 0)
            {
                canApplyShock = true;
                _targetStats.ApplyAilments(canApplyIgnite, canApplyChill, canApplyShock);
                return;
            }
        }

        if (canApplyIgnite)
        {
            _targetStats.SetUpIgniteDamage(Mathf.RoundToInt(_fireDamage * .2f));
        }

        if (canApplyShock)
        {
            _targetStats.SetUpShockDamage(Mathf.RoundToInt(_lightingDamage * .1f));
        }

        _targetStats.ApplyAilments(canApplyIgnite, canApplyChill, canApplyShock);
    }

    public virtual void ApplyAilments(bool _ignite, bool _chill, bool _shock)
    {
        bool canApplyIgnite = !isIgnited && !isChilled && !isShocked;
        bool canApplyChill = !isIgnited && !isChilled && !isShocked;
        bool canApplyShock = !isIgnited && !isChilled;


        if (_ignite && canApplyIgnite)
        {
            isIgnited = _ignite;
            ignitedTimer = ailmentsDuration;

            fx.IgniteFxFor(ailmentsDuration);
        }
        if (_chill && canApplyChill)
        {
            isChilled = _chill;
            chilledTimer = ailmentsDuration;

            float slowPercentage = .2f;

            GetComponent<Entity>().SlowEntityBy(slowPercentage, ailmentsDuration);

            fx.ChillFxFor(ailmentsDuration);
        }
        if (_shock && canApplyShock)
        {
            if (!isShocked)
            {
                ApplyShock(_shock);
            }
            else
            {
                if (GetComponent<Player>() != null) return;
                HitNearestTargetWithShockStrike();
            }

        }

    }

    public void ApplyShock(bool _shock)
    {
        if (isShocked) return;

        isShocked = _shock;
        shockedTimer = ailmentsDuration;

        fx.ShockFxFor(ailmentsDuration);
    }

    private void ApplyIgniteDamage()
    {
        if (ignitedDamageTimer < 0)
        {
            DecreaseHealthBy(igniteDamage);

            if (currentHealth < 0 && !isDead) Die();

            ignitedDamageTimer = ignitedDamageCooldown;
        }
    }

    private void HitNearestTargetWithShockStrike()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 25);

        float closestDistance = Mathf.Infinity;

        Transform closestEnemy = null;

        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Enemy>() != null && Vector2.Distance(transform.position, hit.transform.position) > 1)
            {
                float distanceToEnemy = Vector2.Distance(transform.position, hit.transform.position);

                if (distanceToEnemy < closestDistance)
                {
                    closestDistance = distanceToEnemy;
                    closestEnemy = hit.transform;
                }
            }

            if (closestEnemy == null)
            {
                closestEnemy = transform;
            }

        }

        if (closestEnemy != null)
        {
            GameObject newThunderStrike = Instantiate(thunderStrikePrefab, transform.position, Quaternion.identity);
            newThunderStrike.GetComponent<ThunderStrikeController>().SetUp(shockDamage, closestEnemy.GetComponent<CharacterStats>());
        }
    }

    public void SetUpIgniteDamage(int _damage) => igniteDamage = _damage;
    public void SetUpShockDamage(int _damage) => shockDamage = _damage;

    #endregion

    #region Stats calculation
    private int CheckTargetArmor(CharacterStats _targetStats, int totalDamage)
    {
        if (isChilled)
        {
            totalDamage -= Mathf.RoundToInt(_targetStats.armor.GetValue() * .8f);
        }
        else
        {
            totalDamage -= _targetStats.armor.GetValue();
        }

        totalDamage = Mathf.Clamp(totalDamage, 0, int.MaxValue);
        return totalDamage;
    }

    private int CheckTargetResistance(CharacterStats _targetStats, int totalMagicDamage)
    {
        totalMagicDamage -= _targetStats.magicResistance.GetValue() + (_targetStats.intelligence.GetValue() * 3);
        totalMagicDamage = Mathf.Clamp(totalMagicDamage, 0, int.MaxValue);
        return totalMagicDamage;
    }

    private bool TargetCanAvoidAttack(CharacterStats _targetStats)
    {
        int totalEvasion = _targetStats.evasion.GetValue() + _targetStats.agility.GetValue();

        if (isShocked)
        {
            totalEvasion += 20;
        }

        if (UnityEngine.Random.Range(0, 100) < totalEvasion)
        {
            Debug.Log("Attack avoided");
            return true;
        }

        return false;
    }

    private bool CanCrit()
    {
        int totalCriticalChance = critChance.GetValue() + agility.GetValue();

        if (Random.Range(0, 100) <= totalCriticalChance)
        {
            return true;
        }

        return false;
    }

    private int CalculateCriticalDamage(int _damage)
    {
        float totalCritPower = (critPower.GetValue() + strength.GetValue()) * .01f;

        float critDamage = _damage * totalCritPower;

        return Mathf.RoundToInt(critDamage);
    }

    protected virtual void DecreaseHealthBy(int _damage)
    {
        currentHealth -= _damage;

        if (onHealthChanged != null)
        {
            onHealthChanged();
        }
    }
    #endregion

    protected virtual void Die()
    {
        isDead = true;
    }
}
