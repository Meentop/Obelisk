using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Person : MonoBehaviour
{
    UI ui;
    MouseRay mouseRay;
    PersonsManager personsManager;

    public string fullName;

    public Transform hairSpawnPoint, beardSpawnPoint, bodySpawnPoint;

    public float efficiencyModifier = 0;

    public bool isHungry = false, active = false, inCombatBuilding = false, isCombat;

    public Vector3 nextPosition;

    public Building workplace;

    private void Start()
    {
        ui = UI.Instance;
        mouseRay = MouseRay.Instance;
        personsManager = PersonsManager.Instance;
        nextPosition = transform.position;
        if(active)
            personsManager.allPersons.Add(this);
    }

    private void Update()
    {
        transform.position = Vector3.Lerp(transform.position, nextPosition, 0.3f);
        if (isHungry)
            efficiencyModifier = -25;
        else
            efficiencyModifier = 0;
    }

    public void SetLayer(int layer)
    {
        gameObject.layer = layer;
        transform.GetChild(0).gameObject.layer = layer;
        SetLayerForObjectAndChild(layer, hairSpawnPoint.gameObject);
        SetLayerForObjectAndChild(layer, beardSpawnPoint.gameObject);
        SetLayerForObjectAndChild(layer, bodySpawnPoint.gameObject);
    }
    void SetLayerForObjectAndChild(int layer, GameObject gameObject)
    {
        gameObject.layer = layer;
        gameObject.transform.GetChild(0).gameObject.layer = layer;
    }

    public void Click()
    {
        ui.EnablePersonPanel(fullName, isHungry, isCombat);
    }

    public void SpawnAfterBuilding()
    {
        nextPosition = mouseRay.GetMousePositionOnPlane();
        transform.position = mouseRay.GetMousePositionOnPlane();
        mouseRay.grabPerson = this;
        mouseRay.startMousePos = Input.mousePosition;
    }



    public void Destroy()
    {
        if (workplace != null)
        {
            IWorkplace iWorkplace = (IWorkplace)workplace;
            iWorkplace.RemoveWorker(iWorkplace.FindIndex(this));
        }
        personsManager.allPersons.Remove(this);
        Destroy(gameObject);
    }
}
