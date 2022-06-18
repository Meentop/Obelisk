using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obelisk : Building
{
    BuildingsGrid buildingsGrid;
    PersonCreator personCreator;
    Cycles cycles;
    CameraMove cameraMove;

    public int personSpawnTime;

    [SerializeField] GameObject globalText, myCanvasIcon;

    [SerializeField] Transform personSpawnPoint;

    protected override void Start()
    {
        buildingsGrid = FindObjectOfType<BuildingsGrid>();
        personCreator = FindObjectOfType<PersonCreator>();
        cycles = FindObjectOfType<Cycles>();
        cameraMove = FindObjectOfType<CameraMove>();
        buildingsGrid.PlaceObelisk(this, (int)transform.position.x, (int)transform.position.z);
    }

    private void Update()
    {
        if(ui.IsObeliskPanelEnabled())
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
        if (availableNewPerson && !creatingNewPerson)
        {
            personCreator.StartCreate();
            cycles.SetPause();
            cameraMove.BlockedZoom(10f);
            globalText.SetActive(false);
            myCanvasIcon.SetActive(false);
            availableNewPerson = false;
            creatingNewPerson = true;
        }
        else if(!availableNewPerson && !creatingNewPerson)
            ui.EnableObeliskPanel();
    }

    public void EndCreating(GameObject person)
    {
        creatingNewPerson = false;
        cameraMove.UnblockedZoom();
        person.GetComponent<Person>().active = true;
        Instantiate(person, personSpawnPoint.position, Quaternion.identity);
        cycles.SetPreviousTimeScale();
    }
}
