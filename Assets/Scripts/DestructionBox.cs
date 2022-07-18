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

    List<Building> selectedBuildings = new List<Building>();

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<Building>())
        {
            Building building = collision.gameObject.GetComponent<Building>();
            if (!building.undestroyable)
            {
                selectedBuildings.Add(building);
                building.SetTransparent(false);
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
                selectedBuildings.Remove(building);
                building.SetNormal();
            }
        }
    }

    public List<Building> GetSelectedBuildings()
    {
        return selectedBuildings;
    }

    public void ClearSelectedBuildings()
    {
        selectedBuildings.Clear();
    }
}
