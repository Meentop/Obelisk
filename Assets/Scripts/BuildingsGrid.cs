using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingsGrid : MonoBehaviour
{
    [HideInInspector] public static BuildingsGrid Instance;

    public Vector2Int gridSize;
    [SerializeField] WarPortal[] warPortals = new WarPortal[4];
    [SerializeField] RoadTile[] roadPrefabs;
    [SerializeField] Transform roadsParent;

    Building[,] buildingsGrid;
    Building[,] wallGrid;
    Road[,] roadGrid;
    Building flyingBuilding;
    Road flyingRoad;

    Camera mainCamera;
    RangeRenderer rangeRenderer;
    DestructionLines destructionLines;
    DestructionBox destructionBox;
    Resources resources;
    bool buildingPermit = true, destroyPermit = true;

    public BuildingsMode buildingsMode;

    List<Building> buildings = new List<Building>();
    List<Road> roads = new List<Road>();

    private void Awake()
    {
        Instance = this;
        buildingsGrid = new Building[gridSize.x, gridSize.y];
        wallGrid = new Building[gridSize.x + 1, gridSize.y + 1];
        roadGrid = new Road[gridSize.x / 2, gridSize.y / 2];
        mainCamera = Camera.main;
        rangeRenderer = RangeRenderer.Instance;
        destructionLines = DestructionLines.Instance;
        destructionBox = DestructionBox.Instance;
        resources = Resources.Instance;
    }
       
    private void Start()
    {
        foreach (Transform transform in roadsParent)
        {
            PlaceRoad(transform.GetComponent<Road>(), (int)transform.localPosition.x, (int)transform.localPosition.z);
            roads.Add(transform.GetComponent<Road>());
        }
    }

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

        if (flyingBuilding != null)
        {
            if (Input.GetKeyDown(KeyCode.Q))
                flyingBuilding.RotateModel(-90);
            else if (Input.GetKeyDown(KeyCode.E))
                flyingBuilding.RotateModel(90);
        }

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
            Vector3 worldPosition = ray.GetPoint(position);

            if (flyingRoad != null)
                RoadBuilding(worldPosition);
            else if (flyingBuilding != null)
            {
                if (flyingBuilding is Fortification)
                    WallBuilding(worldPosition);
                else
                    BuildBuilding(worldPosition);
            }
            else
            {
                if (buildingsMode == BuildingsMode.Destruction && destroyPermit)
                    DestructionMode(ray, position);
            }
        }
    }

    void RoadBuilding(Vector3 worldPosition)
    {
        bool available = true;
        int x = Mathf.RoundToInt(worldPosition.x) / 2;
        int y = Mathf.RoundToInt(worldPosition.z) / 2;

        if (x < 0 || x > gridSize.x / 2 - 1) available = false;
        if (y < 0 || y > gridSize.y / 2 - 1) available = false;
        if (available && IsRoadPlaceTaken(x, y)) available = false;
        if (available && !buildingPermit) available = false;

        flyingRoad.transform.localPosition = new Vector3(x, 0, y);

        if (buildingsMode == BuildingsMode.Normal)
        {
            if (available && !resources.HasResources(Resource.Wood, 5)) available = false;

            flyingRoad.GetComponent<Road>().SetTransparent(available);

            if (available && Input.GetMouseButton(0))
            {
                Road road = flyingRoad;
                PlaceFlyingRoad(x, y);
                flyingRoad = Instantiate(road, roadsParent);
                SetDefaultSpriteForFlyingRoad();
            }
        }
        else if (buildingsMode == BuildingsMode.Movement)
        {
            flyingRoad.GetComponent<Road>().SetTransparent(available);

            if (available && Input.GetMouseButtonDown(0))
            {
                if (!cannotBePlaced)
                    PlaceFlyingRoad(x, y);
                cannotBePlaced = false;
            }
        }
    }

    void WallBuilding(Vector3 worldPosition)
    {
        bool available = true;
        float x = Mathf.Floor(worldPosition.x) + 0.5f;
        float y = Mathf.Floor(worldPosition.z) + 0.5f;

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

    void BuildBuilding(Vector3 worldPosition)
    {
        bool available = true;
        int x = Mathf.RoundToInt(worldPosition.x);
        int y = Mathf.RoundToInt(worldPosition.z);

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

    void DestructionMode(Ray ray, float position)
    {
        if (Input.GetMouseButtonDown(0))
        {
            startDestructionPos = ray.GetPoint(position);
            foreach (Building building in buildings)
                building.SetNormal();
            foreach (Road road in roads)
                road.SetNormal();
            destructionBox.ClearSelectedBuildings();
            DefineRoadMode();
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

    public void DestroySelectedBuildings()
    {
        foreach (GameObject gameObject in destructionBox.GetSelectedBuildings())
        {
            if (gameObject.GetComponent<Building>())
                gameObject.GetComponent<Building>().Destroy();
            else if (gameObject.GetComponent<Road>())
                gameObject.GetComponent<Road>().Destroy();
        }
        destructionBox.ClearSelectedBuildings();
    }

    public void EndDestruction()
    {
        destructionLines.Clear();
        foreach (Building building in buildings)
            building.SetNormal();
        foreach (Road road in roads)
            road.SetNormal();
        destructionBox.ClearSelectedBuildings();
        destructionBox.gameObject.SetActive(false);
        SetBuildingsMode(BuildingsMode.Normal);
    }

    public void SetDefaultSpriteForFlyingRoad()
    {
        flyingRoad.GetComponent<Road>().SetDefaultMaterial();
    }

    void DefineRoadMode()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Building")))
        {
            if (hit.collider.GetComponent<Building>())
                destructionBox.SetRoadMode(false);
            else if(hit.collider.GetComponent<Road>())
                destructionBox.SetRoadMode(true);
        }
    }


    bool IsPlaceTaken(int placeX, int placeY)
    {
        for (int x = 0; x < flyingBuilding.size.x; x++)
        {
            for (int y = 0; y < flyingBuilding.size.y; y++)
            {
                if (buildingsGrid[placeX + x, placeY + y] != null) return true;
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

    bool IsRoadPlaceTaken(int placeX, int placeY)
    {
        return roadGrid[placeX, placeY] != null;
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

    void PlaceFlyingRoad(int placeX, int placeY)
    {
        Road road = flyingRoad.GetComponent<Road>();
        PlaceRoad(road, placeX, placeY);
        road.SetNormal();
        if (buildingsMode == BuildingsMode.Normal)
            road.Place();
        roads.Add(flyingRoad.GetComponent<Road>());
        StartUpdatePaths();
        flyingRoad = null;
        road.SetMaterial(GetRoadSprite(road.transform.localPosition, true));
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

    public void RemoveRoad(Road road)
    {
        roads.Remove(road);
        StartUpdatePaths();
    }

    public void StartPlacingBuilding(Building prefabBuilding)
    {
        FinishPlacingBuilding();

        flyingBuilding = Instantiate(prefabBuilding);
    }

    public void StartPlacingRoad(Road road)
    {
        FinishPlacingBuilding();

        flyingRoad = Instantiate(road, roadsParent);
    }

    public void FinishPlacingBuilding()
    {
        if (flyingBuilding != null)
        {
            Destroy(flyingBuilding.gameObject);
            flyingBuilding = null;
            rangeRenderer.Clear();
        }
        if (flyingRoad != null)
        {
            Destroy(flyingRoad.gameObject);
            flyingRoad = null;
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

    public void PlaceRoad(Road road, int placeX, int placeY)
    {
        roadGrid[placeX, placeY] = road;
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

    public void ClearRoadGrid(Vector3 pos)
    {
        roadGrid[(int)pos.x, (int)pos.z] = null;
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

    public void SetFlyingRoad(Road road)
    {
        cannotBePlaced = true;
        flyingRoad = road;
    }

    public bool IsFlyingBuildingNull()
    {
        return flyingBuilding == null;
    }

    public bool IsFlyingRoadNull()
    {
        return flyingRoad == null;
    }

    [SerializeField] Vector2 buildingPlace = Vector2.zero;
    public void SaveBuildingPlace(Building building)
    {
        buildingPlace = new Vector2(building.transform.position.x, building.transform.position.z);
    }

    public void SaveRoadPlace(Vector3 pos)
    {
        buildingPlace = new Vector2(pos.x, pos.z);
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
        else if (buildingsMode == BuildingsMode.Movement && flyingRoad != null)
        {
            flyingRoad.transform.localPosition = buildingPlace;
            PlaceFlyingRoad((int)buildingPlace.x, (int)buildingPlace.y);
        }
    }



    public void OffBuildingsGrid()
    {
        if (buildingsMode == BuildingsMode.Movement)
            ReturnFlyingBuilding();
        else if (buildingsMode == BuildingsMode.Normal)
            FinishPlacingBuilding();
    }


    //road
    Vector3[] directions = new Vector3[] { new Vector3(1, 0, 0), new Vector3(1, 0, -1), new Vector3(0, 0, -1), new Vector3(-1, 0, -1), new Vector3(-1, 0, 0), new Vector3(-1, 0, 1), new Vector3(0, 0, 1), new Vector3(1, 0, 1) };
    public Material GetRoadSprite(Vector3 pos, bool newRoad)
    {
        bool[] tilesArround = new bool[8];
        for (int i = 0; i < 8; i++)
        {
            Vector3 tile = pos + directions[i];
            if (tile.x < 0 || tile.x > gridSize.x / 2 - 1)
            {
                tilesArround[i] = false;
                continue;
            }
            if (tile.z < 0 || tile.z > gridSize.y / 2 - 1)
            {
                tilesArround[i] = false;
                continue;
            }
            if (roadGrid[(int)tile.x, (int)tile.z] == null)
                tilesArround[i] = false;
            else
            {
                tilesArround[i] = true;
                if (newRoad)
                    roadGrid[(int)tile.x, (int)tile.z].SetMaterial(GetRoadSprite(roadGrid[(int)tile.x, (int)tile.z].transform.localPosition, false));
            }
        }
        return GetRoadPrefabs(tilesArround);
    }

    Material GetRoadPrefabs(bool[] tilesArround)
    {
        foreach (RoadTile roadTile in roadPrefabs)
        {
            int similarities = 0;
            for (int i = 0; i < 8; i++)
            {
                if (roadTile.neighborStatuses[i] == NeighborStatus.NoMatter)
                    similarities++;
                else if (roadTile.neighborStatuses[i] == NeighborStatus.True && tilesArround[i])
                    similarities++;
                else if (roadTile.neighborStatuses[i] == NeighborStatus.Fasle && !tilesArround[i])
                    similarities++;
            }
            if (similarities == 8)
                return roadTile.material;
        }
        return null;
    }

    public void UpdateRoadTilesArround(Vector3 center)
    {
        for (int i = 0; i < 8; i++)
        {
            Vector3 tile = center + directions[i];
            if (tile.x < gridSize.x / 2 && tile.z < gridSize.y / 2 && roadGrid[(int)tile.x, (int)tile.z] != null)
                roadGrid[(int)tile.x, (int)tile.z].SetMaterial(GetRoadSprite(roadGrid[(int)tile.x, (int)tile.z].transform.localPosition, false));
        }
    }

    public bool HasRoad(float posX, float posY)
    {
        int x = Mathf.FloorToInt(posX / 2);
        int y = Mathf.FloorToInt(posY / 2);
        if (x < 25 && y < 25) 
            return roadGrid[x, y] != null;
        return false;
    }
}

public enum BuildingsMode
{ 
    Normal,
    Destruction,
    Movement
}

public enum NeighborStatus 
{ 
    True,
    Fasle,
    NoMatter
}

[System.Serializable]
class RoadTile
{
    [SerializeField] public Material material;
    [SerializeField] public NeighborStatus[] neighborStatuses = new NeighborStatus[8];
}