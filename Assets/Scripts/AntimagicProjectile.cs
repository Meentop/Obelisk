using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntimagicProjectile : Projectile
{
    protected override void OnHit()
    {
        target.GetMagicDamage(damage);
        Destroy(gameObject);
    }
}
