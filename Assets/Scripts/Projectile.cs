using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    Cycles cycles;

    Enemy target;
    float speed;
    float damage;

    [SerializeField] float[] damageModifiers;

    private void Awake()
    {
        cycles = Cycles.Instance;
    }

    private void FixedUpdate()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }
        transform.forward = (target.center.transform.position - transform.position).normalized;
        transform.position = Vector3.MoveTowards(transform.position, target.center.transform.position, speed * cycles.timeScale);
        if(Vector3.Distance(transform.position, target.center.transform.position) <= 0)
        {
            target.GetDamage(damage * damageModifiers[(int)target.type]);
            Destroy(gameObject);
        }
    }

    public void Inisialization(Enemy target, float speed, float damage)
    {
        this.target = target;
        this.speed = speed;
        this.damage = damage;
    }
}
