using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class Enemy : MonoBehaviour
{
    NavMeshAgent agent;
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

    private void Start()
    {
        cycles = Cycles.Instance;
        enemyAttacks = EnemyAttacks.Instance;
        agent = GetComponent<NavMeshAgent>();
        agent.destination = FindObjectOfType<EmpirePortal>().center.position;
        maxHp = baseHp + (lvl * lvlStep);
        curHp = maxHp;
        UpdateHPBar();
    }

    private void Update()
    {
        agent.speed = baseSpeed * cycles.timeScale;
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
