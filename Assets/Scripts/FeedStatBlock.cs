using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FeedStatBlock : PersonStatBlock
{
    public Person person { get; private set; }

    [SerializeField] Text workplace;
    [SerializeField] Button feedButton;

    [HideInInspector] public bool fed = false;

    public virtual void FeedInitialization(Person person, bool hungry, string name, string workplace)
    {
        this.person = person;
        this.hungry.SetActive(hungry);
        fullName.text = name;
        this.workplace.text = workplace;
    }

    void SetFed(bool value)
    {
        fed = value;
        if (fed)
        {
            SetFeedButtonColor(new Color(1, 1, 1, 0.333f), new Color(1, 1, 1, 0.333f));
            ui.GiveFood();
        }
        else
        {
            SetFeedButtonColor(new Color(0, 0, 0, 0.333f), new Color(0.5f, 0.5f, 0.5f, 0.333f));
            ui.TakeFood();
        }
    }

    void SetFeedButtonColor(Color normalColor, Color highlightedColor)
    {
        ColorBlock cb = feedButton.colors;
        cb.normalColor = normalColor;
        cb.highlightedColor = highlightedColor;
        feedButton.colors = cb;
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

    public void Click()
    {
        if (ui.HasFood() || fed)
            SetFed(!fed);
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        
    }
}
