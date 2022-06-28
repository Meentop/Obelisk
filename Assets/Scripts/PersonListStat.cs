using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PersonListStat : PersonStatBlock
{
    public Person person { get; private set; }

    [SerializeField] Text workplace;

    public void ListInitialization(int number, Person person, bool hungry, string name, string workplace, bool combat)
    {
        this.number = number;
        this.person = person;
        this.hungry.SetActive(hungry);
        fullName.text = name;
        this.workplace.text = workplace;
        if (combat)
            this.combat.sprite = combatImg;
        else
            this.combat.sprite = industrialImg;
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        if (person.workplace != null)
        {
            ui.EnableStartUI();
            IWorkplace iWorkplace = (IWorkplace)person.workplace;
            iWorkplace.RemoveWorker(number);
            person.SpawnAfterBuilding();
            ui.DisablePersonsList();
        }
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        
    }
}
