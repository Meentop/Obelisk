using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CombatBuilding : Building, IWorkplace
{
    [SerializeField] int damage;
    [SerializeField] float baseAttackSpeed;
    float curAttackSpeed;
    [SerializeField] float radius;
    [SerializeField] float rotateSpeed;
    [SerializeField] float projectileSpeed;

    Person person = null;

    public Priority priority = Priority.First;


    public override void Click()
    {
        //before enemies attack set cur speed attack
        SetCurAttackSpeed();
        ui.EnableCombatPanel(buildingsName, this, person, damage, curAttackSpeed, radius, rotateSpeed, projectileSpeed);
        ui.SetBuildingForPriority(this);
    }

    public bool HasWorkplace()
    {
        return person == null;
    }

    public void AddWorker(GameObject person)
    {
        if (this.person == null)
        {
            this.person = person.GetComponent<Person>();
            person.SetActive(false);
            person.GetComponent<Person>().workplace = this;
            this.person.inCombatBuilding = true;
            SetCurAttackSpeed();
            ui.UpdateCombatStatBlock(this, this.person, curAttackSpeed);
        }
    }

    public Person RemoveWorker(int number)
    {
        Person person = this.person;
        person.workplace = null;
        person.gameObject.SetActive(true);
        person.inCombatBuilding = false;
        curAttackSpeed = 0;
        this.person = null;
        return person;
    }

    public int FindIndex(Person person)
    {
        return 0;
    }

    void SetCurAttackSpeed()
    {
        if (person != null)
        {
            float modifier = 1;
            if (person.isHungry)
                modifier = 0.75f;
            curAttackSpeed = baseAttackSpeed * modifier;
        }
    }
}

public enum Priority 
{ 
    First,
    Last,
    Weakest,
    Strongest,
    Nearest,
    Random
}