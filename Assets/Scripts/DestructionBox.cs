using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructionBox : MonoBehaviour
{
    public static DestructionBox Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void SetPosition(Vector3 start, Vector3 end)
    {
        float sizeX = end.x - start.x;
        float sizeZ = end.z - start.z;
        transform.position = new Vector3(start.x + (sizeX / 2), 0.5f, start.z + (sizeZ / 2));
        transform.localScale = new Vector3(sizeX, 1, sizeZ);
    }

    List<GameObject> selectedBuildings = new List<GameObject>();

    bool roadMode = false;
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<Building>())
        {
            if (!roadMode)
            {
                roadMode = false;
                Building building = collision.gameObject.GetComponent<Building>();
                if (!building.undestroyable)
                {
                    selectedBuildings.Add(collision.gameObject);
                    building.SetTransparent(false);
                }
            }
        }
        else if (collision.gameObject.GetComponent<Road>())
        {
            if (roadMode)
            {
                roadMode = true;
                Road road = collision.gameObject.GetComponent<Road>();
                if (!road.undestroyable)
                {
                    selectedBuildings.Add(collision.gameObject);
                    road.SetTransparent(false);
                }
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.GetComponent<Building>())
        {
            Building building = collision.gameObject.GetComponent<Building>();
            if (!building.undestroyable)
            {
                selectedBuildings.Remove(collision.gameObject);
                building.SetNormal();
            }
        }
        else if (collision.gameObject.GetComponent<Road>())
        {
            Road road = collision.gameObject.GetComponent<Road>();
            if (!road.undestroyable)
            {
                selectedBuildings.Remove(collision.gameObject);
                road.SetNormal();
            }
        }
    }

    public List<GameObject> GetSelectedBuildings()
    {
        return selectedBuildings;
    }

    public void SetRoadMode(bool value)
    {
        roadMode = value;
    }

    public void ClearSelectedBuildings()
    {
        selectedBuildings.Clear();
        SetRoadMode(false);
    }
}
