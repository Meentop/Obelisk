using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseRay : MonoBehaviour
{
    public static MouseRay Instance;

    [SerializeField] LayerMask building, person;

    UI ui;
    OutlineManager outlineManager;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        ui = UI.Instance;
        outlineManager = OutlineManager.Instance;
    }

    public Vector3 startMousePos, curMousePos;
    public Person grabPerson;

    private void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);

        RaycastHit hit;

        if (Input.GetMouseButtonDown(0))
        {
            if(Physics.Raycast(ray, out hit, Mathf.Infinity, person))
            {
                startMousePos = Input.mousePosition;
                grabPerson = hit.collider.GetComponent<Person>();
            }
            else if (Physics.Raycast(ray, out hit, Mathf.Infinity, building))
            {
                hit.collider.GetComponent<Building>().Click();
                outlineManager.EnableOutline(hit.collider.GetComponent<Building>().outline);
            }
            else if (groundPlane.Raycast(ray, out float position))
            {
                ui.ClickOnGround();
                outlineManager.DisableOutline();
            }
        }
        else if(Input.GetMouseButton(0))
        {
            if (grabPerson != null)
            {
                curMousePos = Input.mousePosition;
            }
            if(Vector3.Distance(startMousePos, curMousePos) != 0f && groundPlane.Raycast(ray, out float position) && ui.EnabledStartUI())
            {
                grabPerson.nextPosition = ray.GetPoint(position);
            }
        }
        else if(Input.GetMouseButtonUp(0))
        {
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, person) && Vector3.Distance(startMousePos, curMousePos) == 0f)
            {
                grabPerson.Click();
                outlineManager.EnableOutline(grabPerson.GetComponent<Outline>());
            }
            else
            {
                if (grabPerson != null && Physics.Raycast(ray, out hit, Mathf.Infinity, building))
                {
                    if (hit.collider.GetComponent<IndustrialBuilding>().HasWorkplace())
                    {
                        hit.collider.GetComponent<IndustrialBuilding>().Click();
                        outlineManager.EnableOutline(hit.collider.GetComponent<Outline>());
                        hit.collider.GetComponent<IndustrialBuilding>().AddWorker(grabPerson.gameObject);
                    }
                    else
                        ui.NoWorkplace();
                }
            }
            startMousePos = Vector3.zero;
            curMousePos = Vector3.zero;
            grabPerson = null;
        }
    }

    public Vector3 GetMousePositionOnPlane()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);

        if (groundPlane.Raycast(ray, out float position))
            return ray.GetPoint(position);
        else
            return Vector3.zero;
    }
}
