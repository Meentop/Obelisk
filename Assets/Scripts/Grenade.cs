using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : Projectile
{
    float explosionRange;

    protected override void OnHit()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRange, LayerMask.GetMask("Enemy"));
        foreach (Collider collider in colliders)
        {
            collider.GetComponent<Enemy>().GetDamage(damage * damageModifiers[(int)collider.GetComponent<Enemy>().type]);
        }
        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(transform.position, explosionRange);
    }

    public void InitExplosionRange(float explosionRange)
    {
        this.explosionRange = explosionRange;
    }
}
