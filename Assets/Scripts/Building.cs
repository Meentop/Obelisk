using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Building : MonoBehaviour
{
    [SerializeField] List<Renderer> renderers = new List<Renderer>();
    public Vector2Int size = Vector2Int.one;
    
    [SerializeField] List<Cost> cost = new List<Cost>();

    Transform model;
    protected Resources resources;
    protected UI ui;

    private void Awake()
    {
        resources = FindObjectOfType<Resources>();
        ui = FindObjectOfType<UI>();
        model = transform.GetChild(0);
    }

    protected virtual void Start()
    {
        
    }

    public void SetTransparent(bool available)
    {
        if (available)
        {
            foreach (Renderer renderer in renderers)
            {
                renderer.material.color = Color.green;
            }
        }
        else
        {
            foreach (Renderer renderer in renderers)
            {
                renderer.material.color = Color.red;
            }
        }
    }

    public void SetNormal()
    {
        foreach (Renderer renderer in renderers)
        {
            renderer.material.color = Color.white;
        }
    }

    public void RotateModel(float degrees)
    {
        model.rotation = Quaternion.Euler(new Vector3(model.rotation.eulerAngles.x, model.rotation.eulerAngles.y + degrees, model.rotation.eulerAngles.z));
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