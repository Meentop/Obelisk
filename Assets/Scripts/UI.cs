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
    [SerializeField] GameObject cells;

    [SerializeField] Text[] resourceTexts = new Text[4];

    [SerializeField] Text cycleNumber;

    

    [SerializeField] List<Button> speedButtons = new List<Button>();

    [SerializeField] Sprite[] resourcesSprites = new Sprite[4];

    [Header("Warnings")]
    [SerializeField] GameObject noWorkplace;
    [SerializeField] GameObject storageIsFull;

    [Header("Buildings Type")]
    [SerializeField] List<Button> buildingsTypeButtons = new List<Button>();
    [SerializeField] List<GameObject> buildingsType = new List<GameObject>();

    [Header("PersonCreator")]
    [SerializeField] GameObject personCreatorMenu;
    [SerializeField] InputField fullName;
    [SerializeField] Image hairImg;
    [SerializeField] Image beardImg;
    [SerializeField] Image hairColorImg;
    [SerializeField] Image beardColorImg;
    [SerializeField] List<Sprite> hairs = new List<Sprite>();
    [SerializeField] List<Sprite> beards = new List<Sprite>();
    [SerializeField] List<Color> hairColors = new List<Color>();

    [Header("Person")]
    [SerializeField] GameObject personPanel;
    [SerializeField] Text personName;
    [SerializeField] Text personEffictiency;
    [SerializeField] Transform pointer;

    [Header("Obelisk Panel")]
    [SerializeField] GameObject obeliskPanel;
    [SerializeField] Text cyclesToNewPerson;

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
    [SerializeField] GameObject personStat;
    [SerializeField] Transform personStatSpawnPoint;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        SetTimeSpeedButton(1);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            DisableAllMenu();
            DisableAllBuildingPanels();
            Disable(cells);
            Enable(startUI);
        }
    }

    void DisableAllMenu()
    {
        foreach (GameObject menu in menus)
        {
            Disable(menu);
        }
    }

    void DisableAllBuildingPanels()
    {
        foreach (GameObject panel in buildingPanels)
        {
            Disable(panel);
        }
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
    public void ClickOnGround()
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

    //Warnings

    public void NoWorkplace()
    {
        print("what");
        StartCoroutine(ShowGameObject(noWorkplace));
    }

    public void StorageIsFull()
    {
        StartCoroutine(ShowGameObject(storageIsFull));
    }

    IEnumerator ShowGameObject(GameObject gameObject)
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

    //Resources

    public void UpdateResourceNumber(Resource resource, float resourceNumber, int capasityNumber)
    {
        if(resource == Resource.ResearchPoint)
            resourceTexts[(int)resource].text = resourceNumber.ToString();
        else
            resourceTexts[(int)resource].text = resourceNumber.ToString() + "/" + capasityNumber.ToString();
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
            Enable(personCreatorMenu);
        }
    }

    public void DisablePersonCreatorMenu()
    {
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


    public string GetName()
    {
        return fullName.text;
    }

    //Person

    public void EnablePersonPanel(string name, float combatEfficiency, float industrialEfficiency)
    {
        if (EnabledStartUI())
        {
            DisableAllBuildingPanels();
            Enable(personPanel);
            personName.text = name;
            personEffictiency.text = industrialEfficiency + "/" + combatEfficiency;
            pointer.localPosition = new Vector3((combatEfficiency - 50) * 2, 0, 0);
        }
    }

    //Building
    //Obelisk

    public void EnableObeliskPanel()
    {
        if(EnabledStartUI())
        {
            DisableAllBuildingPanels();
            Enable(obeliskPanel);
        }
    }

    public bool IsObeliskPanelEnabled()
    {
        return obeliskPanel.activeSelf;
    }

    public void SetCyclesToNewPerson(float cycles)
    {
        cyclesToNewPerson.text = "Cycles to a new person: " + cycles.ToString("0.##");
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
            SpawnPersonStatBlocks(building, persons, maxWorkers);
        }
    }

    public void UpdateIndustrialWorkers(IndustrialBuilding building, float production, List<Person> persons, int maxWorkers)
    {
        if (industrialPanel.activeInHierarchy)
        {
            industrialProduction.text = production.ToString("0.##");
            SpawnPersonStatBlocks(building, persons, maxWorkers);
        }
    }

    void SpawnPersonStatBlocks(IndustrialBuilding building, List<Person> persons, int maxWorkers)
    {
        PersonStatBlock[] statBlocks = FindObjectsOfType<PersonStatBlock>();
        for (int i = 0; i < statBlocks.Length; i++)
            Destroy(statBlocks[i].gameObject);
        industrialWorkersCount.text = "Workers: " + persons.Count.ToString() + "/" + maxWorkers.ToString();
        for (int i = 0; i < persons.Count; i++)
        {
            PersonStatBlock block = Instantiate(personStat, personStatSpawnPoint).GetComponent<PersonStatBlock>();
            block.transform.localPosition = new Vector3(0f, i * -70, 0f);
            block.number = i;
            block.building = building;
            block.pointer.localPosition = new Vector3((persons[i].combatEfficiency - 50) * 2, 0, 0);
            block.workerName.text = persons[i].fullName;
            block.efficiency.text = persons[i].industrialEfficiency + "/" + persons[i].combatEfficiency;
        }
    }
}
