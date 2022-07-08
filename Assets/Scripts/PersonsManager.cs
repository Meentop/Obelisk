using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonsManager : MonoBehaviour
{
    public static PersonsManager Instance;

    List<Person> allPersons = new List<Person>();

    Resources resources;
    UI ui;
    Cycles cycles;
    CameraMove cameraMove;
    BuildingsGrid buildingsGrid;

    private void Awake()
    {
        Instance = this;
        ui = UI.Instance;
    }

    private void Start()
    {
        resources = Resources.Instance;
        cycles = Cycles.Instance;
        cameraMove = CameraMove.Instance;
        buildingsGrid = BuildingsGrid.Instance;
        ui.UpdatePersonCount(allPersons.Count);
    }

    public void AddPerson(Person person)
    {
        allPersons.Add(person);
        ui.UpdatePersonCount(allPersons.Count);
    }

    public void RemovePerson(Person person)
    {
        allPersons.Remove(person);
        ui.UpdatePersonCount(allPersons.Count);
    }

    public void TakeFood()
    {
        if (resources.HasResources(Resource.Food, allPersons.Count))
        {
            resources.TakeResource(Resource.Food, allPersons.Count);
            foreach (Person person in allPersons)
            {
                person.isHungry = false;
            }
        }
        else
        {
            cycles.SetBlockedPause();
            cameraMove.BlockedZoom(10f);
            buildingsGrid.OffBuildingsGrid();
            ui.EnableFeedPersonsMenu(allPersons, resources.GetResource(Resource.Food));
        }
    }

    public void FinishFeed()
    {
        if (ui.FinishFeed())
        {
            resources.TakeResource(Resource.Food, resources.GetResource(Resource.Food));
            cycles.SetPreviousTimeScale();
            cameraMove.UnblockedZoom();
        }
    }

    public void EnablePersonsList()
    {
        ui.EnablePersonsList(allPersons);
    }
}
