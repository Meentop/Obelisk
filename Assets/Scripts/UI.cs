using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    [SerializeField] GameObject startUI, cycles, resources;
    [SerializeField] List<GameObject> menus = new List<GameObject>();
    [SerializeField] List<GameObject> buildingPanels = new List<GameObject>();
    [SerializeField] GameObject cells;

    [SerializeField] Text[] resourceTexts = new Text[4];

    [SerializeField] Text cycleNumber;

    [SerializeField] List<Button> speedButtons = new List<Button>();

    [SerializeField] Sprite[] resourcesSprites = new Sprite[4];

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

    [Header("Obelisk Panel")]
    [SerializeField] GameObject obeliskPanel;
    [SerializeField] Text cyclesToNewPerson;

    [Header("Storage Panel")]
    [SerializeField] GameObject storagePanel;
    [SerializeField] Text storageNameText;
    [SerializeField] Image storageResource;
    [SerializeField] Text storageCapasityText;

    private void Start()
    {
        SetNotInteractable(1);
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

    public void SetNotInteractable(int position)
    {
        foreach (Button button in speedButtons)
        {
            button.interactable = true;
        }
        speedButtons[position].interactable = false;
    }

    bool permitOnDisableBuildingsPanel = true;
    public void ClickOnGround()
    {
        if (startUI.activeInHierarchy && permitOnDisableBuildingsPanel)
            DisableAllBuildingPanels();
    }

    public void SetPermitOnDisabledBuildingsPanel(bool permit)
    {
        permitOnDisableBuildingsPanel = permit;
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
        if (startUI.activeInHierarchy)
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

    public void SetName(string name)
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

    //Building
    //Obelisk

    public void EnableObeliskPanel()
    {
        if(startUI.activeInHierarchy)
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
        if (startUI.activeInHierarchy)
        {
            DisableAllBuildingPanels();
            Enable(storagePanel);
            storageNameText.text = storageName;
            storageResource.sprite = resourcesSprites[resource];
            storageCapasityText.text = capasity.ToString();
        }
    }
}
