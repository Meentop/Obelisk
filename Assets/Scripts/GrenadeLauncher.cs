using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeLauncher : CombatBuilding
{
    [Header("Unique characteristics")]
    [SerializeField] float explosionRange;
    [SerializeField] GameObject rangeChar, damageChar, attackSpeedChar, rotationSpeedChar, projectileSpeedChar, explosionRangeChar;

    protected override void Shoot()
    {
        if (person != null)
        {
            Projectile proj = Instantiate(projectile, shootPoint.position, Quaternion.identity).GetComponent<Projectile>();
            proj.BasicInit(target, projectileSpeed, damage);
            proj.GetComponent<Grenade>().InitExplosionRange(explosionRange);
        }
    }

    public override void InitCharacteristics()
    {
        ui.SpawnCharacteristic(rangeChar, radius);
        ui.SpawnCharacteristic(damageChar, damage);
        ui.SpawnCharacteristic(attackSpeedChar, curAttackSpeed);
        ui.SpawnCharacteristic(rotationSpeedChar, rotateSpeed * 50);
        ui.SpawnCharacteristic(projectileSpeedChar, projectileSpeed * 50);
        ui.SpawnCharacteristic(explosionRangeChar, explosionRange);
    }
}
