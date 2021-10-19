using DG.Tweening;
using Purria;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FieldsManager : MonoBehaviour
{
    public static FieldsManager Instance;

    public List<Plant> FieldPlants = new List<Plant>();
    public List<Contract> ContractFields = new List<Contract>();
    public List<FieldManager> FieldsManagerButtons = new List<FieldManager>();



    [SerializeField]
    ContractController contractController;

    [SerializeField]
    List<Button> DirtBlocks = new List<Button>();

    [SerializeField]
    RectTransform FieldHolder;
    public Contract currentContract;
    public FieldManager currentContractField;


    private void Awake()
    {
        Instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        SetAllContractsForFields();
        GetFieldsDropdownValue();
    }

    void SetAllContractsForFields()
    {
        for (int i = 0; i < contractController.Contracts.Count; i++)
        {
            ContractFields.Add(contractController.Contracts[i]);

        }

        var findFieldDirtBlocks = GameObject.FindGameObjectsWithTag("dirtblock");
        foreach (var item in findFieldDirtBlocks)
        {
            DirtBlocks.Add(item.GetComponent<Button>());
        }
        DirtBlocks.ForEach(b => b.gameObject.AddComponent<DirtBlock>());


        for (int i = 0; i < FieldsManagerButtons.Count; i++)
        {
            FieldsManagerButtons[i].field.StaticFieldID = i;
        }
    }


    public void CreateFieldWithContract()
    {
        var selectedContract = EventSystem.current.currentSelectedGameObject;
        if (selectedContract == null) return;
        var contract = selectedContract.transform.parent.GetComponent<Contract>();
        if (contract.contractStats != null && contract.contractStats.isContractStarted)
        {

            currentContract = contract;
            UIController.Instance.CloseContractsFieldsUI();
            LinkFieldsWithContractID();
            ResetDropdownValue();
            UIController.Instance.OpenFieldUI();


        }
        else
            MainManager.OnUIInfoOpen("Contract is not yet started", 3f);
    }

    public void ResetDropdownValue()
    {
        Dropdown contractFieldDD = UIController.Instance.ContractsFieldsDropdown;
        contractFieldDD.value = -1;
        contractFieldDD.Select(); 
        contractFieldDD.RefreshShownValue(); 
    }

    public void GetFieldsDropdownValue()
    {

        Dropdown contractFieldDD = UIController.Instance.ContractsFieldsDropdown;
        contractFieldDD.onValueChanged.AddListener(delegate
        {
            DropdownValueChanged(contractFieldDD);
        });
    }

    void DropdownValueChanged(Dropdown change)
    {
        ResetFieldManagerStatsOnExitField();
        currentContract = ContractFields[change.value];

        UIController.Instance.CloseContractsFieldsUI();
        LinkFieldsWithContractID();

    }

    void LinkFieldsWithContractID()
    {
        currentContract.InstantiatePlantsForContract();
        for (int i = 0; i < FieldsManagerButtons.Count; i++)
        {
            FieldsManagerButtons[i].SetFieldButton(currentContract);
        }
        GetAllFieldsData();
    }

    public void GetAllFieldsData()
    {

        for (int i = 0; i < FieldsManagerButtons.Count; i++)
        {
            FieldsManagerButtons[i].GetFieldData();
        }
    }

    public void SetDirtBlocksForField(List<Plant> plants)
    {
        for (int i = 0; i < DirtBlocks.Count; i++)
        {
            if (DirtBlocks[i].gameObject.GetComponent<DirtBlock>() != null)
            {
                DirtBlocks[i].GetComponent<DirtBlock>().SetDirtBlock(plants[i]);
            }
        }
        FieldHolder.DOAnchorPos(new Vector2(-2719.16f, -109), 0.5f);
    }
   



    public void ResetField()
    {
        UIController.Instance.CloseFieldUI();
    }

    void ResetFieldManagerStatsOnExitField()
    {
        for (int i = 0; i < FieldsManagerButtons.Count; i++)
        {
            FieldsManagerButtons[i].field.FieldContractID = 0;
            FieldsManagerButtons[i].field.isSet = false;
        }
    }

    public Contract GetFieldCurrentContract()
    {
        return currentContract;
    }

    public

    // Update is called once per frame
    void Update()
    {

    }
}
