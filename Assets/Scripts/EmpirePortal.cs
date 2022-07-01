using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmpirePortal : Building
{
    [HideInInspector] public static EmpirePortal Instance;

    PersonCreator personCreator;
    Cycles cycles;
    CameraMove cameraMove;

    public int personSpawnTime;

    [SerializeField] GameObject globalText, myCanvasIcon;

    [SerializeField] Transform personSpawnPoint;

    [SerializeField] LayerMask enemy;

    public int maxHp;
    public int curHp;

    private void Awake()
    {
        Instance = this;
    }

    protected override void Start()
    {
        base.Start();
        personCreator = PersonCreator.Instance;
        cycles = Cycles.Instance;
        cameraMove = CameraMove.Instance;
        buildingsGrid.PlaceBuilding(this, (int)transform.position.x, (int)transform.position.z);

        curHp = maxHp;
    }

    private void Update()
    {
        if(ui.IsEmpirePortalPanelEnabled())
        {
            for (int i = 1; i <= personSpawnTime + 1; i++)
            {
                if ((cycles.cycle + i) % personSpawnTime == 0)
                {
                    float time = (cycles.cycleTime - cycles.curCycleTime) / cycles.cycleTime;
                    ui.SetCyclesToNewPerson(time + i - 1);
                    break;
                }
            }
        }

        Collider[] hitColliders = Physics.OverlapBox(gameObject.transform.position, transform.localScale / 2, Quaternion.identity, enemy);

        if (hitColliders.Length > 0)
        {
            foreach (Collider collider in hitColliders)
            {
                Destroy(collider.gameObject);
                curHp--;
                ui.SetEmpirePortalHP(curHp);
                if (curHp <= 0)
                    print("end game");
            }
        }
    }

    bool availableNewPerson = false, creatingNewPerson = false;
    public void AvailableNewPerson()
    {
        globalText.SetActive(true);
        myCanvasIcon.SetActive(true);
        availableNewPerson = true;
    }

    public override void Click()
    {
        if (availableNewPerson && !creatingNewPerson && ui.EnabledStartUI())
        {
            personCreator.StartCreate();
            cycles.SetBlockedPause();
            cameraMove.BlockedZoom(10f);
            globalText.SetActive(false);
            myCanvasIcon.SetActive(false);
            availableNewPerson = false;
            creatingNewPerson = true;
        }
        else if(!availableNewPerson && !creatingNewPerson)
            ui.EnableEmpirePortalPanel();
    }

    public void EndCreating(GameObject person)
    {
        creatingNewPerson = false;
        cameraMove.UnblockedZoom();
        person.GetComponent<Person>().active = true;
        int layer = LayerMask.NameToLayer("Person");
        person.GetComponent<Person>().SetLayer(layer);
        Instantiate(person, personSpawnPoint.position, Quaternion.identity);
        cycles.SetPreviousTimeScale();
    }
}
