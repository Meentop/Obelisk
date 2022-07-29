using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseRay : MonoBehaviour
{
    public static MouseRay Instance;

    [SerializeField] LayerMask building, person;

    UI ui;
    OutlineManager outlineManager;
    BuildingsGrid buildingsGrid;
    RangeRenderer rangeRenderer;
    EnemyAttacks enemyAttacks;
    WallManager wallManager;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        ui = UI.Instance;
        outlineManager = OutlineManager.Instance;
        buildingsGrid = BuildingsGrid.Instance;
        rangeRenderer = RangeRenderer.Instance;
        enemyAttacks = EnemyAttacks.Instance;
        wallManager = WallManager.Instance;
    }

    public Vector3 startMousePos, curMousePos;
    public Person grabPerson;
    public Building selectedBulding = null;

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
                rangeRenderer.Clear();
            }
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, building))
            {
                if (hit.collider.GetComponent<Building>())
                {
                    Building building = hit.collider.GetComponent<Building>();
                    building.Click();
                    selectedBulding = building;
                    outlineManager.EnableOutline(building.outline);
                    if (building is CombatBuilding && (ui.IsEnabledMenuButtons() || enemyAttacks.IsEnemyAttack()))
                        rangeRenderer.DrawRange(20, building.GetComponent<CombatBuilding>());
                    else
                        rangeRenderer.Clear();
                    if (ui.EnabledBuildingsGrid() && buildingsGrid.buildingsMode == BuildingsMode.Movement && !building.unmovable && buildingsGrid.IsFlyingBuildingNull())
                    {
                        buildingsGrid.SetFlyingBuilding(building);
                        if (building is Fortification)
                        {
                            wallManager.DestroyWalls(building.GetComponent<Fortification>());
                            buildingsGrid.ClearWallGrid(building.transform.position.x, building.transform.position.z);
                        }
                        else
                            buildingsGrid.ClearGrid(building);
                        buildingsGrid.SaveBuildingPlace(building);
                    }
                }
                else if(hit.collider.GetComponent<Road>())
                {
                    Road road = hit.collider.GetComponent<Road>();
                    road.Click();
                    outlineManager.EnableOutline(road.outline);
                    if (ui.EnabledBuildingsGrid() && buildingsGrid.buildingsMode == BuildingsMode.Movement && !road.unmovable && buildingsGrid.IsFlyingRoadNull())
                    {
                        buildingsGrid.SetFlyingRoad(road);
                        buildingsGrid.ClearRoadGrid(road.transform.localPosition);
                        buildingsGrid.SaveRoadPlace(road.transform.localPosition);
                        buildingsGrid.UpdateRoadTilesArround(road.transform.localPosition);
                        buildingsGrid.SetDefaultSpriteForFlyingRoad();
                    }
                }
            }
            else if (groundPlane.Raycast(ray, out float position))
            {
                ui.DisableAllPanels();
                outlineManager.DisableOutline();
                rangeRenderer.Clear();
            }
        }
        else if(Input.GetMouseButton(0))
        {
            if (grabPerson != null)
            {
                curMousePos = Input.mousePosition;
            }
            if(Vector3.Distance(startMousePos, curMousePos) != 0f && groundPlane.Raycast(ray, out float position) && (ui.IsEnabledMenuButtons() || enemyAttacks.IsEnemyAttack()) && !grabPerson.inCombatBuilding)
            {
                grabPerson.nextPosition = ray.GetPoint(position);
            }
        }
        else if(Input.GetMouseButtonUp(0) && grabPerson != null && !grabPerson.inCombatBuilding)
        {
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, person) && Vector3.Distance(startMousePos, curMousePos) == 0f && (ui.IsEnabledMenuButtons() || enemyAttacks.IsEnemyAttack()))
            {
                grabPerson.Click();
                outlineManager.EnableOutline(grabPerson.GetComponent<Outline>());
            }
            else if(Physics.Raycast(ray, out hit, Mathf.Infinity, person) && Vector3.Distance(startMousePos, curMousePos) != 0f && (ui.IsEnabledMenuButtons() || enemyAttacks.IsEnemyAttack()))
            {
                if (grabPerson != null && Physics.Raycast(ray, out hit, Mathf.Infinity, building))
                {
                    if (hit.collider.GetComponent<Building>() is IWorkplace)
                    {
                        IWorkplace workplace = (IWorkplace)hit.collider.GetComponent<Building>();
                        if (workplace.HasWorkplace())
                        {
                            hit.collider.GetComponent<Building>().Click();
                            outlineManager.EnableOutline(hit.collider.GetComponent<Outline>());
                            if (hit.collider.GetComponent<CombatBuilding>())
                                rangeRenderer.DrawRange(20, hit.collider.GetComponent<CombatBuilding>());
                            selectedBulding = hit.collider.GetComponent<Building>();
                            workplace.AddWorker(grabPerson.gameObject);
                        }
                        else
                            ui.NoWorkplace();
                    }
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

    public void DestroySelectedBuilding()
    {
        selectedBulding.Destroy();
    }
}
