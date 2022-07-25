using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cycles : MonoBehaviour
{
    [HideInInspector] public static Cycles Instance;

    public int cycle { get; private set; } = 0;
    public int cycleTime = 240;
    public int timeScale { get; private set; } = 99;

    public float curCycleTime/* { get; private set; }*/ = 0;
    public bool blockedPause { get; private set; } = false;

    [SerializeField] Transform arrow;

    [Header("Day/Night")]
    [SerializeField] Light mySun;
    [SerializeField] Light myMoon;
    [SerializeField] AnimationCurve sunCurve;
    [SerializeField] AnimationCurve moonCurve;
    float sunIntensity;
    float moonIntensity;

    UI ui;
    EmpirePortal empirePortal;
    PersonsManager personsManager;
    EnemyAttacks enemyAttacks;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        ui = UI.Instance;
        empirePortal = EmpirePortal.Instance;
        personsManager = PersonsManager.Instance;
        enemyAttacks = EnemyAttacks.Instance;
        StartCoroutine(CycleCount());
        ui.UpdateCycleNumber(cycle);

        sunIntensity = mySun.intensity;
        moonIntensity = myMoon.intensity;
    }

    int previousTimeScale = 1;

    private void Update()
    {
        SetArrowAngle();
        if (Input.GetKeyDown(KeyCode.Space) && timeScale != 0)
            SetPause();
        else if (Input.GetKeyDown(KeyCode.Space) && timeScale == 0 && !blockedPause)
            SetPreviousTimeScale();
        else if (Input.GetKeyDown(KeyCode.Alpha1))
            SetTimeScale(1);
        else if (Input.GetKeyDown(KeyCode.Alpha2))
            SetTimeScale(2);
        else if (Input.GetKeyDown(KeyCode.Alpha3))
            SetTimeScale(3);

        SetLightsRotation();
        SetLightsIntensity();
    }

    IEnumerator CycleCount()
    {
        while(true)
        {
            yield return new WaitForSecondsRealtime(0.1f);
            curCycleTime += 0.1f * timeScale;
            if(curCycleTime >= cycleTime)
            {
                cycle++;
                enemyAttacks.CheckEnemyAttack();
                personsManager.TakeFood();
                if (cycle % empirePortal.personSpawnTime == 0)
                    empirePortal.AvailableNewPerson();
                ui.UpdateCycleNumber(cycle);
                curCycleTime = 0;
            }
        }
    }

    void SetArrowAngle()
    {
        arrow.localRotation = Quaternion.Euler(new Vector3(0, 0, curCycleTime / cycleTime * -360));
    }

    public void SetTimeScale(int timeScale)
    {
        if (!blockedPause && timeScale >= 0 && timeScale <= 3)
        {
            this.timeScale = timeScale;
            ui.SetTimeSpeedButton(timeScale);
        }
    }

    public void SetPause()
    {
        if (timeScale != 0)
        {
            previousTimeScale = timeScale;
            SetTimeScale(0);
        }
    }

    public void SetPreviousTimeScale()
    {
        blockedPause = false;
        SetTimeScale(previousTimeScale);
    }

    public void SetBlockedPause()
    {
        SetPause();
        blockedPause = true;
    }

    public void UnblockPause()
    {
        blockedPause = false;
    }

    //Day or Night

    void SetLightsRotation()
    {
        Quaternion nextSunRotation = Quaternion.Euler(curCycleTime / cycleTime * 360, -35, 0);
        mySun.transform.localRotation = Quaternion.Lerp(mySun.transform.localRotation, nextSunRotation, 0.125f);
        Quaternion nextMoonRotation = Quaternion.Euler(curCycleTime / cycleTime * 360 + 180f, -35, 0);
        myMoon.transform.localRotation = Quaternion.Lerp(myMoon.transform.localRotation, nextMoonRotation, 0.125f);
    }

    void SetLightsIntensity()
    {
        mySun.intensity = sunIntensity * sunCurve.Evaluate(curCycleTime / cycleTime);
        myMoon.intensity = moonIntensity * moonCurve.Evaluate(curCycleTime / cycleTime);
    }
}
