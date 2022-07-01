using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    [HideInInspector] public static UI Instance;

    [SerializeField] GameObject startUI, cycles, resources;
    [SerializeField] List<GameObject> menus = new List<GameObject>();
    [SerializeField] List<GameObject> buildingPanels = new List<GameObject>();
    [SerializeField] List<GameObject> necessaryPanels = new List<GameObject>();
    [SerializeField] GameObject cells;

    [SerializeField] Text[] resourceTexts = new Text[5];

    [SerializeField] Text cycleNumber;    

    [SerializeField] List<Button> speedButtons = new List<Button>();

    [SerializeField] Sprite[] resourcesSprites = new Sprite[5];

    [Header("Warnings")]
    [SerializeField] GameObject noWorkplace;
    [SerializeField] GameObject storageIsFull;
    [SerializeField] GameObject noWayToEmpirePortal;
    [SerializeField] GameObject enemyNear;
    [SerializeField] GameObject attackRepulsed;

    [Header("Buildings Type")]
    [SerializeField] List<Button> buildingsTypeButtons = new List<Button>();
    [SerializeField] List<GameObject> buildingsType = new List<GameObject>();

    [Header("PersonsList")]
    [SerializeField] GameObject personsList;
    [SerializeField] SortButtonGroup listSortButtonGroup;
    [SerializeField] GameObject personListStat;
    [SerializeField] Transform personsListStats;

    [Header("PersonCreator")]
    [SerializeField] GameObject personCreatorMenu;
    [SerializeField] GameObject apearanceMenu;
    [SerializeField] InputField fullName;
    [SerializeField] Image hairImg;
    [SerializeField] Image beardImg;
    [SerializeField] Image hairColorImg;
    [SerializeField] Image beardColorImg;
    [SerializeField] List<Sprite> hairs = new List<Sprite>();
    [SerializeField] List<Sprite> beards = new List<Sprite>();
    [SerializeField] List<Color> hairColors = new List<Color>();

    [Header("Feed persons")]
    [SerializeField] GameObject feedPersonsMenu;
    [SerializeField] Text maxFood;
    [SerializeField] GameObject stillNeedWarning;
    [SerializeField] SortButtonGroup feedSortButtonGroup;
    [SerializeField] GameObject feedPersonStat;
    [SerializeField] Transform feedPersonStats;

    [Header("Person")]
    [SerializeField] GameObject personPanel;
    [SerializeField] Text personName;
    [SerializeField] GameObject hungry;

    [Header("Empire portal")]
    [SerializeField] GameObject empirePortalPanel;
    [SerializeField] Text cyclesToNewPerson;
    [SerializeField] GameObject empirePortalHP;
    [SerializeField] Text empirePortalHPText;

    [Header("War portal")]
    [SerializeField] GameObject warPortalPanel;
    [SerializeField] GameObject[] warPortalsUI = new GameObject[4];
    [SerializeField] Text[] timeToAttackTexts = new Text[4];

    [Header("Storage Panel")]
    [SerializeField] GameObject storagePanel;
    [SerializeField] Text storageNameText;
    [SerializeField] Image storageResource;
    [SerializeField] Text storageCapasityText;

    [Header("Industrial Panel")]
    [SerializeField] GameObject industrialPanel;
    [SerializeField] Text industrialName;
    [SerializeField] Image industrialResource;
    [SerializeField] Text industrialProduction;
    [SerializeField] Text industrialWorkersCount;
    [SerializeField] GameObject industrialPersonStat;
    [SerializeField] Transform industrialPersonStats;

    [Header("Fortification Panel")]
    [SerializeField] GameObject fortificationPanel;
    [SerializeField] Text fortificationName;

    [Header("Combat Panel")]
    [SerializeField] GameObject combatPanel;
    [SerializeField] Text combatName;
    [SerializeField] Text damage;
    [SerializeField] Text attackSpeed;
    [SerializeField] Text radius;
    [SerializeField] Text rotateSpeed;
    [SerializeField] Text projectileSpeed;
    [SerializeField] GameObject combatPersonStat;
    [SerializeField] Transform combatPersonStats;

    [Header("Attack priority")]
    [SerializeField] Text priorityText;
    [SerializeField] Image priorityImage;
    [SerializeField] Sprite[] prioritySprites = new Sprite[6];

    EnemyAttacks enemyAttacks;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        SetTimeSpeedButton(1);
        enemyAttacks = EnemyAttacks.Instance;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            bool activeNecessaryPanel = false;
            foreach (GameObject gameObject in necessaryPanels)
            {
                if (gameObject.activeInHierarchy)
                    activeNecessaryPanel = true;
            }
            if (!activeNecessaryPanel)
            {
                DisableAllMenu();
                DisableAllBuildingPanels();
                Disable(cells);
                if(!enemyAttacks.IsEnemyAttack())
                    Enable(startUI);
                SetCursorOnButton(false);
            }
        }
    }

    void DisableAllMenu()
    {
        foreach (GameObject menu in menus)
        {
            Disable(menu);
        }
        if(personsList.activeSelf)
            DisablePersonsList();
    }

    void DisableAllBuildingPanels()
    {
        foreach (GameObject panel in buildingPanels)
        {
            Disable(panel);
        }
    }

    public void EnableStartUI()
    {
        Enable(startUI);
    }

    public void DisnableStartUI()
    {
        Disable(startUI);
    }

    public void DisableCells()
    {
        Disable(cells);
    }


    public void Enable(GameObject element)
    {
        element.SetActive(true);
    }

    public void Disable(GameObject element)
    {
        element.SetActive(false);
    }

    public void UpdateCycleNumber(int number)
    {
        cycleNumber.text = number.ToString();
    } 

    public void SetTimeSpeedButton(int position)
    {
        foreach (Button button in speedButtons)
        {
            button.interactable = true;
        }
        speedButtons[position].interactable = false;
    }

    public bool cursorOnButton { get; private set; } = false;
    public void DisableAllPanels()
    {
        if (startUI.activeInHierarchy && !cursorOnButton)
            DisableAllBuildingPanels();
    }

    // fot time scale button
    public void SetCursorOnButton(bool value)
    {
        cursorOnButton = value;
    }

    public bool EnabledStartUI()
    {
        return startUI.activeInHierarchy;
    }

    public bool EnabledBuildingsGrid()
    {
        return cells.activeInHierarchy;
    }

    //Warnings

    public void NoWorkplace()
    {
        StartCoroutine(ShowWarning(noWorkplace));
    }

    public void StorageIsFull()
    {
        StartCoroutine(ShowWarning(storageIsFull));
    }

    public void SetNoWayToEmpirePortal(bool value)
    {
        noWayToEmpirePortal.SetActive(value);
    }

    public void SetEnemyNearText(bool value)
    {
        enemyNear.SetActive(value);
    }

    public void AttackRepulsed()
    {
        StartCoroutine(ShowWarning(attackRepulsed));
    }

    IEnumerator ShowWarning(GameObject gameObject)
    {
        gameObject.SetActive(true);
        yield return new WaitForSeconds(3);
        gameObject.SetActive(false);
    }

    //BuildingsType

    public void SetBuildingsType(int type)
    {
        foreach (Button button in buildingsTypeButtons)
        {
            button.interactable = true;
        }
        foreach (GameObject gameObject in buildingsType)
        {
            gameObject.SetActive(false);
        }
        buildingsTypeButtons[type].interactable = false;
        buildingsType[type].SetActive(true);
    }

    //PersonsList

    public void EnablePersonsList(List<Person> persons)
    {
        SpawnListStatBlocks(persons);
        listSortButtonGroup.StartWork();
    }

    public void DisablePersonsList()
    {
        foreach (Transform transform in personsListStats)
        {
            Destroy(transform.gameObject);
        }
        Disable(personsList);
        listSortButtonGroup.SetNeutralAllButtons();
    }

    void SpawnListStatBlocks(List<Person> persons)
    {
        
        for (int i = 0; i < persons.Count; i++)
        {
            PersonListStat block = Instantiate(personListStat, personsListStats).GetComponent<PersonListStat>();
            string workplace = "-";
            int number = 0;
            if (persons[i].workplace != null)
            {
                workplace = persons[i].workplace.buildingsName;
                IWorkplace iWorcplace = (IWorkplace)persons[i].workplace;
                number = iWorcplace.FindIndex(persons[i]);
            }
            block.ListInitialization(number, persons[i], persons[i].isHungry, persons[i].fullName, workplace);
        }
    }

    public void SortPersonsList(PersonListStat[] personsListStats)
    {
        for (int i = 0; i < personsListStats.Length; i++)
        {
            foreach (Transform transform in this.personsListStats)
            {
                if (transform.GetComponent<PersonListStat>() == personsListStats[i])
                    transform.SetSiblingIndex(i);
            }
        }
    }

    //Resources

    public void UpdateResourceNumber(Resource resource, float resourceNumber, int capasityNumber)
    {
        resourceTexts[(int)resource].text = resourceNumber.ToString() + "/" + capasityNumber.ToString();
    }

    public void UpdateResourceNumber(Resource resource, float resourceNumber)
    {
        resourceTexts[(int)resource].text = resourceNumber.ToString();
    }

    //PersonCreator

    public void EnablePersonCreatorMenu()
    {
        if (EnabledStartUI())
        {
            DisableAllBuildingPanels();
            Disable(startUI);
            Disable(cycles);
            Disable(resources);
            EnableApearanceMenu(true);
            Enable(personCreatorMenu);
        }
    }

    public void DisablePersonCreatorMenu()
    {
        if(!enemyAttacks.IsEnemyAttack())
            Enable(startUI);
        Enable(cycles);
        Enable(resources);
        Disable(personCreatorMenu);
    }

    public void SetPersonCreatorName(string name)
    {
        fullName.text = name;
    }

    public void SetHair(int value)
    {
        hairImg.sprite = hairs[value];
    }

    public void SetBeard(int value)
    {
        beardImg.sprite = beards[value];
    }

    public void SetHairColor(int value)
    {
        hairColorImg.color = hairColors[value];
    }

    public void SetBeardColor(int value)
    {
        beardColorImg.color = hairColors[value];
    }

    public void EnableApearanceMenu(bool value)
    {
        apearanceMenu.SetActive(value);
    }

    public string GetName()
    {
        return fullName.text;
    }

    //Feed Persons

    int fedPersons = 0, maxFoodCount;
    public void EnableFeedPersonsMenu(List<Person> persons, int maxFood)
    {
        DisableAllMenu();
        Disable(cells);
        DisableAllBuildingPanels();
        Disable(startUI);
        Disable(cycles);
        Enable(feedPersonsMenu);
        SpawnFeedStatBlocks(persons, maxFood);
        feedSortButtonGroup.StartWork();
    }

    public bool FinishFeed()
    {
        if (fedPersons == maxFoodCount)
        {
            foreach (Transform transform in feedPersonStats)
            {
                transform.GetComponent<FeedStatBlock>().FinishFeed();
                Destroy(transform.gameObject);
            }
            if(!enemyAttacks.IsEnemyAttack())
                Enable(startUI);
            Enable(cycles);
            Disable(feedPersonsMenu);
            fedPersons = 0;
            feedSortButtonGroup.SetNeutralAllButtons();
            return true;
        }
        else
        {
            StartCoroutine(ShowWarning(stillNeedWarning));
            return false;
        }
    }

    void SpawnFeedStatBlocks(List<Person> persons, int maxFood)
    {
        maxFoodCount = maxFood;
        this.maxFood.text = fedPersons + "/" + maxFoodCount;
        for (int i = 0; i < persons.Count; i++)
        {
            FeedStatBlock block = Instantiate(feedPersonStat, feedPersonStats).GetComponent<FeedStatBlock>();
            string workplace = "-";
            if (persons[i].workplace != null)
                 workplace = persons[i].workplace.buildingsName;
            block.FeedInitialization(persons[i], persons[i].isHungry, persons[i].fullName, workplace);
        }
    }

    public void SortFeedStatBlocks(FeedStatBlock[] feedStatBlocks)
    {
        for (int i = 0; i < feedStatBlocks.Length; i++)
        {
            foreach (Transform transform in feedPersonStats)
            {
                if (transform.GetComponent<FeedStatBlock>() == feedStatBlocks[i])
                    transform.SetSiblingIndex(i);
            }
        }
    }

    public void GiveFood()
    {
        fedPersons++;
        maxFood.text = fedPersons + "/" + maxFoodCount;
    }

    public void TakeFood()
    {
        fedPersons--;
        maxFood.text = fedPersons + "/" + maxFoodCount;
    }

    public bool HasFood()
    {
        return maxFoodCount > fedPersons;
    }

    //Person

    public void EnablePersonPanel(string name, bool hungry)
    {
        if (EnabledStartUI())
        {
            DisableAllBuildingPanels();
            Enable(personPanel);
            personName.text = name;
            this.hungry.SetActive(hungry);
        }
    }

    //Building
    //EmpirePortal

    public void EnableEmpirePortalPanel()
    {
        if(EnabledStartUI())
        {
            DisableAllBuildingPanels();
            Enable(empirePortalPanel);
        }
    }

    public bool IsEmpirePortalPanelEnabled()
    {
        return empirePortalPanel.activeSelf;
    }

    public void SetCyclesToNewPerson(float cycles)
    {
        cyclesToNewPerson.text = "Cycles to a new person: " + cycles.ToString("0.##");
    }

    public void SetEnableEmpirePortalHP(bool value)
    {
        empirePortalHP.SetActive(value);
    }

    public void SetEmpirePortalHP(int hp)
    {
        empirePortalHPText.text = hp.ToString();
    }

    //War portal

    public void EnableWarPortalPanel()
    {
        if (EnabledStartUI())
        {
            DisableAllBuildingPanels();
            Enable(warPortalPanel);
        }
    }

    public void EnableWarPortalUI(int portal)
    {
        DisableWarPortalsUI();
        warPortalsUI[portal].SetActive(true);
    }

    public void DisableWarPortalsUI()
    {
        foreach (GameObject gameObject in warPortalsUI)
        {
            gameObject.SetActive(false);
        }
    }

    public void SetTimeToAttack(int portal, float time)
    {
        if (!warPortalsUI[portal].activeInHierarchy)
            EnableWarPortalUI(portal);
        timeToAttackTexts[portal].text = time.ToString("0.##");
    }

    //Storage

    public void EnableStoragePanel(string storageName, int resource, int capasity)
    {
        if (EnabledStartUI())
        {
            DisableAllBuildingPanels();
            Enable(storagePanel);
            storageNameText.text = storageName;
            storageResource.sprite = resourcesSprites[resource];
            storageCapasityText.text = capasity.ToString();
        }
    }

    //Industrial

    public void EnableIndustrialPanel(IndustrialBuilding building, string name, int resource, float production, List<Person> persons, int maxWorkers)
    {
        if (EnabledStartUI())
        {
            DisableAllBuildingPanels();
            Enable(industrialPanel);
            industrialName.text = name;
            industrialResource.sprite = resourcesSprites[resource];
            industrialProduction.text = production.ToString("0.##");
            SpawnIndustrialStatBlocks(building, persons, maxWorkers);
        }
    }

    public void UpdateIndustrialWorkers(IndustrialBuilding building, float production, List<Person> persons, int maxWorkers)
    {
        if (industrialPanel.activeInHierarchy)
        {
            industrialProduction.text = production.ToString("0.##");
            SpawnIndustrialStatBlocks(building, persons, maxWorkers);
        }
    }

    void SpawnIndustrialStatBlocks(IndustrialBuilding building, List<Person> persons, int maxWorkers)
    {
        foreach (Transform child in industrialPersonStats)
            Destroy(child.gameObject);
        industrialWorkersCount.text = "Workers: " + persons.Count.ToString() + "/" + maxWorkers.ToString();
        for (int i = 0; i < persons.Count; i++)
        {
            PersonStatBlock block = Instantiate(industrialPersonStat, industrialPersonStats).GetComponent<PersonStatBlock>();
            block.Initialization(i, building, persons[i].isHungry, persons[i].fullName);
        }
    }

    public void SetWorkIndicator(int number, float value)
    {
        if (industrialPanel.activeInHierarchy)
        {
            industrialPersonStats.GetChild(number).GetComponent<PersonStatBlock>().SetWorkIndicator(value);
        }
    }

    //FortificationPanel

    public void EnableFortificationPanel(string name)
    {
        if (EnabledStartUI())
        {
            DisableAllBuildingPanels();
            Enable(fortificationPanel);
            fortificationName.text = name;
        }
    }

    //CombatPanel

    CombatBuilding combatBuilding;

    public void EnableCombatPanel(string name, IWorkplace building, Person person, int damage, float attackSpeed, float radius, float rotateSpeed, float projectileSpeed)
    {
        if (EnabledStartUI())
        {
            DisableAllBuildingPanels();
            Enable(combatPanel);
            combatName.text = name;
            this.damage.text = "Damage: " + damage.ToString();
            this.attackSpeed.text = "Attack speed: " + attackSpeed.ToString();
            this.radius.text = "Radius: " + radius.ToString();
            this.rotateSpeed.text = "Rotate speed: " + rotateSpeed.ToString();
            this.projectileSpeed.text = "Projectile speed: " + projectileSpeed.ToString();
            if (combatPersonStats.childCount > 0)
                Destroy(combatPersonStats.GetChild(0).gameObject);
            if (person != null)
                SpawnCombatStatBlock(building, person);
        }
    }

    public void SetBuildingForPriority(CombatBuilding building)
    {
        combatBuilding = building;
        SetPriority((int)combatBuilding.priority);
    }


    public void UpdateCombatStatBlock(IWorkplace building, Person person, float attackSpeed)
    {
        if (combatPanel.activeInHierarchy)
        {
            this.attackSpeed.text = "Attack speed: " + attackSpeed.ToString();
            SpawnCombatStatBlock(building, person);
        }
    }

    void SpawnCombatStatBlock(IWorkplace building, Person person)
    {
        PersonStatBlock block = Instantiate(combatPersonStat, combatPersonStats).GetComponent<PersonStatBlock>();
        block.Initialization(0, building, person.isHungry, person.fullName);
    }

    void SetPriority(int priority)
    {
        priorityImage.sprite = prioritySprites[priority];
        combatBuilding.priority = (Priority)priority;
        priorityText.text = combatBuilding.priority.ToString();
    }

    public void NextPriority()
    {
        int i = (int)combatBuilding.priority + 1;
        if (i > 5)
            i = 0;
        SetPriority(i);
    }

    public void LastPriority()
    {
        int i = (int)combatBuilding.priority - 1;
        if (i < 0)
            i = 5;
        SetPriority(i);
    }
}
