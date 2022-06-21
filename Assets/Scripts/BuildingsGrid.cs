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
    bool buildingPermit = true;

    private void Awake()
    {
        Instance = this;
        grid = new Building[gridSize.x, gridSize.y];
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            FinishPlacingBuilding();
        }
        if (Input.GetKeyDown(KeyCode.Q))
            flyingBuilding.RotateModel(-90);
        else if(Input.GetKeyDown(KeyCode.E))
            flyingBuilding.RotateModel(90);

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
                if (available && !buildingPermit) available = false;
                if (available && IsPlaceTaken(x, y)) available = false;
                if (available && !flyingBuilding.HasResources()) available = false;

                flyingBuilding.transform.position = new Vector3(x, 0f, y);
                flyingBuilding.SetTransparent(available);

                if (available && Input.GetMouseButtonDown(0))
                {
                    Building lastBuilding = flyingBuilding;
                    PlaceFlyingBuilding(x, y);
                    flyingBuilding = Instantiate(lastBuilding);
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
        for (int x = 0; x < flyingBuilding.size.x; x++)
        {
            for (int y = 0; y < flyingBuilding.size.y; y++)
            {
                grid[placeX + x, placeY + y] = flyingBuilding;
            }
        }

        flyingBuilding.SetNormal();
        flyingBuilding.Place();
        flyingBuilding = null;
    }

    public void StartPlacingBuilding(Building prefabBuilding)
    {
        if (flyingBuilding != null)
            Destroy(flyingBuilding.gameObject);

        flyingBuilding = Instantiate(prefabBuilding);
    }

    public void FinishPlacingBuilding()
    {
        if(flyingBuilding != null)
            Destroy(flyingBuilding.gameObject);
    }

    public void SetBuildingPermit(bool permit)
    {
        buildingPermit = permit;
    }

    public void PlaceObelisk(Building obelisk, int placeX, int placeY)
    {
        for (int x = 0; x < obelisk.size.x; x++)
        {
            for (int y = 0; y < obelisk.size.y; y++)
            {
                grid[placeX + x, placeY + y] = obelisk;
            }
        }
    }
}
