using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Storage : Building
{
    [SerializeField] Resource resource;
    [SerializeField] int capasity;
  
    public override void Place()
    {
        base.Place();
        resources.AddCapasity(resource, capasity);
    }

    public override void Click()
    {
        ui.EnableStoragePanel(buildingsName, (int)resource, capasity);
    }
}
