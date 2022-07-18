using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class WarPortal : Building
{
    EnemyAttacks enemyAttacks;
    Cycles cycles;
    [SerializeField] PortalType portalType;

    [SerializeField] Transform[] startPoints, finishPoints;

    List<EnemyType> enemyTypes = new List<EnemyType>();

    List<Vector2Int[]> paths1 = new List<Vector2Int[]>();
    List<Vector2Int[]> paths2 = new List<Vector2Int[]>();

    protected override void Start()
    {
        base.Start();
        enemyAttacks = EnemyAttacks.Instance;
        cycles = Cycles.Instance;
        buildingsGrid.PlaceBuilding(this, (int)transform.position.x, (int)transform.position.z);
    }

    public override void Click()
    {
        ui.EnableWarPortalPanel();
        enemyAttacks.SetUIWaves(portalType);
    }

    public void SetEnemyTypes(List<EnemyType> enemyTypes)
    {
        this.enemyTypes.AddRange(enemyTypes);
    }

    public void ClearEnemyTypes()
    {
        enemyTypes.Clear();
    }

    bool pathBlocked = false;
    public bool UpdatePaths()
    {
        paths1.Clear();
        paths2.Clear();
        Vector2Int start1 = new Vector2Int((int)startPoints[0].position.x, (int)startPoints[0].position.z);
        Vector2Int start2 = new Vector2Int((int)startPoints[1].position.x, (int)startPoints[1].position.z);
        Vector2Int finish1 = new Vector2Int((int)finishPoints[0].position.x, (int)finishPoints[0].position.z);
        Vector2Int finish2 = new Vector2Int((int)finishPoints[1].position.x, (int)finishPoints[1].position.z);
        foreach (EnemyType enemyType in enemyTypes)
        {
            Vector2Int[] path1 = GetPath(start1, finish1, enemyType);
            if(path1 == null)
            {
                PathBlocked();
                return false;
            }
            else
                paths1.Add(path1);

            Vector2Int[] path2 = GetPath(start2, finish2, enemyType);
            if (path2 == null)
            {
                PathBlocked();
                return false;
            }
            else
                paths2.Add(path2);
        }
        if (pathBlocked)
            PathClear();
        return true;
    }

    void PathBlocked()
    {
        ui.SetNoWayToEmpirePortal(true);
        cycles.SetBlockedPause();
        pathBlocked = true;
    }

    void PathClear()
    {
        ui.SetNoWayToEmpirePortal(false);
        cycles.UnblockPause();
        pathBlocked = false;
    }

    public Vector2Int[] GetPath(int value, EnemyType enemyType)
    {
        if (value == 0)
            return paths1[enemyTypes.FindIndex(type => type == enemyType)];
        else
            return paths2[enemyTypes.FindIndex(type => type == enemyType)];
    }

    Vector2Int[] GetPath(Vector2Int startPoint, Vector2Int finishPoint, EnemyType enemyType)
    {
        Tile start = new Tile();
        start.position = startPoint;
        Tile finish = new Tile();
        finish.position = finishPoint;

        start.SetDistance(finish.position);
        List<Tile> activeTiles = new List<Tile>();
        activeTiles.Add(start);
        List<Tile> visitedTiles = new List<Tile>();

        while(activeTiles.Any())
        {
            Tile checkTile = activeTiles.OrderBy(x => x.costDistance).First();
            if (checkTile.position == finish.position)
            {
                Tile tile = checkTile;
                List<Vector2Int> path = new List<Vector2Int>();
                while (true)
                {
                    path.Add(tile.position);
                    tile = tile.parent;
                    if (tile == null)
                    {
                        for (int i = 0; i < path.Count - 1; i++)
                        {
                            Vector3 startPos = new Vector3(path[i].x, 0, path[i].y);
                            Vector3 finishPos = new Vector3(path[i + 1].x, 0, path[i + 1].y);
                            Debug.DrawLine(startPos, finishPos, Color.red);
                        }
                        path.Reverse();
                        return path.ToArray();
                    }
                }
            }
            visitedTiles.Add(checkTile);
            activeTiles.Remove(checkTile);
            List<Tile> walkableTiles = GetWalkableTiles(checkTile, finish, enemyType);
            foreach (Tile walkableTile in walkableTiles)
            {
                if (visitedTiles.Any(tile => tile.position == walkableTile.position))
                    continue;
                if (activeTiles.Any(tile => tile.position == walkableTile.position))
                {
                    Tile existingTile = activeTiles.First(tile => tile.position == walkableTile.position);
                    if (existingTile.costDistance > checkTile.costDistance)
                    {
                        activeTiles.Remove(existingTile);
                        activeTiles.Add(walkableTile);
                    }
                }
                else
                    activeTiles.Add(walkableTile);
            }
        }
        return null;
    }

    Vector3[] directions = new Vector3[] { Vector3.forward, Vector3.back, Vector3.right, Vector3.left };
    List<Tile> GetWalkableTiles(Tile currentTile, Tile targetTile, EnemyType enemyType)
    {
        List<Tile> possibleTiles = new List<Tile>();
        for (int i = 0; i < 4; i++)
        {
            RaycastHit hit;
            if (Physics.Raycast(new Vector3(currentTile.position.x, 0, currentTile.position.y), directions[i], out hit, 0.5f, LayerMask.GetMask("Building")))
            {
                //Debug.DrawRay(new Vector3(currentTile.position.x, 0, currentTile.position.y), directions[i]);
                if (hit.collider.GetComponent<Barrier>() && (int)hit.collider.GetComponent<Barrier>().GetBarrierFor() != (int)enemyType)
                {
                    possibleTiles.Add(new Tile(new Vector2Int(currentTile.position.x + (int)directions[i].x, currentTile.position.y + (int)directions[i].z), currentTile.cost + 1, currentTile));
                    //Debug.DrawRay(new Vector3(currentTile.position.x, 0, currentTile.position.y), directions[i]);
                }
                else if(hit.collider.GetComponent<Transportation>() && hit.collider.GetComponent<Transportation>().HasOutgoingPoints())
                {
                    possibleTiles.Add(new Tile(hit.collider.GetComponent<Transportation>().GetOutgoingPoint(), currentTile.cost + 1, currentTile));
                    
                }
                else if (hit.collider.GetComponent<IncomingPortal>() || hit.collider.GetComponent<OutgoingPortal>() || hit.collider.GetComponent<EmpirePortal>())
                {
                    possibleTiles.Add(new Tile(new Vector2Int(currentTile.position.x + (int)directions[i].x, currentTile.position.y + (int)directions[i].z), currentTile.cost + 1, currentTile));
                    //Debug.DrawRay(new Vector3(currentTile.position.x, 0, currentTile.position.y), directions[i]);
                }
            }
            else
            {
                possibleTiles.Add(new Tile(new Vector2Int(currentTile.position.x + (int)directions[i].x, currentTile.position.y + (int)directions[i].z), currentTile.cost + 1, currentTile));
                //Debug.DrawRay(new Vector3(currentTile.position.x, 0, currentTile.position.y), directions[i]);
            }
        }
        possibleTiles.ForEach(tile => tile.SetDistance(targetTile.position));
        return possibleTiles
            .Where(tile => tile.position.x >= 0 && tile.position.x <= buildingsGrid.gridSize.x - 1)
            .Where(tile => tile.position.y >= 0 && tile.position.y <= buildingsGrid.gridSize.y - 1)
            .ToList();
    }

    public Vector3 GetSpawnPosition(int value)
    {
        return startPoints[value].position;
    }
}

class Tile
{
    public Vector2Int position;
    public int cost;
    public int distance;
    public int costDistance => cost + distance;
    public Tile parent;

    public Tile(Vector2Int position, int cost, Tile parent)
    {
        this.position = position;
        this.cost = cost;
        this.parent = parent;
    }
    public Tile()
    {

    }
    public void SetDistance(Vector2Int targetPos)
    {
        distance = Mathf.Abs(targetPos.x - position.x) + Mathf.Abs(targetPos.y - position.y);
    }
}
