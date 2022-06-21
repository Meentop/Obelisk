using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingButton : MonoBehaviour
{
    [SerializeField] List<ButtonCost> cost = new List<ButtonCost>();
    Button button;
    Resources resources;

    private void Start()
    {
        button = GetComponent<Button>();
        resources = Resources.Instance;
        for (int i = 0; i < cost.Count; i++)
        {
            cost[i].text.text = cost[i].cost.ToString();
        }
    }

    private void Update()
    {
        for (int i = 0; i < cost.Count; i++)
        {
            if (resources.HasResources(cost[i].resource, cost[i].cost))
                cost[i].text.color = Color.black;
            else
                cost[i].text.color = Color.red;
        }
        for (int i = 0; i < cost.Count; i++)
        {
            if (!resources.HasResources(cost[i].resource, cost[i].cost))
            {
                button.interactable = false;
                return;
            }
        }
        button.interactable = true;
    }
}

[Serializable]
class ButtonCost
{
    public Resource resource;
    public int cost;
    public Text text;
}