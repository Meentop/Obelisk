using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SortButton : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] Sprite neutral; //0
    [SerializeField] Sprite descending; //1
    [SerializeField] Sprite ascending; //2

    [SerializeField] SortBy sortBy;

    public SortButtonGroup group;
    int curStatus = 0;
    Image image;
    UI ui;

    private void Start()
    {
        ui = UI.Instance;
        image = transform.GetChild(0).GetComponent<Image>();
    }


    public void OnPointerClick(PointerEventData eventData)
    {
        group.SetNeutralButtons(this);
        DefineStatus();
        if (curStatus != 0)
        {
            if (group.isFeed)
            {
                if (sortBy == SortBy.Hungry)
                    FeedOrderBy(i => i.person.isHungry);
                else if (sortBy == SortBy.Industrial)
                    FeedOrderBy(i => i.person.industrialEfficiency);
            }
            else
            {
                if (sortBy == SortBy.Hungry)
                    ListOrderBy(i => i.person.isHungry);
                else if (sortBy == SortBy.Industrial)
                    ListOrderBy(i => i.person.industrialEfficiency);
                else if (sortBy == SortBy.Workplace)
                    ListOrderBy(i => i.person.workplace != null);
            }
        }
    }

    void DefineStatus()
    {
        if (curStatus == 0)
        {
            curStatus = 1;
            image.sprite = descending;
        }
        else if (curStatus == 1)
        {
            curStatus = 2;
            image.sprite = ascending;
        }
        else if (curStatus == 2)
        {
            curStatus = 0;
            SetNeutral();
        }
    }

    public void SetNeutral()
    {
        image.sprite = neutral;
        curStatus = 0;
    }

    void FeedOrderBy(System.Func<FeedStatBlock, bool>operation)
    {
        FeedStatBlock[] newArray = group.feedStatBlocks.OrderByDescending(operation).ToArray();
        if (curStatus == 1)
            ui.SortFeedStatBlocks(newArray);
        else if (curStatus == 2)
            ui.SortFeedStatBlocks(newArray.Reverse().ToArray());
    }

    void FeedOrderBy(System.Func<FeedStatBlock, float> operation)
    {
        FeedStatBlock[] newArray = group.feedStatBlocks.OrderByDescending(operation).ToArray();
        if (curStatus == 1)
            ui.SortFeedStatBlocks(newArray);
        else if (curStatus == 2)
            ui.SortFeedStatBlocks(newArray.Reverse().ToArray());
    }

    void ListOrderBy(System.Func<PersonListStat, bool> operation)
    {
        PersonListStat[] newArray = group.personListStats.OrderByDescending(operation).ToArray();
        if (curStatus == 1)
            ui.SortPersonsList(newArray);
        else if (curStatus == 2)
            ui.SortPersonsList(newArray.Reverse().ToArray());
    }

    void ListOrderBy(System.Func<PersonListStat, float> operation)
    {
        PersonListStat[] newArray = group.personListStats.OrderByDescending(operation).ToArray();
        if (curStatus == 1)
            ui.SortPersonsList(newArray);
        else if (curStatus == 2)
            ui.SortPersonsList(newArray.Reverse().ToArray());
    }
}



public enum SortBy 
{ 
    Hungry,
    Industrial,
    Workplace
}
