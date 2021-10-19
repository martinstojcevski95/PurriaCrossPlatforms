using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Firebase.Database;
using Purria;
using UnityEngine;
using UnityEngine.UI;

public class FieldManager : MonoBehaviour
{

    //  public int FieldContractID;
    //  public bool isSet;
    //  public int StaticFieldID;

    public List<Plant> FieldContractPlants = new List<Plant>();
    [SerializeField] Text FieldIDText;
    public Button AddFieldButton;

    public Contract currentFieldContract;
    public Field field;

    // Start is called before the first frame update
    private void Start()
    {

    }


    public void SetFieldButton(Contract contract)
    {
        currentFieldContract = contract;
        FieldContractPlants = currentFieldContract.plants;
        // FieldContractID  = currentFieldContract.contractStats.ContractID;
        field.FieldContractID = currentFieldContract.contractStats.ContractID;
        FieldIDText.text = "Field " + field.StaticFieldID + " for contract " + field.FieldContractID;
        currentFieldContract.contractStats.FieldsIds.Add(field.StaticFieldID);
        AddFieldButton.onClick.AddListener(OnSetFieldToContract);
        UIController.Instance.FieldsDropdownData();
    }


    public void GetFieldData()
    {
        MainManager.firebase().Child("contract" + currentFieldContract.contractStats.ContractID).Child("Field" + field.StaticFieldID).GetValueAsync().ContinueWith(task =>
           {
               if (task.IsFaulted)
               {
                   // Handle the error...
               }
               else if (task.IsCompleted)
               {
                   DataSnapshot snapshot = task.Result;

                   if (snapshot.GetRawJsonValue().Length > 5)
                   {
                       field = JsonUtility.FromJson<Field>(snapshot.GetRawJsonValue());

                   }


               }
           });
    }

    public void SetAndOpenFieldBlockPlantsForContract()
    {
        currentFieldContract.FieldManager = this;
        GetFieldPlantsData();
    }

    public void OnSetFieldToContract()
    {
       
        field.FieldContractID = currentFieldContract.contractStats.ContractID;
        field.isSet = true;

        string serializedJson = JsonUtility.ToJson(field);
        MainManager.firebase().Child("contract" + currentFieldContract.contractStats.ContractID).Child("Field" + field.StaticFieldID).SetRawJsonValueAsync(serializedJson).ContinueWith(task =>
        {
            if (task.IsFaulted)
            {

                Debug.Log("POST REQUEST FAILED  FOR FIELD " + field.StaticFieldID);
                // Handle the error...
            }
            else if (task.IsCompleted)
            {
                currentFieldContract.CreatePlantsForContract(field.StaticFieldID);
                Debug.Log("POST REQUEST SUCCESS  FOR FIELD " + field.StaticFieldID);


            }
        });


    }

    void GetFieldPlantsData()
    {
        currentFieldContract.RetrieveFieldPlantsData(field.StaticFieldID);
        FieldContractPlants = currentFieldContract.plants;
        FieldsManager.Instance.SetDirtBlocksForField(FieldContractPlants);
    }

    public Contract GetFieldCurrentContract()
    {
        return currentFieldContract;
    }

    private void Update()
    {
        if (field != null)
        {
            if (field.isSet)
            {
                AddFieldButton.interactable = false;
                gameObject.GetComponent<Button>().interactable = true;
            }
            else
            {
                gameObject.GetComponent<Button>().interactable = false;

                AddFieldButton.interactable = true;
            }
        }

    }

    [Serializable]
    public class Field
    {
        public int FieldContractID;
        public bool isSet;
        public int StaticFieldID;
    }


}
