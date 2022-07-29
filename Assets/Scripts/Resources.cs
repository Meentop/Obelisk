using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resources : MonoBehaviour
{
    public static Resources Instance;

    int[] resources = new int[5] { 0, 0, 0, 0, 0 };

    int[] resourcesCapasity = new int[3] { 0, 0, 0 };

    UI ui;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        ui = UI.Instance;
        for (int i = 0; i < resources.Length; i++)
            UpdateResourceNumber((Resource)i);
        AddCapasity(Resource.Food, 50);
        AddCapasity(Resource.Wood, 400);
        AddCapasity(Resource.Metal, 400);
        AddResource(Resource.Food, 2);
        AddResource(Resource.Wood, 450);
        AddResource(Resource.Metal, 400);
        AddResource(Resource.ResearchPoint, 999);
        AddResource(Resource.MatterGenerator, 5);
    }

    //Resources

    public void AddResource(Resource resource, int add)
    {
        resources[(int)resource] += add;
        if (resource != Resource.ResearchPoint && resource != Resource.MatterGenerator && resources[(int)resource] > resourcesCapasity[(int)resource])
        {
            resources[(int)resource] = resourcesCapasity[(int)resource];
            ui.StorageIsFull();
        }
        UpdateResourceNumber(resource);
    }

    public bool HasResources(Resource resource, int take)
    {
        return resources[(int)resource] >= take;
    }

    public void TakeResource(Resource resource, int take)
    {
        resources[(int)resource] -= take;
        UpdateResourceNumber(resource);
    }

    public int GetResource(Resource resource)
    {
        return resources[(int)resource];
    }

    //Capasity

    public void AddCapasity(Resource resource, int add)
    {
        resourcesCapasity[(int)resource] += add;
        ui.UpdateResourceNumber(resource, resources[(int)resource], resourcesCapasity[(int)resource]);
    }


    void UpdateResourceNumber(Resource resource)
    {
        if (resource != Resource.ResearchPoint && resource != Resource.MatterGenerator)
            ui.UpdateResourceNumber(resource, resources[(int)resource], resourcesCapasity[(int)resource]);
        else
            ui.UpdateResourceNumber(resource, resources[(int)resource]);
    }
}

public enum Resource 
{ 
    Food, 
    Wood,
    Metal,
    ResearchPoint,
    MatterGenerator
}

