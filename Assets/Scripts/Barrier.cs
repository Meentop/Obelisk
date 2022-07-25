using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrier : Building
{
    [SerializeField] EnemyType barrierFor;

    public override void Click()
    {
        ui.EnableBarrierPanel();
        ui.SetBuildingForBarriering(this);
    }

    public void SetBarrierFor(EnemyType barrier)
    {
        barrierFor = barrier;
        buildingsGrid.StartUpdatePaths();
    }

    public EnemyType GetBarrierFor()
    {
        return barrierFor;
    }
}
