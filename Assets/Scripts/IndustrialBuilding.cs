using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndustrialBuilding : Building, IWorkplace
{
    Cycles cycles;

    [SerializeField] int maxWorkersCount = 1;
    [SerializeField] Resource resource;
    [SerializeField] float baseProduction;
    List<Person> workers = new List<Person>();
    [SerializeField] GameObject sleep;

    List<IEnumerator> works = new List<IEnumerator>();

    protected override void Start()
    {
        base.Start();
        cycles = Cycles.Instance;
    }

    public override void Click()
    {
        ui.EnableIndustrialPanel(this, buildingsName, (int)resource, AllProduction(), workers, maxWorkersCount);
    }

    public override void Destroy()
    {
        int count = workers.Count;
        for (int i = 0; i < count; i++)
        {
            RemoveWorker(0);
        }
        base.Destroy();
    }

    public bool HasWorkplace()
    {
        return workers.Count < maxWorkersCount;
    }


    public void AddWorker(GameObject person)
    {
        if(workers.Count < maxWorkersCount)
        {
            workers.Add(person.GetComponent<Person>());
            person.SetActive(false);
            person.GetComponent<Person>().workplace = this;
            StartWork(person.GetComponent<Person>());
            ui.UpdateIndustrialWorkers(this, AllProduction(), workers, maxWorkersCount);
            sleep.SetActive(false);
        }
    }

    public Person RemoveWorker(int number)
    {
        Person person = workers[number];
        person.workplace = null;
        FinishWork(number);
        person.gameObject.SetActive(true);
        workers.RemoveAt(number);
        if (workers.Count == 0)
            sleep.SetActive(true);
        return person;
    }

    float AllProduction()
    {
        float production = 0;
        foreach (Person person in workers)
        {
            float modifier = 1;
            if (person.isHungry)
                modifier = 0.75f;
            production += baseProduction * modifier;
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
            float timeForOneProduct = cycles.cycleTime / baseProduction;
            float curTime = 0;
            while (curTime < timeForOneProduct)
            {
                if (cycles.timeScale == 0)
                    yield return new WaitWhile(() => cycles.timeScale == 0);
                yield return new WaitForSecondsRealtime(Time.fixedDeltaTime);
                float modifier = 1;
                if (person.isHungry)
                    modifier = 0.75f;
                curTime += Time.fixedDeltaTime * cycles.timeScale * modifier;
                if(gameObject.GetComponent<Outline>().isActiveAndEnabled)
                    ui.SetWorkIndicator(FindIndex(person), curTime / timeForOneProduct);
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
