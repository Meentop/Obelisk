using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PersonListStat : PersonStatBlock
{
    public Person person { get; private set; }

    [SerializeField] Text workplace;

    public void ListInitialization(int number, Person person, Vector3 pointerPos, bool hungry, string name, string efficiency, string workplace)
    {
        this.number = number;
        this.person = person;
        pointer.localPosition = pointerPos;
        this.hungry.SetActive(hungry);
        fullName.text = name;
        this.efficiency.text = efficiency;
        this.workplace.text = workplace;
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        if (person.workplace != null)
        {
            ui.EnableStartUI();
            person.workplace.RemoveWorker(number);
            person.SpawnAfterBuilding();
            ui.DisablePersonsList();
        }
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        fullName.gameObject.SetActive(true);
        efficiency.gameObject.SetActive(true);
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        fullName.gameObject.SetActive(false);
        efficiency.gameObject.SetActive(false);
    }
}
