using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fortification : Building
{
    WallManager wallsManager;

    [SerializeField] GameObject wall;

    Vector3[] directions = new Vector3[] { Vector3.back, Vector3.forward, Vector3.right, Vector3.left };

    protected override void Start()
    {
        base.Start();
        wallsManager = WallManager.Instance;
    }

    public override void Place()
    {
        base.Place();
        SetWalls();
    }

    public void SetWalls()
    {
        StartCoroutine(SetWallsCor());
    }

    IEnumerator SetWallsCor()
    {
        yield return new WaitForSeconds(0.1f);
        for (int i = 0; i < 4; i++)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, directions[i], out hit, 1.01f, LayerMask.GetMask("Building")))
            {
                if (hit.collider.GetComponent<Fortification>() && hit.collider.GetComponent<Fortification>() != this)
                {
                    Wall wall = Instantiate(this.wall, transform.position, Quaternion.LookRotation(directions[i])).GetComponent<Wall>();
                    wall.startColumn = this;
                    wall.endColumn = hit.collider.GetComponent<Fortification>();
                }
            }
        }
    }

    public override void Click()
    {
        ui.EnableFortificationPanel(buildingsName);
    }

    public override void Destroy()
    {
        base.Destroy();
        wallsManager.DestroyWalls(this);
    }
}
