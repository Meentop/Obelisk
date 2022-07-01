using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Storage : Building
{
    public Resource resource;
    public int capasity;
    public int resourcesInStorage = 0;

    public override void Place()
    {
        base.Place();
        resources.AddCapasity(resource, capasity);
    }

    public override void Click()
    {
        ui.EnableStoragePanel(buildingsName, (int)resource, capasity);
    }

    public override void Destroy()
    {
        
    }
}
