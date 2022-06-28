using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Fortification : Building
{
    public override void Place()
    {
        base.Place();
    }

    public override void Click()
    {
        ui.EnableFortificationPanel(buildingsName);
    }
}
