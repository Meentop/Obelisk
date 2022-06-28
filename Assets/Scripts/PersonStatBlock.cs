using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PersonStatBlock : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    protected int number;

    IWorkplace building;

    [SerializeField] protected GameObject hungry;

    [SerializeField] protected Text fullName;

    [SerializeField] protected Sprite combatImg, industrialImg;

    [SerializeField] protected Image combat;

    protected UI ui;

    private void Start()
    {
        ui = UI.Instance;
    }

    public virtual void Initialization(int number, IWorkplace building, bool hungry, string name, bool combat)
    {
        this.number = number;
        this.building = building;
        this.hungry.SetActive(hungry);
        fullName.text = name;
        if (combat)
            this.combat.sprite = combatImg;
        else
            this.combat.sprite = industrialImg;
    }

    public virtual void OnPointerDown(PointerEventData eventData)
    {
        Person person = building.RemoveWorker(number);
        person.SpawnAfterBuilding();
        ui.SetCursorOnButton(false);
    }

    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        ui.SetCursorOnButton(true);
    }

    public virtual void OnPointerExit(PointerEventData eventData)
    {
        ui.SetCursorOnButton(false);
    }
}
