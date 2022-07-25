using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Projectile : MonoBehaviour
{
    protected Cycles cycles;

    protected Enemy target;
    protected float speed;
    protected float damage;

    [SerializeField] protected float[] damageModifiers;

    private void Awake()
    {
        cycles = Cycles.Instance;
    }

    void FixedUpdate()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }
        transform.forward = (target.center.transform.position - transform.position).normalized;
        transform.position = Vector3.MoveTowards(transform.position, target.center.transform.position, speed * cycles.timeScale);
        if (Vector3.Distance(transform.position, target.center.transform.position) <= 0.001f)
            OnHit();
    }

    protected virtual void OnHit()
    {
        target.GetDamage(damage * damageModifiers[(int)target.type]);
        Destroy(gameObject);
    }

    public void BasicInit(Enemy target, float speed, float damage)
    {
        this.target = target;
        this.speed = speed;
        this.damage = damage;
    }
}
