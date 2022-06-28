using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FeedStatBlock : PersonStatBlock
{
    public Person person { get; private set; }

    [SerializeField] Text workplace;
    [SerializeField] Image feedButton;

    public bool fed = false;

    public virtual void FeedInitialization(Person person, bool hungry, string name, string workplace, bool combat)
    {
        this.person = person;
        this.hungry.SetActive(hungry);
        fullName.text = name;
        this.workplace.text = workplace;
        if (combat)
            this.combat.sprite = combatImg;
        else
            this.combat.sprite = industrialImg;
    }

    void SetFed(bool value)
    {
        fed = value;
        if (fed)
        {
            feedButton.color = Color.gray;
            ui.GiveFood();
        }
        else
        {
            feedButton.color = Color.white;
            ui.TakeFood();
        }
    }

    public void FinishFeed()
    {
        if(!fed)
        {
            if (!person.isHungry)
                person.isHungry = true;
            else
                person.Destroy();
        }
        else
            person.isHungry = false;
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        if(ui.HasFood() || fed)
            SetFed(!fed);
    }
}
