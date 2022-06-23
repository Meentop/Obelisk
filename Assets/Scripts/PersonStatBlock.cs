using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PersonStatBlock : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    protected int number;

    IndustrialBuilding building;

    [SerializeField] protected Transform pointer;

    [SerializeField] protected GameObject hungry;

    [SerializeField] protected Text fullName, efficiency;

    protected UI ui;

    private void Start()
    {
        ui = UI.Instance;
    }

    public virtual void Initialization(int number, IndustrialBuilding building, Vector3 pointerPos, bool hungry, string name, string efficiency)
    {
        this.number = number;
        this.building = building;
        pointer.localPosition = pointerPos;
        this.hungry.SetActive(hungry);
        fullName.text = name;
        this.efficiency.text = efficiency;
    }

    public virtual void OnPointerDown(PointerEventData eventData)
    {
        Person person = building.RemoveWorker(number);
        person.SpawnAfterBuilding();
        ui.SetCursorOnButton(false);
    }

    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        fullName.gameObject.SetActive(true);
        efficiency.gameObject.SetActive(true);
        ui.SetCursorOnButton(true);
    }

    public virtual void OnPointerExit(PointerEventData eventData)
    {
        fullName.gameObject.SetActive(false);
        efficiency.gameObject.SetActive(false);
        ui.SetCursorOnButton(false);
    }
}
