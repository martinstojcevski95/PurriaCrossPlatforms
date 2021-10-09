using Firebase.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Analytics;
using UnityEngine.EventSystems;
using UnityEngine.Experimental.AI;
using UnityEngine.UI;


public class Contract : MonoBehaviour
{


    public ContractStats contractStats;


   ContractPublicInfo contractPublicInfo;

    public List<Plant> plants = new List<Plant>();

    public GameObject plantprefab;

    public int spawnedPlants;

    public FieldManager FieldManager;

    public Button CreateContractButton;
    public bool isContractDataLoadedFully;
    public ContractGrid contGrid;
    [SerializeField] InputField ContractFieldIDInput;
   // DateTime over = DateTime.Parse("09/28/2021 22:00:39");



   DateTime ContractTime;

    private void Awake()
    {
        var parent = GetComponent<ContractPublicInfo>();
        contractPublicInfo = parent;
    }



    // Start is called before the first frame update
    void Start()
    {
   
        ContractFieldIDInput = UIController.Instance.ContractFieldIDInput;
    }


 
    /// <summary>
    /// Instaitiate plants for started contract
    /// </summary>
    public void InstantiatePlantsForContract()
    {

        while (spawnedPlants < InitialPlantsCount)
        {
            spawnedPlants++;
            GameObject plant = Instantiate(plantprefab, gameObject.transform);
            plant.transform.parent = gameObject.transform;
            plants.Add(plant.GetComponent<Plant>());
            spawnedPlants = plants.Count;
        }
    }


    /// <summary>
    /// Setting grid stats to the linked contract
    /// </summary>
    public void OnGridStatsSetToDB()
    {
        contGrid = ContractController.Instance.Grid;
        if (contGrid != null)
            contGrid.SetGridStats(this);
    }


    /// <summary>
    /// Creating initial contract
    /// </summary>
    public void CreateContract()
    {
        if (ContractController.Instance.isGridsContractChosen)
        {

            contractStats = new ContractStats();
            if (contractStats.isContractStarted == false)
            {
                contractStats.ContractDescription = "test";
                contractStats.ContractID = contractPublicInfo.StaticConttractID;
                contractStats.isContractStarted = true;
                string serializedJson = JsonUtility.ToJson(contractStats);

                 FirebaseReferenceManager.reference.Child("USERS").Child(LogInAndRegister.Instance.UserName).Child("farmdata").Child("contract" + contractStats.ContractID).SetRawJsonValueAsync(serializedJson).ContinueWith(task =>
                {
                    if (task.IsFaulted)
                    {
                        Debug.Log("POST REQUEST FAILED  FOR CONTRACT " + contractStats.ContractID);
                    }
                    else if (task.IsCompleted)
                    {

                        Debug.Log("POST REQUEST SUCCESS  FOR CONTRACT" + contractStats.ContractID);
                        LoadContractData();
                        ContractController.Instance.ContractInteractibleButtons(true);
                        ContractController.Instance.isGridsContractChosen = false;
                    }
                });
                OnGridStatsSetToDB();
            }
        }
        else
            UIController.Instance.DisplayAutomaticLogText(2f,"Go to auction for bidding and select a field and return to create contract");
 
    }



    /// <summary>
    /// Planting plants for the created contract
    /// </summary>
   public void CreatePlantsForContract(int fieldID)
    {
        InstantiatePlantsForContract();
        for (int i = 0; i < plants.Count; i++)
        {
            plants[i].SetInitialPlants(contractStats.ContractID, i,fieldID);
        }
    }


    /// <summary>
    /// Retrieveing field plants growth factors and stats data
    /// </summary>
    /// <param name="fieldid"></param>
    public void RetrieveFieldPlantsData( int fieldid)
    {
        if (contractStats == null)
            return;

        for (int i = 0; i < plants.Count; i++)
        {
            plants[i].GetFieldPlantStatsData(contractStats.ContractID, i, fieldid);
            plants[i].GetFieldPlantsGrwothFactorsData(contractStats.ContractID, i,fieldid);
        }

    }

    /// <summary>
    /// Deleting the contract with pop up confirmation yes/no
    /// </summary>
    public void DeleteContract()
    {
        if (contractStats != null)
        {
            ContractController.Instance.RemoveCreatedContractId(contractStats.ContractID);
            FirebaseReferenceManager.reference.Child("USERS").Child(LogInAndRegister.Instance.UserName).Child("farmdata").Child("contract" + contractStats.ContractID).RemoveValueAsync();
            UIController.Instance.CloseDeleteDialog();
            contractStats = null;
            UIController.Instance.DeleteDialogYesButton.onClick.RemoveAllListeners();
        }
        DeletePlants();   
    }



    /// <summary>
    /// Deleteing all plants for the contract
    /// </summary>
    void DeletePlants()
    {
        for (int i = 0; i < plants.Count; i++)
        {
            plants[i].ClearPlantStats();
        }
    }

    /// <summary>
    /// Passing the deletecontract method to the Yes button in the confirmation dialog
    /// </summary>
    /// <param name="Yes"></param>
    public void OpenDeleteContractDialog()
    {
        UIController.Instance.DeleteDialogYesButton.onClick.AddListener(() => DeleteContract());
        UIController.Instance.DeleteDialog(true, "Do you want to delete the contract?");
    }

  

    /// <summary>
    /// Loads data  from db for each contract after the log in 
    /// </summary>
    public void LoadContractData()
    {

        FirebaseDatabase.DefaultInstance
           .GetReference("USERS").Child(LogInAndRegister.Instance.UserName).Child("farmdata").Child("contract" + contractPublicInfo.StaticConttractID)
           .GetValueAsync().ContinueWith(task => OnContractResponseReceived(task.Result,task));
      //{
      //if (task.IsFaulted)
      //{
      //    Debug.Log("GET REQUEST FAILED FOR CONTRACT " + contractPublicInfo.StaticConttractID);

      //}
      //else if (task.IsCompleted)
      //{
      //    DataSnapshot snapshot = task.Result;


      //        if (snapshot.GetRawJsonValue() == "Null")
      //            isDataLoaded = true;
      //        else
      //        {
      //            contractStats = JsonUtility.FromJson<ContractStats>(snapshot.GetRawJsonValue());     
      //            Debug.Log("GET REQUEST SUCCESS FOR CONTRACT " + contractPublicInfo.StaticConttractID + " DATA :  " + snapshot.GetRawJsonValue());
      //            isDataLoaded = true;
      //            UIController.Instance.isLogged = true;
      //            ContractController.Instance.ContractIDS.Add(contractStats.ContractID);
      //        }

      //    }

     // });

    }

    void OnContractResponseReceived(DataSnapshot snap,Task task)
    {
        if (task.IsFaulted)
        {
            Debug.LogError("failed");
            return;
        }

        Debug.Log("conctract data " + snap.GetRawJsonValue());
        contractStats = JsonUtility.FromJson<ContractStats>(snap.GetRawJsonValue());
        UIController.Instance.isLogged = true;
        ContractController.Instance.ContractIDS.Add(contractStats.ContractID);
    }

    /// <summary>
    /// Get the current field manager for the contract
    /// </summary>
    /// <returns></returns>
    public FieldManager GetCurrentContractFieldManager()
    {
        return FieldManager;
    }

    public bool isContractDataLoaded()
    {
        return isDataLoaded;
    }



    bool isDataLoaded;
    int InitialPlantsCount = 30;


    //CLASSES
    [Serializable]
    public class ContractStats
    {
        public int ContractID;
        public bool isContractStarted;
        public string ContractDescription;
        public string ContractTag = "Contract";
        public List<int> FieldsIds = new List<int>();
        public string Time;  
    }


}
