using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ContractController : MonoBehaviour
{

    [SerializeField]
    FieldsManager fieldsManager;


    [SerializeField]
    List<ContractGrid> ContractsGrid = new List<ContractGrid>();
    [SerializeField]
    Color pickingColor;
    // Sprite pickingImg,defaultImage;

    public int ChosenContractIDFroAuction;
    public bool isGridSelected;


    bool isCountDownStarted;
    public bool isBidding;
    public float countDownCounter;
    float gridRefreshTime;
    [SerializeField]
    Text countDownText;
    int gridSelector;



    public Contract CurrentContract;
    public ContractGrid Grid;
    public bool isGridsContractChosen;
    public List<int> ContractIDS = new List<int>();

    private void Awake()
    {
        Instance = this;

        SetContractsGrid();
    }



    public void StartCountDown(bool isOn)
    {
        isCountDownStarted = isOn;

    }

    public void RemoveCurrentContractGridData(int contractID)
    {
        ContractsGrid[contractID].RemoveLinkedGridWithContract(contractID);
    }



    public void ResetBidding()
    {
        ContractsGrid.ForEach(c => c.GetComponent<Image>().color = Color.white);
        ContractsGrid.ForEach(c => c.GetComponent<Button>().interactable = false);
        isBidding = false;
        gridSelector = 0;
    }

    // Update is called once per frame
    void Update()
    {
        countDownText.text = "Auction bidding starts in - " + countDownCounter.ToString("F0");
        if (isCountDownStarted)
        {
            if (countDownCounter > 0)
            {
                countDownCounter -= Time.deltaTime;
            }
            else
            {

                countDownCounter = 5;
                isCountDownStarted = false;
                isBidding = true;
            }
        }
        if (isBidding)
        {

            countDownText.text = "Bid now!";
            if (gridRefreshTime < 2)
            {
                gridRefreshTime += Time.deltaTime;
            }
            else
            {
                if (gridSelector <= ContractsGrid.Count)
                {

                    ContractsGridLoop();
                }
                else
                    isBidding = false;
            }

        }

    }



    void SetContractsGrid()
    {
        for (int i = 0; i < ContractsGrid.Count; i++)
        {
            ContractsGrid[i].SetGrid(i);
        }
        isCountDownStarted = false;
        countDownText.text = "";
        gridSelector = 0;
        countDownCounter = 5;
        gridRefreshTime = 0;
        ContractsGrid.ForEach(c => c.GetComponent<Button>().interactable = false);
    }


    void ContractsGridLoop()
    {

        ContractsGrid.ForEach(c => c.GetComponent<Image>().color = Color.white);
        ContractsGrid.ForEach(c => c.GetComponent<Button>().interactable = false);
        ContractsGrid[gridSelector].GetComponent<Button>().interactable = true;
        ContractsGrid[gridSelector].GetComponent<Image>().color = pickingColor;
        gridSelector += 1;
        gridRefreshTime = 0;



    }



    public void LinkCurrentGridWithContract()
    {
        var selectedGrid = EventSystem.current.currentSelectedGameObject.GetComponent<ContractGrid>();
        Grid = selectedGrid;
    }

    public void RemoveStartedGridContract(int GridNumber)
    {
        ContractsGrid.RemoveAt(GridNumber);
    }


    /// <summary>
    /// Getting the db data for each started contract
    /// </summary>
    public void GetDataForAllContracts()
    {
        foreach (var item in Contracts)
        {
            item.LoadContractData();
        }
    }


    public void SetCreatedContractsID(int contractID)
    {
        if (ContractIDS.Contains(contractID))
            ContractIDS.Add(contractID);
    }

    public void RemoveCreatedContractId(int contractID)
    {
        RemoveCurrentContractGridData(contractID);
        ContractIDS.Remove(contractID);
    }


    public void GridsData()
    {
        for (int i = 0; i < ContractsGrid.Count; i++)
        {
                ContractsGrid[i].GetGridData(i);
        }
    }

    public void ContractInteractibleButtons(bool isActive)
    {
        if (Contracts != null)
            Contracts.ForEach(c => c.CreateContractButton.interactable = isActive);
    }

    //PUBLIC VARIABLES 
    public static ContractController Instance;
    public List<Contract> Contracts = new List<Contract>();

}
