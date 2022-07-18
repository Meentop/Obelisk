using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutgoingPortal : Building
{
    PortalManager portalManager;

    [SerializeField] Transform[] outgoingPoints;

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
        ui.EnableOutgoingPortalPanel();
    }

    public override void Destroy()
    {
        portalManager.RemovePortal(this);
        base.Destroy();
    }

    public Transform[] GetOutgoingPoints()
    {
        return outgoingPoints;
    }
}
