using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class Enemy : MonoBehaviour
{
    NavMeshAgent agent;
    Cycles cycles;
    EnemyAttacks enemyAttacks;

    [SerializeField] int lvl, lvlStep;
    [SerializeField] float baseHp;
    [SerializeField] float maxHp, curHp;

    [SerializeField] float baseSpeed;

    [SerializeField] int power;

    private void Start()
    {
        cycles = Cycles.Instance;
        enemyAttacks = EnemyAttacks.Instance;
        agent = GetComponent<NavMeshAgent>();
        agent.destination = FindObjectOfType<EmpirePortal>().transform.position;
        maxHp = baseHp + (lvl * lvlStep);
        curHp = maxHp;
    }

    private void Update()
    {
        agent.speed = baseSpeed * cycles.timeScale;
    }

    public void GetDamage(float damage)
    {
        curHp -= damage;
        if (curHp <= 0)
            Destroy(gameObject);
    }

    private void OnDestroy()
    {
        enemyAttacks.RemoveEnemy(this);
    }
}
