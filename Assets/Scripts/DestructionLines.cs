using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructionLines : MonoBehaviour
{
    public static DestructionLines Instance;
    LineRenderer lineRenderer;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    public void DrawRectangle(Vector3 start, Vector3 end)
    {
        lineRenderer.positionCount = 5;
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, new Vector3(end.x, 0, start.z));
        lineRenderer.SetPosition(2, end);
        lineRenderer.SetPosition(3, new Vector3(start.x, 0, end.z));
        lineRenderer.SetPosition(4, start);
    }

    public void Clear()
    {
        lineRenderer.positionCount = 0;
    }
}
