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

    [SerializeField] Transform[] enemySpawnPoints = new Transform[4];

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
        {
            attackNow = true;
            ui.DisableWarPortalsUI();
            ui.SetEnemyNearText(false);
            ui.SetEnableEmpirePortalHP(true);
            ui.SetEmpirePortalHP(empirePortal.curHp);
            cycles.SetTimeScale(1);
            ui.SetTimeSpeedButton(1);
            ui.DisnableStartUI();
            buildingsGrid.OffBuildingsGrid();
            ui.DisableCells();
            StartCoroutine(SpawnEnemy());
        }
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
                    while (j < 0.6f)
                    {
                        yield return new WaitForSecondsRealtime(Time.fixedDeltaTime);
                        j += Time.fixedDeltaTime * cycles.timeScale;
                    }
                    enemyInWave.Add(Instantiate(enemies[(int)enemyGroup.enemyType], enemySpawnPoints[(int)enemyAttacks[0].portalType].position, Quaternion.identity));
                }
            }
            yield return new WaitUntil(() => enemyInWave.Count == 0);
        }
        yield return new WaitForSecondsRealtime(5);
        attackNow = false;
        ui.SetEnableEmpirePortalHP(false);
        if (empirePortal.curHp == empirePortal.maxHp)
            resources.AddResource(Resource.MatterGenerator, 1);
        ui.AttackRepulsed();
        empirePortal.curHp = empirePortal.maxHp;
        ui.EnableStartUI();
        enemyAttacks.RemoveAt(0);
    }

    public void RemoveEnemy(Enemy enemy)
    {
        enemyInWave.Remove(enemy);
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

    public int enemyCount;
}