using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingsGrid : MonoBehaviour
{
    [HideInInspector] public static BuildingsGrid Instance;

    [SerializeField] Vector2Int gridSize;

    Building[,] grid;
    Building flyingBuilding;
    Camera mainCamera;
    RangeRenderer rangeRenderer;
    bool buildingPermit = true;

    public BuildingsMode buildingsMode;

    private void Awake()
    {
        Instance = this;
        grid = new Building[gridSize.x, gridSize.y];
        mainCamera = Camera.main;
        rangeRenderer = RangeRenderer.Instance;
    }

    //Universal

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (buildingsMode == BuildingsMode.Normal)
                FinishPlacingBuilding();
            ReturnFlyingBuilding();
        }

        if (Input.GetKeyDown(KeyCode.Q))
            flyingBuilding.RotateModel(-90);
        else if(Input.GetKeyDown(KeyCode.E))
            flyingBuilding.RotateModel(90);

        if (flyingBuilding != null && flyingBuilding.GetComponent<CombatBuilding>())
            rangeRenderer.DrawRange(20, flyingBuilding.GetComponent<CombatBuilding>());

        if (flyingBuilding != null)
        {
            Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            if(groundPlane.Raycast(ray, out float position))
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

                    if (available && Input.GetMouseButtonDown(0))
                    {
                        Building lastBuilding = flyingBuilding;
                        PlaceFlyingBuilding(x, y);
                        flyingBuilding = Instantiate(lastBuilding);
                    }
                }
                else if(buildingsMode == BuildingsMode.Movement)
                {
                    flyingBuilding.SetTransparent(available);

                    if (available && Input.GetMouseButtonDown(0))
                    {
                        if(!cannotBePlaced)
                            PlaceFlyingBuilding(x, y);
                        cannotBePlaced = false;
                    }
                }
            }
        }
    }

    bool IsPlaceTaken(int placeX, int placeY)
    {
        for (int x = 0; x < flyingBuilding.size.x; x++)
        {
            for (int y = 0; y < flyingBuilding.size.y; y++)
            {
                if(grid[placeX + x, placeY + y] != null) return true;
            }
        }

        return false;
    }

    void PlaceFlyingBuilding(int placeX, int placeY)
    {
        PlaceBuilding(flyingBuilding, placeX, placeY);
        flyingBuilding.SetNormal();
        if(buildingsMode == BuildingsMode.Normal)
            flyingBuilding.Place();
        rangeRenderer.Clear();
        flyingBuilding = null;
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

    public void PlaceBuilding(Building building, int placeX, int placeY)
    {
        for (int x = 0; x < building.size.x; x++)
        {
            for (int y = 0; y < building.size.y; y++)
                grid[placeX + x, placeY + y] = building;
        }
    }

    public void ClearGrid(Building building)
    {
        for (int x = 0; x < building.size.x; x++)
        {
            for (int y = 0; y < building.size.y; y++)
                grid[(int)building.transform.position.x + x, (int)building.transform.position.z + y] = null;
        }
    }

    //Destruction

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

    [SerializeField] Vector2Int buildingPlace = Vector2Int.zero;
    public void SaveBuildingPlace(Building building)
    {
        buildingPlace = new Vector2Int((int)building.transform.position.x, (int)building.transform.position.z);
    }

    public void ReturnFlyingBuilding()
    {
        if (buildingsMode == BuildingsMode.Movement && flyingBuilding != null)
        {
            flyingBuilding.SetPosition(new Vector3(buildingPlace.x, 0f, buildingPlace.y));
            PlaceFlyingBuilding(buildingPlace.x, buildingPlace.y);
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
