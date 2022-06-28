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
            curAttackSpeed = baseAttackSpeed * (this.person.efficiencyModifier / 100);
            ui.UpdateCombatStatBlock(this, this.person, curAttackSpeed);
        }
    }

    public Person RemoveWorker(int number)
    {
        Person person = this.person;
        person.workplace = null;
        person.gameObject.SetActive(true);
        person.inCombatBuilding = false;
        this.person = null;
        return person;
    }

    public int FindIndex(Person person)
    {
        return 0;
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