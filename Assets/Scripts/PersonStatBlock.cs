using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PersonStatBlock : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    public int number;

    public IndustrialBuilding building;

    public Transform pointer;

    public Text workerName, efficiency;

    UI ui;

    private void Start()
    {
        ui = UI.Instance;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Person person = building.RemoveWorker(number);
        person.SpawnAfterBuilding();
        ui.SetCursorOnButton(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        workerName.gameObject.SetActive(true);
        efficiency.gameObject.SetActive(true);
        ui.SetCursorOnButton(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        workerName.gameObject.SetActive(false);
        efficiency.gameObject.SetActive(false);
        ui.SetCursorOnButton(false);
    }
}
