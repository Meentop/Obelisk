using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseRay : MonoBehaviour
{
    [SerializeField] LayerMask building;

    UI ui;

    private void Awake()
    {
        ui = GetComponent<UI>();
    }

    private void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);

        RaycastHit hit;

        if (Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, building))
                hit.collider.GetComponent<Building>().Click();
            else if (groundPlane.Raycast(ray, out float position))
                ui.ClickOnGround();
        }
    }
}
