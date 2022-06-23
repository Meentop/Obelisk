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


    public virtual void FeedInitialization(Person person, Vector3 pointerPos, bool hungry, string name, string efficiency, string workplace)
    {
        this.person = person;
        pointer.localPosition = pointerPos;
        this.hungry.SetActive(hungry);
        fullName.text = name;
        this.efficiency.text = efficiency;
        this.workplace.text = workplace; 
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
