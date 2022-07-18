using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrier : Building
{
    [SerializeField] BarrierFor barrierFor;

    public override void Click()
    {
        ui.SetBuildingForBarriering(this);
    }

    public void SetBarrierFor(BarrierFor barrier)
    {
        barrierFor = barrier;
        buildingsGrid.StartUpdatePaths();
    }

    public BarrierFor GetBarrierFor()
    {
        return barrierFor;
    }
}

public enum BarrierFor
{
    Ork,
    Troll,
    Gremline
}
