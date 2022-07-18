using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingsGrid : MonoBehaviour
{
    [HideInInspector] public static BuildingsGrid Instance;

    public Vector2Int gridSize;
    [SerializeField] WarPortal[] warPortals = new WarPortal[4];

    Building[,] buildingsGrid;
    Building[,] wallGrid;
    Building flyingBuilding;
    Camera mainCamera;
    RangeRenderer rangeRenderer;
    DestructionLines destructionLines;
    DestructionBox destructionBox;
    bool buildingPermit = true, destroyPermit = true;

    public BuildingsMode buildingsMode;

    List<Building> buildings = new List<Building>();

    private void Awake()
    {
        Instance = this;
        buildingsGrid = new Building[gridSize.x, gridSize.y];
        wallGrid = new Building[gridSize.x + 1, gridSize.y + 1];
        mainCamera = Camera.main;
        rangeRenderer = RangeRenderer.Instance;
        destructionLines = DestructionLines.Instance;
        destructionBox = DestructionBox.Instance;
    }

    //Universal
    Vector3 startDestructionPos, endDestructionPos;
    float timerToUpdatePath = 0;
    bool pathUpdated = true;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (buildingsMode == BuildingsMode.Normal)
                FinishPlacingBuilding();
            else if (buildingsMode == BuildingsMode.Destruction)
                EndDestruction();
            else
                ReturnFlyingBuilding();
        }

        if (Input.GetKeyDown(KeyCode.Q))
            flyingBuilding.RotateModel(-90);
        else if(Input.GetKeyDown(KeyCode.E))
            flyingBuilding.RotateModel(90);

        if (flyingBuilding != null && flyingBuilding.GetComponent<CombatBuilding>())
            rangeRenderer.DrawRange(20, flyingBuilding.GetComponent<CombatBuilding>());

        if (timerToUpdatePath > 0)
            timerToUpdatePath -= Time.deltaTime;
        if (timerToUpdatePath <= 0 && !pathUpdated)
            UpdatePaths();


        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (groundPlane.Raycast(ray, out float position))
        {
            if (flyingBuilding != null)
            {
                if (flyingBuilding is Fortification)
                {
                    Vector3 worldPosition = ray.GetPoint(position);

                    float x = Mathf.Floor(worldPosition.x) + 0.5f;
                    float y = Mathf.Floor(worldPosition.z) + 0.5f;

                    bool available = true;

                    if (x < -0.5f || x > gridSize.x - 0.5f) available = false;
                    if (y < -0.5f || y > gridSize.y - 0.5f) available = false;
                    if (available && IsWallPlaceTaken(x, y)) available = false;
                    if (available && !buildingPermit) available = false;

                    flyingBuilding.SetPosition(new Vector3(x, 0f, y));

                    if (buildingsMode == BuildingsMode.Normal)
                    {
                        if (available && !flyingBuilding.HasResources()) available = false;

                        flyingBuilding.SetTransparent(available);

                        if (available && Input.GetMouseButton(0))
                        {
                            Building buildig = flyingBuilding;
                            PlaceFlyingWall(x, y);
                            flyingBuilding = Instantiate(buildig);
                        }
                    }
                    else if (buildingsMode == BuildingsMode.Movement)
                    {
                        flyingBuilding.SetTransparent(available);

                        if (available && Input.GetMouseButtonDown(0))
                        {
                            if (!cannotBePlaced)
                                PlaceFlyingWall(x, y);
                            cannotBePlaced = false;
                        }
                    }
                }
                else
                {
                    Vector3 worldPosition = ray.GetPoint(position);

                    int x = Mathf.RoundToInt(worldPosition.x);
                    int y = Mathf.RoundToInt(worldPosition.z);

                    bool available = true;

                    if (x < 0 || x > gridSize.x - flyingBuilding.size.x) available = false;
                    if (y < 0 || y > gridSize.y - flyingBuilding.size.y) available = false;
                    if (available && IsPlaceTaken(x, y)) available = false;
                    if (available && !buildingPermit) available = false;

                    flyingBuilding.SetPosition(new Vector3(x, 0f, y));

                    if (buildingsMode == BuildingsMode.Normal)
                    {
                        if (available && !flyingBuilding.HasResources()) available = false;

                        flyingBuilding.SetTransparent(available);

                        if (available && Input.GetMouseButton(0))
                        {
                            Building building = flyingBuilding;
                            PlaceFlyingBuilding(x, y);
                            flyingBuilding = Instantiate(building);
                        }
                    }
                    else if (buildingsMode == BuildingsMode.Movement)
                    {
                        flyingBuilding.SetTransparent(available);

                        if (available && Input.GetMouseButtonDown(0))
                        {
                            if (!cannotBePlaced)
                                PlaceFlyingBuilding(x, y);
                            cannotBePlaced = false;
                        }
                    }
                }
            }
            else
            {
                if (buildingsMode == BuildingsMode.Destruction && destroyPermit)
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        startDestructionPos = ray.GetPoint(position);
                        foreach (Building building in buildings)
                            building.SetNormal();
                        destructionBox.ClearSelectedBuildings();
                        destructionBox.gameObject.SetActive(true);
                    }
                    if (Input.GetMouseButton(0))
                    {
                        endDestructionPos = ray.GetPoint(position);
                        destructionLines.DrawRectangle(startDestructionPos, endDestructionPos);
                        destructionBox.SetPosition(startDestructionPos, endDestructionPos);
                    }
                    if (Input.GetMouseButtonUp(0))
                    {
                        destructionLines.Clear();
                        destructionBox.gameObject.SetActive(false);
                    }
                }
            }
        }
    }

    public void DestroySelectedBuildings()
    {
        foreach (Building building in destructionBox.GetSelectedBuildings())
        {
            building.Destroy();
        }
        destructionBox.ClearSelectedBuildings();
    }

    public void EndDestruction()
    {
        destructionLines.Clear();
        foreach (Building building in buildings)
            building.SetNormal();
        destructionBox.ClearSelectedBuildings();
        destructionBox.gameObject.SetActive(false);
        SetBuildingsMode(BuildingsMode.Normal);
    }



    bool IsPlaceTaken(int placeX, int placeY)
    {
        for (int x = 0; x < flyingBuilding.size.x; x++)
        {
            for (int y = 0; y < flyingBuilding.size.y; y++)
            {
                if(buildingsGrid[placeX + x, placeY + y] != null) return true;
            }
        }
        for (int x = 0; x < flyingBuilding.size.x - 1; x++)
        {
            for (int y = 0; y < flyingBuilding.size.y - 1; y++)
            {
                if (wallGrid[placeX + x + 1, placeY + y + 1] != null) return true;
            }
        }

        return false;
    }

    bool IsWallPlaceTaken(float placeX, float placeY)
    {
        return wallGrid[Mathf.CeilToInt(placeX), Mathf.CeilToInt(placeY)] != null;
    }

    void PlaceFlyingBuilding(int placeX, int placeY)
    {
        PlaceBuilding(flyingBuilding, placeX, placeY);
        flyingBuilding.SetNormal();
        if(buildingsMode == BuildingsMode.Normal)
            flyingBuilding.Place();
        rangeRenderer.Clear();
        buildings.Add(flyingBuilding);
        StartUpdatePaths();
        flyingBuilding = null;
    }

    void PlaceFlyingWall(float placeX, float placeY)
    {
        PlaceWall(flyingBuilding, placeX, placeY);
        flyingBuilding.SetNormal();
        if (buildingsMode == BuildingsMode.Normal)
            flyingBuilding.Place();
        else if (buildingsMode == BuildingsMode.Movement)
            flyingBuilding.GetComponent<Fortification>().SetWalls();
        buildings.Add(flyingBuilding);
        StartUpdatePaths();
        flyingBuilding = null;
    }

    public void StartUpdatePaths()
    {
        timerToUpdatePath = 1;
        pathUpdated = false;
    }

    void UpdatePaths()
    {
        print("update paths");
        foreach (WarPortal warPortal in warPortals)
        {
            warPortal.UpdatePaths();
        }
        pathUpdated = true;
    }

    public void RemoveBuilding(Building building)
    {
        buildings.Remove(building);
        StartUpdatePaths();
    }

    public void StartPlacingBuilding(Building prefabBuilding)
    {
        FinishPlacingBuilding();

        flyingBuilding = Instantiate(prefabBuilding);
    }

    public void FinishPlacingBuilding()
    {
        if (flyingBuilding != null)
        {
            Destroy(flyingBuilding.gameObject);
            flyingBuilding = null;
            rangeRenderer.Clear();
        }
    }

    public void SetBuildingPermit(bool permit)
    {
        buildingPermit = permit;
    }

    public void SetDestroyPermit(bool permit)
    {
        destroyPermit = permit;
    }

    public void PlaceBuilding(Building building, int placeX, int placeY)
    {
        for (int x = 0; x < building.size.x; x++)
        {
            for (int y = 0; y < building.size.y; y++)
                buildingsGrid[placeX + x, placeY + y] = building;
        }
        for (int x = 0; x < building.size.x - 1; x++)
        {
            for (int y = 0; y < building.size.y - 1; y++)
                PlaceWall(building, placeX + 0.5f + x, placeY + 0.5f + y);
        }
    }

    public void PlaceWall(Building building, float placeX, float placeY)
    {
        wallGrid[Mathf.CeilToInt(placeX), Mathf.CeilToInt(placeY)] = building;
    }

    public void ClearGrid(Building building)
    {
        for (int x = 0; x < building.size.x; x++)
        {
            for (int y = 0; y < building.size.y; y++)
                buildingsGrid[(int)building.transform.position.x + x, (int)building.transform.position.z + y] = null;
        }
        for (int x = 0; x < building.size.x - 1; x++)
        {
            for (int y = 0; y < building.size.y - 1; y++)
                ClearWallGrid(building.transform.position.x + x + 0.5f, building.transform.position.z + y + 0.5f);
        }
    }

    public void ClearWallGrid(float placeX, float placeY)
    {
        wallGrid[Mathf.CeilToInt(placeX), Mathf.CeilToInt(placeY)] = null;
    }



    public void SetBuildingsMode(BuildingsMode buildingsMode)
    {
        this.buildingsMode = buildingsMode;
    }

    //for button
    public void SetNormalBuildingsMode()
    {
        buildingsMode = BuildingsMode.Normal;
    }
    public void SetDestructionBuildingsMode()
    {
        buildingsMode = BuildingsMode.Destruction;
    }
    public void SetMovementBuildingsMode()
    {
        buildingsMode = BuildingsMode.Movement;
    }

    //Movement

    bool cannotBePlaced = false;
    public void SetFlyingBuilding(Building building)
    {
        cannotBePlaced = true;
        flyingBuilding = building;
    }

    public bool IsFlyingBuildingNull()
    {
        return flyingBuilding == null;
    }

    [SerializeField] Vector2 buildingPlace = Vector2.zero;
    public void SaveBuildingPlace(Building building)
    {
        buildingPlace = new Vector2(building.transform.position.x, building.transform.position.z);
    }

    public void ReturnFlyingBuilding()
    {
        if (buildingsMode == BuildingsMode.Movement && flyingBuilding != null)
        {
            flyingBuilding.SetPosition(new Vector3(buildingPlace.x, 0f, buildingPlace.y));
            if (flyingBuilding is Fortification)
                PlaceFlyingWall(buildingPlace.x, buildingPlace.y);
            else
                PlaceFlyingBuilding((int)buildingPlace.x, (int)buildingPlace.y);
        }
    }



    public void OffBuildingsGrid()
    {
        if (buildingsMode == BuildingsMode.Movement)
            ReturnFlyingBuilding();
        else if (buildingsMode == BuildingsMode.Normal)
            FinishPlacingBuilding();
    }
}

public enum BuildingsMode
{ 
    Normal,
    Destruction,
    Movement
}
