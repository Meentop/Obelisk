using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncomingPortal : Building
{
    PortalManager portalManager;

    [SerializeField] GameObject unrelated;
    [SerializeField] Transportation transportation;

    protected override void Start()
    {
        base.Start();
        portalManager = PortalManager.Instance;
    }

    public override void Place()
    {
        base.Place();
        portalManager.AddPortal(this);
    }

    public override void Click()
    {
        ui.EnableIncomingPortalPanel();
    }

    public override void Destroy()
    {
        portalManager.RemovePortal(this);
        base.Destroy();
    }

    public void SetRelation(OutgoingPortal outgoingPortal)
    {
        unrelated.SetActive(false);
        transportation.SetOutgoingPoints(outgoingPortal.GetOutgoingPoints());
        buildingsGrid.StartUpdatePaths();
    }

    public bool HasRelation()
    {
        return transportation.HasOutgoingPoints();
    }

    public void BreakRelation()
    {
        unrelated.SetActive(true);
        transportation.Clear();
    }
}
