using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Transportation : MonoBehaviour
{
    [SerializeField] Transform[] outgoingPoints = new Transform[2];

    public bool HasOutgoingPoints()
    {
        return outgoingPoints[0] != null;
    }

    public Vector2Int GetOutgoingPoint()
    {
        int rand = Random.Range(0, 2);
        return new Vector2Int((int)outgoingPoints[rand].position.x, (int)outgoingPoints[rand].position.z);
    }

    public void SetOutgoingPoints(Transform[] points)
    {
        outgoingPoints[0] = points[0];
        outgoingPoints[1] = points[1];
    }

    public void Clear()
    {
        outgoingPoints[0] = null;
        outgoingPoints[1] = null;
    }
}
