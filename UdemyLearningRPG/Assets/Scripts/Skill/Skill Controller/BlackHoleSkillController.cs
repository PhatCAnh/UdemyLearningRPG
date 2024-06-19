using System.Collections.Generic;
using UnityEngine;

public class BlackHoleSkillController : MonoBehaviour
{
    [SerializeField] private GameObject hotKeyPrefab;
    [SerializeField] private List<KeyCode> listKeyCode;

    private float maxSize;
    private float growSpeed;
    private float shrinkSpeed;

    private bool canGrow = true;
    private bool canShrink;
    private bool canCreateHotKey = true;
    private bool cloneAttackReleased;
    private bool playerDisapear = true;

    private int amountOfAttacks;
    private float cloneAttackCooldown;
    private float cloneAttackTimer;
    private float skillTimer;

    private List<Transform> targets = new List<Transform>();
    private List<GameObject> createdHotKey = new List<GameObject>();

    public bool playerCanExitState { get; private set; }

    public void SetUpBlackHole(float _maxSize, float _growSpeed, float _shrinkSpeed, int _amountOfAttacks, float _cloneAttackCooldown, float _skillDuration)
    {
        maxSize = _maxSize;
        growSpeed = _growSpeed;
        shrinkSpeed = _shrinkSpeed;
        amountOfAttacks = _amountOfAttacks;
        cloneAttackCooldown = _cloneAttackCooldown;
        skillTimer = _skillDuration;
    }

    private void Update()
    {
        cloneAttackTimer -= Time.deltaTime;
        skillTimer -= Time.deltaTime;

        if (skillTimer < 0)
        {
            skillTimer = Mathf.Infinity;

            if (targets.Count > 0) ReleaseCloneAttack();
            else FinishAbility();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            ReleaseCloneAttack();
        }

        CloneAttackLogic();

        if (canGrow && !canShrink)
        {
            transform.localScale = Vector2.Lerp(transform.localScale, new Vector2(maxSize, maxSize), growSpeed * Time.deltaTime);
        }

        if (canShrink)
        {
            transform.localScale = Vector2.Lerp(transform.localScale, new Vector2(-1, -1), shrinkSpeed * Time.deltaTime);

            if (transform.localScale.x < 0)
                Destroy(gameObject);
        }

    }

    private void ReleaseCloneAttack()
    {
        if (targets.Count <= 0) return;

        DestoyHotKey();
        cloneAttackReleased = true;
        canCreateHotKey = false;

        if (playerDisapear)
        {
            playerDisapear = false;
            PlayerManager.instance.player.MakeTransparent(true);
        }
    }

    private void CloneAttackLogic()
    {
        if (cloneAttackTimer < 0 && cloneAttackReleased && amountOfAttacks > 0)
        {
            cloneAttackTimer = cloneAttackCooldown;

            int randomIndex = Random.Range(0, targets.Count);

            float xOffset = (Random.Range(0, 100) > 50) ? 2 : -2;

            SkillManager.instance.cloneSkill.CreateClone(targets[randomIndex], new Vector3(xOffset, 0));

            amountOfAttacks--;
            if (amountOfAttacks <= 0)
            {
                Invoke("FinishAbility", 1f);
            }
        }
    }

    private void FinishAbility()
    {
        DestoyHotKey();
        playerCanExitState = true;
        canShrink = true;
        cloneAttackReleased = false;
    }

    private void DestoyHotKey()
    {
        if (createdHotKey.Count <= 0) return;

        for (int i = 0; i < createdHotKey.Count; i++)
        {
            Destroy(createdHotKey[i]);
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Enemy>() != null)
        {
            collision.GetComponent<Enemy>().FreezeTimer(true);
            CreateHotKey(collision);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<Enemy>() != null)
        {
            collision.GetComponent<Enemy>().FreezeTimer(false);
        }
    }

    private void CreateHotKey(Collider2D collision)
    {
        if (listKeyCode.Count <= 0)
        {
            Debug.Log("Not Enough Hot Key in listKeyCode");
            return;
        }

        if (!canCreateHotKey) return;


        GameObject newHotKey = Instantiate(hotKeyPrefab, collision.transform.position + new Vector3(0, 2), Quaternion.identity);
        createdHotKey.Add(newHotKey);

        KeyCode choosenKey = listKeyCode[Random.Range(0, listKeyCode.Count)];
        listKeyCode.Remove(choosenKey);

        BlackHoleHotKeyController newHotKeyScript = newHotKey.GetComponent<BlackHoleHotKeyController>();

        newHotKeyScript.SetUpHotKey(choosenKey, collision.transform, this);
    }

    public void AddEnemyToList(Transform _enemyTransform) => targets.Add(_enemyTransform);

}
