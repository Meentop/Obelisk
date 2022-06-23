using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SortButtonGroup : MonoBehaviour
{
    [SerializeField] List<SortButton> sortButtons = new List<SortButton>();

    [SerializeField] Transform parentForSortObjects;
    public bool isFeed;

    [HideInInspector] public FeedStatBlock[] feedStatBlocks;
    [HideInInspector] public PersonListStat[] personListStats;

    public void StartWork()
    {
        foreach (SortButton sortButton in sortButtons)
            sortButton.group = this;
        if (isFeed)
        {
            feedStatBlocks = new FeedStatBlock[parentForSortObjects.childCount];
            for (int i = 0; i < parentForSortObjects.childCount; i++)
                feedStatBlocks[i] = parentForSortObjects.GetChild(i).GetComponent<FeedStatBlock>();
        }
        else
        {
            personListStats = new PersonListStat[parentForSortObjects.childCount];
            for (int i = 0; i < parentForSortObjects.childCount; i++)
                personListStats[i] = parentForSortObjects.GetChild(i).GetComponent<PersonListStat>();
        }
    }

    public void SetNeutralButtons(SortButton expectButton)
    {
        foreach (SortButton sortButton in sortButtons)
        {
            if (sortButton != expectButton)
                sortButton.SetNeutral();
        }
    }

    public void SetNeutralAllButtons()
    {
        foreach (SortButton sortButton in sortButtons)
        {
            sortButton.SetNeutral();
        }
    }
}
