using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarPortal : Building
{
    protected override void Start()
    {
        base.Start();
        buildingsGrid.PlaceBuilding(this, (int)transform.position.x, (int)transform.position.z);
    }

    public override void Click()
    {
        ui.EnableWarPortalPanel();
    }
}
