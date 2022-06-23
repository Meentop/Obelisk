using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resources : MonoBehaviour
{
    public static Resources Instance;

    float[] resources = new float[4] { 0, 0, 0, 0 };

    int[] resourcesCapasity = new int[4] { 0, 0, 0, 0};

    UI ui;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        ui = UI.Instance;
        for (int i = 0; i < resources.Length; i++)
        {
            ui.UpdateResourceNumber((Resource)i, resources[i], resourcesCapasity[i]);
        }
        AddCapasity(Resource.Food, 100);
        AddCapasity(Resource.Wood, 150);
        AddCapasity(Resource.Metal, 150);
        AddResource(Resource.Food, 1);
        AddResource(Resource.Wood, 100);
        AddResource(Resource.Metal, 50);
        AddResource(Resource.ResearchPoint, 999);
    }

    //Resources

    public void AddResource(Resource resource, float add)
    {
        resources[(int)resource] += add;
        if (resources[(int)resource] > resourcesCapasity[(int)resource] && resource != Resource.ResearchPoint)
        {
            resources[(int)resource] = resourcesCapasity[(int)resource];
            ui.StorageIsFull();
        }
        ui.UpdateResourceNumber(resource, resources[(int)resource], resourcesCapasity[(int)resource]);
    }

    public bool HasResources(Resource resource, float take)
    {
        return resources[(int)resource] >= take;
    }

    public void TakeResource(Resource resource, float take)
    {
        resources[(int)resource] -= take;
        ui.UpdateResourceNumber(resource, resources[(int)resource], resourcesCapasity[(int)resource]);
    }

    public int GetResource(Resource resource)
    {
        return (int)resources[(int)resource];
    }

    //Capasity

    public void AddCapasity(Resource resource, int add)
    {
        resourcesCapasity[(int)resource] += add;
        ui.UpdateResourceNumber(resource, resources[(int)resource], resourcesCapasity[(int)resource]);
    }

    public void RemoveCapasity(Resource resource, int remove)
    {
        resourcesCapasity[(int)resource] -= remove;
        ui.UpdateResourceNumber(resource, resources[(int)resource], resourcesCapasity[(int)resource]);
    }
}

public enum Resource 
{ 
    Food, 
    Wood,
    Metal,
    ResearchPoint
}

