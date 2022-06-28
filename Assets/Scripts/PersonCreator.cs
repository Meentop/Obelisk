using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonCreator : MonoBehaviour
{
    [HideInInspector] public static PersonCreator Instance;

    UI ui;
    EmpirePortal empirePortal;
    [SerializeField] GameObject personPrefab;
    [SerializeField] Transform personSpawnPoint;
    Person person;

    [SerializeField] GameObject nameWarning;
    public string[] firstName = new string[3] { "vasy", "pety", "vany" };
    public string[] lastName = new string[3] { "big", "small", "strong" };

    [SerializeField] List<GameObject> hairs = new List<GameObject>();
    [SerializeField] List<GameObject> beards = new List<GameObject>();
    [SerializeField] List<Material> hairMaterials = new List<Material>();
    [SerializeField] List<GameObject> sex = new List<GameObject>();

    int curHair, curBeard, curHairMaterial, curBeardMaterial;

    private void Awake()
    {
        Instance = this;
        ui = UI.Instance;
    }

    private void Start()
    {
        empirePortal = EmpirePortal.Instance;
    }

    public void StartCreate()
    {
        ui.EnablePersonCreatorMenu();
        RandomName();
        Instantiate(personPrefab, personSpawnPoint);
        person = personSpawnPoint.GetChild(0).GetComponent<Person>();
        RandomAppearance();
    }

    public void RandomName()
    {
        ui.SetPersonCreatorName(firstName[Random.Range(0, firstName.Length)] + " " + lastName[Random.Range(0, lastName.Length)]);
    }

    public void RandomAppearance()
    {
        curHair = Random.Range(0, hairs.Count);
        curBeard = Random.Range(0, beards.Count);
        curHairMaterial = Random.Range(0, hairMaterials.Count);
        curBeardMaterial = Random.Range(0, hairMaterials.Count);
        SetHair(0);
        SetBeard(0);
        SetSex(Random.Range(0, 2));
    }



    public void SetHair(int value)
    {
        value += curHair;
        value = SetInBorber(value, hairs.Count);
        if (person.hairSpawnPoint.childCount >= 1)
            Destroy(person.hairSpawnPoint.GetChild(0).gameObject);
        hairs[value].GetComponent<MeshRenderer>().material = hairMaterials[curHairMaterial];
        Instantiate(hairs[value], person.hairSpawnPoint);
        ui.SetHair(value);
        ui.SetHairColor(curHairMaterial);
        curHair = value;
    }

    public void SetBeard(int value)
    {
        value += curBeard;
        value = SetInBorber(value, beards.Count);
        if (person.beardSpawnPoint.childCount >= 1)
            Destroy(person.beardSpawnPoint.GetChild(0).gameObject);
        beards[value].GetComponent<MeshRenderer>().material = hairMaterials[curBeardMaterial];
        Instantiate(beards[value], person.beardSpawnPoint);
        ui.SetBeard(value);
        ui.SetBeardColor(curBeardMaterial);
        curBeard = value;
    }

    public void SetHairColor(int value)
    {
        value += curHairMaterial;
        value = SetInBorber(value, hairMaterials.Count);
        person.hairSpawnPoint.GetChild(0).GetComponent<MeshRenderer>().material = hairMaterials[value];
        ui.SetHairColor(value);
        curHairMaterial = value;
    }

    public void SetBeardColor(int value)
    {
        value += curBeardMaterial;
        value = SetInBorber(value, hairMaterials.Count);
        person.beardSpawnPoint.GetChild(0).GetComponent<MeshRenderer>().material = hairMaterials[value];
        ui.SetBeardColor(value);
        curBeardMaterial = value;
    }

    // 0 = male 1 = female
    public void SetSex(int value)
    {
        if (person.bodySpawnPoint.childCount >= 1)
            Destroy(person.bodySpawnPoint.GetChild(0).gameObject);
        Instantiate(sex[value], person.bodySpawnPoint);
    }

    int SetInBorber(int value, int count)
    {
        if (value < 0)
            return count - 1;
        else if (value >= count)
            return 0;
        return value;
    }


    public void CheckNameLength()
    {
        nameWarning.SetActive(ui.GetName().Length > 30);
    }

    public void ConfirmAppearance()
    {
        if (ui.GetName().Length <= 30)
        {
            ui.EnableApearanceMenu(false);
            person.gameObject.SetActive(false);
        }
    }

    public void Confirm()
    {
        if (ui.GetPersonType() == 0)
            person.isCombat = true;
        if (ui.GetTypeButtons())
        {
            ui.DisablePersonCreatorMenu();
            person.fullName = ui.GetName();
            person.gameObject.SetActive(true);
            empirePortal.EndCreating(person.gameObject);
            Destroy(person.gameObject);
        }
    }
}
