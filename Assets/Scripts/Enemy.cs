using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    Cycles cycles;
    EnemyAttacks enemyAttacks;

    [HideInInspector] public int number;
    public EnemyType type;

    [SerializeField] int lvl, lvlStep;
    [SerializeField] float baseHp;
    [SerializeField] float maxHp, curHp;
    [SerializeField] float baseSpeed;

    public int strength;

    public Transform center;
    [SerializeField] Transform hpBar;
    [SerializeField] Vector2Int[] path;

    private void Start()
    {
        cycles = Cycles.Instance;
        enemyAttacks = EnemyAttacks.Instance;
        maxHp = baseHp + (lvl * lvlStep);
        curHp = maxHp;
        UpdateHPBar();
    }

    int curPosition = 0;
    public bool invulnerable /*{ get; private set; } */= false;
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

    public void GetDamage(float damage)
    {
        curHp -= damage;
        UpdateHPBar();
        if (curHp <= 0)
            Destroy(gameObject);
    }

    void UpdateHPBar()
    {
        hpBar.localScale = new Vector3(curHp / maxHp, 1, 1);
    }

    public float GetCurHP(int lvl)
    {
        return baseHp + (lvl * lvlStep);
    }

    private void OnDestroy()
    {
        enemyAttacks.RemoveEnemy(this);
    }
}
