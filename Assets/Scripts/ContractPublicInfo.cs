using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ContractPublicInfo : MonoBehaviour
{
    public Contract contract;

    private void Awake()
    {

    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       if(contract != null)
        {
            if(contract.contractStats != null)
            {
                if(contract.contractStats.isContractStarted)
                {
                    ContractIDFrmGrid.text = "Contract " + contract.contractStats.ContractID;
                    CreateContractUIButton.interactable = false;
                    ContractUIButtonInfo.text = "Contract Started";
                    DeleteContractButton.interactable = true;
                }
            }
            else
            {
                ContractIDFrmGrid.text = "";
                CreateContractUIButton.interactable = true;
                ContractUIButtonInfo.text = "Not Started";
                DeleteContractButton.interactable = false;
            }

        }

    }


    public int GetStaticContractID()
    {
        return StaticConttractID;
    }

    //PUBLIC VARIABLES
    public Button ContractUIButton;
    public Button CreateContractUIButton;
    public Button DeleteContractButton;
    public int StaticConttractID;
    public int GridContractID;
    public Text ContractIDFrmGrid;
    public Text ActivePlantsInContract;
    public Text AssignedDroneToContract;
    public Text ContractUIButtonInfo;


}
