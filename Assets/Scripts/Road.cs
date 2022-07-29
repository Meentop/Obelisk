using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Road : MonoBehaviour
{
    UI ui;
    BuildingsGrid buildingsGrid;
    Resources resources;
    [HideInInspector] public Outline outline;
    MeshRenderer mesh;
    [SerializeField] Material defaultMaterial;
    public bool undestroyable, unmovable;

    private void Awake()
    {
        mesh = GetComponent<MeshRenderer>();
        outline = GetComponent<Outline>();
    }

    private void Start()
    {
        ui = UI.Instance;
        buildingsGrid = BuildingsGrid.Instance;
        resources = Resources.Instance;
    }

    bool colored = false;
    private void OnMouseEnter()
    {
        if (ui.EnabledBuildingsGrid() && buildingsGrid.buildingsMode == BuildingsMode.Movement && !unmovable && buildingsGrid.IsFlyingRoadNull())
        {
            SetTransparent(true);
            colored = true;
        }
    }

    private void OnMouseExit()
    {
        if (colored)
        {
            SetNormal();
            colored = false;
        }
    }

    public void Click()
    {
        ui.EnableRoadPanle();
    }

    public void SetTransparent(bool available)
    {
        if (available)
            mesh.material.color = Color.green;
        else
            mesh.material.color = Color.red;
    }

    public void SetNormal()
    {
        if(mesh != null)
            mesh.material.color = Color.white;
    }

    public void Place()
    {
        resources.TakeResource(Resource.Wood, 5);
    }

    public void SetDefaultMaterial()
    {
        mesh.material = defaultMaterial;
    }

    public void SetMaterial(Material newMaterial)
    {
        mesh.material = newMaterial;
    }

    public void Destroy()
    {
        resources.AddResource(Resource.Wood, 5);
        buildingsGrid.RemoveRoad(this);
        buildingsGrid.ClearRoadGrid(transform.localPosition);
        buildingsGrid.UpdateRoadTilesArround(transform.localPosition);
        Destroy(gameObject);
    }
}
