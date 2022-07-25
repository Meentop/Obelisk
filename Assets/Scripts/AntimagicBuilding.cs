using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AntimagicBuilding : CombatBuilding
{
    [SerializeField] GameObject rangeChar, magicDamage, attackSpeedChar, rotationSpeedChar, projectileSpeedChar;

    public override void InitCharacteristics()
    {
        ui.SpawnCharacteristic(rangeChar, radius);
        ui.SpawnCharacteristic(magicDamage, damage);
        ui.SpawnCharacteristic(attackSpeedChar, curAttackSpeed);
        ui.SpawnCharacteristic(rotationSpeedChar, rotateSpeed * 50);
        ui.SpawnCharacteristic(projectileSpeedChar, projectileSpeed * 50);
    }

    protected override void SetEnemies()
    {
        if (enemyAttacks.IsEnemyAttack() && person != null)
        {
            Collider[] colliders = Physics.OverlapSphere(center.position, radius, LayerMask.GetMask("Enemy"));
            IEnumerable<Collider> coll = colliders.Where(enemy => enemy.GetComponent<Enemy>().invulnerable == false).Where(enemy => enemy.GetComponent<Enemy>().HasMagic());
            enemies = new Enemy[coll.Count()];
            for (int i = 0; i < coll.Count(); i++)
                enemies[i] = coll.ElementAt(i).GetComponent<Enemy>();
        }
    }
}
