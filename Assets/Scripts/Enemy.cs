using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    Cycles cycles;
    EnemyAttacks enemyAttacks;

    [HideInInspector] public int number;
    public EnemyType type;

    [SerializeField] int lvl, lvlHealthStep, lvlMagicStep;
    [SerializeField] float baseHp, baseMagic;
    [SerializeField] float maxHp, curHp, maxMagic, curMagic;
    [SerializeField] float baseSpeed;

    public int strength;

    public Transform center;
    [SerializeField] Transform hpBar, magicBar;
    [SerializeField] Vector2Int[] path;

    private void Start()
    {
        cycles = Cycles.Instance;
        enemyAttacks = EnemyAttacks.Instance;
        maxHp = baseHp + (lvl * lvlHealthStep);
        curHp = maxHp;
        maxMagic = baseMagic + (lvl * lvlMagicStep);
        curMagic = maxMagic;
        UpdateBars();
    }

    int curPosition = 0;
    public bool invulnerable { get; private set; } = false;
    private void FixedUpdate()
    {
        if (path.Length > 0)
        {
            Vector3 target = new Vector3(path[curPosition].x, 0, path[curPosition].y);
            transform.position = Vector3.MoveTowards(transform.position, target, baseSpeed * cycles.timeScale);
            if (Vector3.Distance(transform.position, target) < 0.001f)
            {
                curPosition++;
                CheckPortal();
            }
        }
    }

    void CheckPortal()
    {
        if (Vector3.Distance(transform.position, new Vector3(path[curPosition].x, 0, path[curPosition].y)) > 1.01f)
            invulnerable = true;
        else
            invulnerable = false;
    }

    public void SetPath(Vector2Int[] path)
    {
        this.path = path;
    }



    public bool HasMagic()
    {
        return curMagic != 0;
    }

    public void GetDamage(float damage)
    {
        curHp -= damage;
        UpdateBars();
        if (curHp <= 0)
            Destroy(gameObject);
    }

    public void GetMagicDamage(float damage)
    {
        curMagic -= damage;
        if (curMagic <= 0)
            curMagic = 0;
        UpdateBars();
    }

    void UpdateBars()
    {
        hpBar.localScale = new Vector3(curHp / maxHp, 1, 1);
        if(maxMagic > 0)
            magicBar.localScale = new Vector3(curMagic / maxMagic, 1, 1);
    }

    public float GetCurHP(int lvl)
    {
        return baseHp + (lvl * lvlHealthStep);
    }

    public float GetCurMagic(int lvl)
    {
        return baseMagic + (lvl * lvlMagicStep);
    }

    private void OnDestroy()
    {
        enemyAttacks.RemoveEnemy(this);
    }
}
