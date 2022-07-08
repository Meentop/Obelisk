using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarPortal : Building
{
    EnemyAttacks enemyAttacks;
    [SerializeField] PortalType portalType;

    protected override void Start()
    {
        base.Start();
        enemyAttacks = EnemyAttacks.Instance;
        buildingsGrid.PlaceBuilding(this, (int)transform.position.x, (int)transform.position.z);
    }

    public override void Click()
    {
        ui.EnableWarPortalPanel();
        enemyAttacks.SetUIWaves(portalType);
    }
}
