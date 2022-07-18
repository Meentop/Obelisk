using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    WallManager wallsManager;

    [HideInInspector] public Fortification startColumn, endColumn;

    private void Start()
    {
        wallsManager = WallManager.Instance;
        wallsManager.AddWall(this);
    }

    private void OnDestroy()
    {
        wallsManager.RemoveWall(this);
    }
}
