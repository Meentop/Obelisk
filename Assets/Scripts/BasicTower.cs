using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicTower : CombatBuilding
{
    [SerializeField] GameObject rangeChar, damageChar, attackSpeedChar, rotationSpeedChar, projectileSpeedChar;

    public override void InitCharacteristics()
    {
        ui.SpawnCharacteristic(rangeChar, radius);
        ui.SpawnCharacteristic(damageChar, damage);
        ui.SpawnCharacteristic(attackSpeedChar, curAttackSpeed);
        ui.SpawnCharacteristic(rotationSpeedChar, rotateSpeed * 50);
        ui.SpawnCharacteristic(projectileSpeedChar, projectileSpeed * 50);
    }
}
