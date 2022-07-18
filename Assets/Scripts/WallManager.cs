using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallManager : MonoBehaviour
{
    public static WallManager Instance;

    List<Wall> walls = new List<Wall>();

    private void Awake()
    {
        Instance = this;
    }

    public void AddWall(Wall wall)
    {
        walls.Add(wall);
    }

    public void RemoveWall(Wall wall)
    {
        walls.Remove(wall);
    }

    public void DestroyWalls(Fortification column)
    {
        foreach (Wall wall in walls)
        {
            if(wall.startColumn == column || wall.endColumn == column)
            {
                Destroy(wall.gameObject);
            }  
        }
    }
}
