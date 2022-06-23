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
    bool blockedPause = false;

    [SerializeField] Transform arrow;

    UI ui;
    Obelisk obelisk;
    PersonsManager personsManager;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        ui = UI.Instance;
        obelisk = Obelisk.Instance;
        personsManager = PersonsManager.Instance;
        StartCoroutine(CycleCount());
        ui.UpdateCycleNumber(cycle);
    }

    int previousTimeScale = 1;

    private void Update()
    {
        SetArrowAngle();
        if (Input.GetKeyDown(KeyCode.Space) && timeScale != 0)
            SetPause();
        else if (Input.GetKeyDown(KeyCode.Space) && timeScale == 0 && !blockedPause)
            SetPreviousTimeScale();
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
                personsManager.TakeFood();
                if (cycle % obelisk.personSpawnTime == 0)
                    obelisk.AvailableNewPerson();
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
        this.timeScale = timeScale;
    }

    public void SetPause()
    {
        previousTimeScale = timeScale;
        SetTimeScale(0);
        ui.SetTimeSpeedButton(0);
    }

    public void SetPreviousTimeScale()
    {
        SetTimeScale(previousTimeScale);
        ui.SetTimeSpeedButton(previousTimeScale);
        blockedPause = false;
    }

    public void SetBlockedPause()
    {
        SetPause();
        blockedPause = true;
    }
}
