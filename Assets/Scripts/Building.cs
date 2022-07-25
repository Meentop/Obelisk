using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Building : MonoBehaviour
{
    public string buildingsName, description;

    public Transform center;
    protected Resources resources;
    protected UI ui;
    protected BuildingsGrid buildingsGrid;
    [HideInInspector] public Outline outline;

    [SerializeField] List<Renderer> renderers = new List<Renderer>();
    public Vector2Int size = Vector2Int.one;
    
    [SerializeField] List<Cost> cost = new List<Cost>();

    public bool undestroyable, unmovable;

    protected virtual void Start()
    {
        resources = Resources.Instance;
        center = transform.GetChild(0);
        outline = GetComponent<Outline>();
        buildingsGrid = BuildingsGrid.Instance;
        ui = UI.Instance;
        nextRotation = center.localRotation;
    }

    protected virtual void Update()
    {
        if(center.localRotation != nextRotation)
            center.localRotation = Quaternion.Lerp(center.localRotation, nextRotation, 30 * Time.deltaTime);
    }

    bool colored = false;
    private void OnMouseEnter()
    {
        if(ui.EnabledBuildingsGrid() && buildingsGrid.buildingsMode == BuildingsMode.Movement && !unmovable && buildingsGrid.IsFlyingBuildingNull())
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

    public virtual void SetPosition(Vector3 pos)
    {
        transform.position = pos;
    }

    public void SetTransparent(bool available)
    {
        if (available)
        {
            foreach (Renderer renderer in renderers)
                renderer.material.color = Color.green;
        }
        else
        {
            foreach (Renderer renderer in renderers)
                renderer.material.color = Color.red;
        }
    }

    public void SetNormal()
    {
        foreach (Renderer renderer in renderers)
        {
            renderer.material.color = Color.white;
        }
    }

    Quaternion nextRotation;
    public void RotateModel(float degrees)
    {
        nextRotation = Quaternion.Euler(new Vector3(0, nextRotation.eulerAngles.y + degrees, 0));
    }

    public bool HasResources()
    {
        for (int i = 0; i < cost.Count; i++)
        {
            if (!resources.HasResources(cost[i].resource, cost[i].cost))
                return false;
        }
        return true;
    }

    public virtual void Place()
    {
        for (int i = 0; i < cost.Count; i++)
        {
            resources.TakeResource(cost[i].resource, cost[i].cost);
        }
    }

    public abstract void Click();


    public virtual void Destroy()
    {
        for (int i = 0; i < cost.Count; i++)
        {
            resources.AddResource(cost[i].resource, cost[i].cost);
        }
        buildingsGrid.RemoveBuilding(this);
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                Gizmos.DrawCube(transform.position + new Vector3(x, 0, y), new Vector3(1f, .1f, 1f));
            }
        }
    }
}

[Serializable]
class Cost
{
    public Resource resource;
    public int cost;
}

public interface IWorkplace
{
    public bool HasWorkplace();

    public void AddWorker(GameObject person);

    public Person RemoveWorker(int number);

    public int FindIndex(Person person);
}