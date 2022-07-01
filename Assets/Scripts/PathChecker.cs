using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PathChecker : MonoBehaviour
{
    public static PathChecker Instance;

    [SerializeField] Transform[] warPortals = new Transform[4];
    EmpirePortal empirePortal;

    NavMeshPath path;
    UI ui;
    Cycles cycles;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        empirePortal = EmpirePortal.Instance;
        path = new NavMeshPath();
        ui = UI.Instance;
        cycles = Cycles.Instance;
        StartCoroutine(Timer());
    }

    IEnumerator Timer()
    {
        while(true)
        {
            yield return new WaitForSeconds(1);
            CheckPath();
        }
    }

    bool pathBlocked = false;
    public void CheckPath()
    {
        foreach (Transform warPortal in warPortals)
        {
            NavMesh.CalculatePath(warPortal.position, empirePortal.transform.position, NavMesh.AllAreas, path);
            if (path.status == NavMeshPathStatus.PathPartial)
            {
                ui.SetNoWayToEmpirePortal(true);
                cycles.SetBlockedPause();
                pathBlocked = true;
                return;
            }
        }
        if (pathBlocked)
        {
            ui.SetNoWayToEmpirePortal(false);
            cycles.SetPreviousTimeScale();
            pathBlocked = false;
        }
    }
}
