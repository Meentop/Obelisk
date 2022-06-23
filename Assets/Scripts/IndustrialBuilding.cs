using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndustrialBuilding : Building
{
    Cycles cycles;

    [SerializeField] int workersCount = 1;
    [SerializeField] Resource resource;
    [SerializeField] float baseProduction;
    [SerializeField] List<Person> workers = new List<Person>();

    List<IEnumerator> works = new List<IEnumerator>();

    protected override void Start()
    {
        base.Start();
        cycles = Cycles.Instance;
    }

    public override void Click()
    {
        ui.EnableIndustrialPanel(this, buildingsName, (int)resource, AllProduction(), workers, workersCount);
    }

    public bool HasWorkplace()
    {
        return workers.Count < workersCount;
    }


    public void AddWorker(GameObject person)
    {
        if(workers.Count < workersCount)
        {
            workers.Add(person.GetComponent<Person>());
            person.SetActive(false);
            person.GetComponent<Person>().workplace = this;
            StartWork(person.GetComponent<Person>());
            ui.UpdateIndustrialWorkers(this, AllProduction(), workers, workersCount);
        }
    }

    public Person RemoveWorker(int number)
    {
        Person person = workers[number];
        person.workplace = null;
        FinishWork(number);
        person.gameObject.SetActive(true);
        workers.RemoveAt(number); 
        return person;
    }

    float AllProduction()
    {
        float production = 0;
        foreach (Person person in workers)
        {
            production += baseProduction * ((person.industrialEfficiency + person.efficiencyModifier) / 100);
        }
        return production;
    }

    //WorkProcess

    void StartWork(Person person)
    {
        works.Add(Work(person));
        StartCoroutine(works[works.Count - 1]);
    }

    void FinishWork(int number)
    {
        StopCoroutine(works[number]);
        works.Remove(works[number]);
    }

    IEnumerator Work(Person person)
    {
        while (true)
        {
            float productionInCycle = baseProduction * ((workers[FindIndex(person)].industrialEfficiency + workers[FindIndex(person)].efficiencyModifier) / 100);
            float timeForOneProduct = cycles.cycleTime / productionInCycle;
            float curTime = 0;
            while (curTime < timeForOneProduct)
            {
                yield return new WaitForSecondsRealtime(0.1f);
                workers[FindIndex(person)].AddIndustrialExp(cycles.timeScale);
                curTime += 0.1f * cycles.timeScale;
                if (curTime >= timeForOneProduct)
                    resources.AddResource(resource, 1);
            }
        }
    }

    public int FindIndex(Person person)
    {
        for (int i = 0; i < workers.Count; i++)
        {
            if (person == workers[i])
                return i;
        }
        return -1;
    }
}
