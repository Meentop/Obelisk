using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttacks : MonoBehaviour
{
    public static EnemyAttacks Instance;
    Cycles cycles;
    UI ui;
    BuildingsGrid buildingsGrid;
    EmpirePortal empirePortal;
    Resources resources;

    [SerializeField] WarPortal[] warPortals = new WarPortal[4];

    [SerializeField] List<Enemy> enemies = new List<Enemy>();

    [SerializeField] List<EnemyAttack> enemyAttacks = new List<EnemyAttack>();

    bool attackNow = false;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        cycles = Cycles.Instance;
        ui = UI.Instance;
        buildingsGrid = BuildingsGrid.Instance;
        empirePortal = EmpirePortal.Instance;
        resources = Resources.Instance;
        StartCoroutine(PrepareToNewAttack());
    }

    private void Update()
    {
        if (!IsEnemyAttack())
        {
            float timeToAttack = enemyAttacks[0].attackCycle - (cycles.cycle + (cycles.curCycleTime / cycles.cycleTime));
            ui.SetTimeToAttack((int)enemyAttacks[0].portalType, timeToAttack);
            if (timeToAttack < 0.25f)
                ui.SetEnemyNearText(true);
        }
    }

    public void CheckEnemyAttack()
    {
        if(cycles.cycle == enemyAttacks[0].attackCycle)
            StartAttack();
    }

    void StartAttack()
    {
        attackNow = true;
        ui.DisableWarPortalsUI();
        ui.SetEnemyNearText(false);
        ui.SetEnableEmpirePortalHP(true);
        ui.SetEmpirePortalHP(empirePortal.curHp);
        cycles.SetTimeScale(1);
        ui.SetTimeSpeedButton(1);
        ui.DisableAllMenu();
        ui.DisableMenuButtons();
        buildingsGrid.OffBuildingsGrid();
        ui.DisableCells();
        CombatBuilding[] combats = FindObjectsOfType<CombatBuilding>();
        foreach (CombatBuilding combat in combats)
            combat.SetCurAttackSpeed();
        StartCoroutine(SpawnEnemy());
    }

    List<Enemy> enemyInWave = new List<Enemy>();
    IEnumerator SpawnEnemy()
    {
        foreach (Wave wave in enemyAttacks[0].waves)
        {
            enemyInWave.Clear();
            foreach (EnemyGroup enemyGroup in wave.enemyGroups)
            {
                for (int i = 0; i < enemyGroup.enemyCount; i++)
                {
                    if(cycles.timeScale == 0)
                        yield return new WaitWhile(() => cycles.timeScale == 0);
                    float j = 0;
                    while (j < enemyGroup.spawnInterval)
                    {
                        yield return new WaitForSecondsRealtime(Time.fixedDeltaTime);
                        j += Time.fixedDeltaTime * cycles.timeScale;
                    }
                    int rand = UnityEngine.Random.Range(0, 2);
                    Enemy enemy = Instantiate(enemies[(int)enemyGroup.enemyType], warPortals[(int)enemyAttacks[0].portalType].GetSpawnPosition(rand), Quaternion.identity);
                    enemyInWave.Add(enemy);
                    enemy.number = enemyInWave.Count - 1;
                    enemy.SetPath(warPortals[(int)enemyAttacks[0].portalType].GetPath(rand, enemyGroup.enemyType));
                }
            }
            yield return new WaitUntil(() => enemyInWave.Count == 0);
        }
        yield return new WaitForSecondsRealtime(5);
        EndAttack();
    }

    void EndAttack()
    {
        attackNow = false;
        ui.SetEnableEmpirePortalHP(false);
        if (empirePortal.curHp == empirePortal.maxHp)
            resources.AddResource(Resource.MatterGenerator, 1);
        ui.AttackRepulsed();
        empirePortal.curHp = empirePortal.maxHp;
        ui.EnableMenuButtons();
        enemyAttacks.RemoveAt(0);
        StartCoroutine(PrepareToNewAttack());
    }

    IEnumerator PrepareToNewAttack()
    {
        yield return new WaitForEndOfFrame();
        SetEnemiesIcons();
        foreach (WarPortal warPortal in warPortals)
        {
            warPortal.ClearEnemyTypes();
        }
        warPortals[(int)enemyAttacks[0].portalType].SetEnemyTypes(GetUniqueEnemyTypes());
        warPortals[(int)enemyAttacks[0].portalType].UpdatePaths();
    }

    List<EnemyType> GetUniqueEnemyTypes()
    {
        List<EnemyType> enemies = new List<EnemyType>();
        enemies.Add(enemyAttacks[0].waves[0].enemyGroups[0].enemyType);
        foreach (Wave wave in enemyAttacks[0].waves)
        {
            foreach (EnemyGroup enemyGroup in wave.enemyGroups)
            {
                int j = 0;
                for (int i = 0; i < enemies.Count; i++)
                {
                    if (enemies[i] != enemyGroup.enemyType)
                        j++;
                }
                if (j == enemies.Count)
                    enemies.Add(enemyGroup.enemyType);
            }
        }
        return enemies;
    }

    void SetEnemiesIcons()
    {
        ui.SetEnemiesIcons((int)enemyAttacks[0].portalType, GetUniqueEnemyTypes().ToArray());
    }

    public void SetUIWaves(PortalType portalType)
    {
        if(portalType == enemyAttacks[0].portalType)
        {
            for (int i = 0; i < enemyAttacks[0].waves.Count; i++)
            {
                ui.CreateWaveBlocks(i + 1);
                foreach (EnemyGroup enemyGroup in enemyAttacks[0].waves[i].enemyGroups)
                {
                    ui.InitNewEnemyGroupBlock(i, enemyGroup.enemyType, enemyGroup.spawnInterval > 0.5f, (int)enemies[(int)enemyGroup.enemyType].GetCurHP(enemyGroup.lvl), enemyGroup.enemyCount);
                }
            }
        }
    }

    public void RemoveEnemy(Enemy enemy)
    {
        enemyInWave.Remove(enemy);
    }

    public int GetIndex(Enemy enemy)
    {
        for (int i = 0; i < enemyInWave.Count; i++)
        {
            if (enemyInWave[i] == enemy)
                return i;
        }
        return -1;
    }

    public bool IsEnemyAttack()
    {
        return attackNow;
    }
}

public enum PortalType
{
    TopRight,
    BottomRight,
    BottomLeft,
    TopLeft
}

public enum EnemyType
{
    Ork,
    Troll,
    Gremlin
}

[Serializable]
public class EnemyAttack
{
    public int attackCycle;

    public PortalType portalType;

    public List<Wave> waves = new List<Wave>();
}

[Serializable]
public class Wave
{
    public List<EnemyGroup> enemyGroups = new List<EnemyGroup>();
}

[Serializable]
public class EnemyGroup
{
    public EnemyType enemyType;

    public float spawnInterval;

    public int lvl;

    public int enemyCount;
}