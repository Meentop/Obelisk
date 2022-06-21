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

    IEnumerator[] works;

    protected override void Start()
    {
        base.Start();
        cycles = Cycles.Instance;
        works = new IEnumerator[workersCount];
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
            StartWork(workers.Count - 1);
            ui.UpdateIndustrialWorkers(this, AllProduction(), workers, workersCount);
        }
    }

    public Person RemoveWorker(int number)
    {
        Person person = workers[number];
        person.gameObject.SetActive(true);
        FinishWork(number);
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

    void StartWork(int number)
    {
        FinishWork(number);
        works[number] = Work(number);
        StartCoroutine(works[number]);
    }

    void FinishWork(int number)
    {
        if (works[number] != null)
            StopCoroutine(works[number]);
    }

    IEnumerator Work(int number)
    {
        float productionInCycle = baseProduction * ((workers[number].industrialEfficiency + workers[number].efficiencyModifier) / 100);
        float timeForOneProduct = cycles.cycleTime / productionInCycle;
        float curTime = 0;
        while (true)
        {
            yield return new WaitForSecondsRealtime(0.1f);
            workers[number].AddIndustrialExp(cycles.timeScale);
            curTime += 0.1f * cycles.timeScale;
            if (curTime >= timeForOneProduct)
            {
                resources.AddResource(resource, 1);
                StartWork(number);
            }
        }
    }
}
