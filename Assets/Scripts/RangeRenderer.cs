using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeRenderer : MonoBehaviour
{
    public static RangeRenderer Instance;
    LineRenderer lineRenderer;
    UI ui;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        ui = UI.Instance;
    }

    public void DrawRange(int steps, CombatBuilding combat)
    {
        print("draw range");
        if (combat.center != null)
        {
            Clear();

            lineRenderer.positionCount = steps + 1;

            for (int i = 0; i <= steps; i++)
            {
                float circumferenceProgress = (float)i / steps;

                float curRadian = circumferenceProgress * 2 * Mathf.PI;

                float xScaled = Mathf.Cos(curRadian);
                float yScaled = Mathf.Sin(curRadian);

                float x = xScaled * combat.radius + combat.center.position.x;
                float y = yScaled * combat.radius + combat.center.position.z;

                Vector3 curPos = new Vector3(x, 0, y);

                lineRenderer.SetPosition(i, curPos);
            }
        }
    }

    public void Clear()
    {
        if (!ui.cursorOnButton)
            lineRenderer.positionCount = 0;
    }
}
