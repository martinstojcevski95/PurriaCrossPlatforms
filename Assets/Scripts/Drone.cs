using Firebase.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Drone : MonoBehaviour
{

    public DroneType dropeType;
    public int DroneStaticID;

   public DroneStats droneStats;
    DroneManager droneManager;

    [SerializeField] Text droneAssingment;

    [SerializeField] Text droneTypeName, droneDescription;



    // Start is called before the first frame update
    void Start()
    {
    
        droneTypeName.text = dropeType.ToString();
        droneDescription.text = "Test description";

        FindDroneManager();
        var droneBtn = this.GetComponent<Button>();
        if(droneBtn != null)
            droneBtn.onClick.AddListener(AssignDroneToField);

    }

    void FindDroneManager()
    {
        var dronemanager = (DroneManager)FindObjectOfType(typeof(DroneManager));
        if (dronemanager != null)
        {
            droneManager = dronemanager;
        }
        else
            Debug.LogError("drone manager not found");
    }

    // Update is called once per frame
    void Update()
    {
        DroneUI();
    }

    void DroneUI()
    {
        if (droneStats != null && droneStats.isDroneAssigned)
            droneAssingment.text = "Drone assigned to contract " + droneStats.DroneContractID + " field " + droneStats.DroneFieldID;
        else
            droneAssingment.text = "Not Assigned";
    }

    /// <summary>
    /// Passing the deletecontract method to the Yes button in the confirmation dialog
    /// </summary>
    /// <param name="Yes"></param>
    public void OpenDeleteDroneDialog()
    {
        UIController.Instance.DeleteDialogYesButton.onClick.AddListener(() => RemoveDroneFromField());
        UIController.Instance.DeleteDialog(true, "Do you want to remove the drone " + dropeType.ToString() + " ?");
    }



    public void RemoveDroneFromField()
    {
        if(droneStats != null)
        {
            if (droneStats.isDroneAssigned)
            {
                var currentDroneContract = droneManager.GetCurrentDroneManagerContract();

                droneStats = null;
                UIController.Instance.CloseDeleteDialog();
                UIController.Instance.DeleteDialogYesButton.onClick.RemoveAllListeners();

                FirebaseReferenceManager.reference.Child("USERS").Child(LogInAndRegister.Instance.UserName).Child("farmdata").Child("contract" + currentDroneContract.contractStats.ContractID).Child("Field" + currentDroneContract.GetCurrentContractFieldManager().field.StaticFieldID).Child("Drone" + DroneStaticID).RemoveValueAsync();//.ContinueWith(task =>
                //{
                //    if (task.IsFaulted)
                //    {

                //          Debug.Log("POST REQUEST FAILED  FOR CONTRACT " + contractStats.ContractID);
                //         Handle the error...
                //    }
                //    else if (task.IsCompleted)
                //    {

                //         Debug.Log("POST REQUEST SUCCESS  FOR CONTRACT" + contractStats.ContractID);
                //         LoadContractDataA();

                //        Debug.Log(task.IsCompleted);
                //        droneStats = null;
                //        UIController.Instance.CloseDeleteDialog();
                //        UIController.Instance.DeleteDialogYesButton.onClick.RemoveAllListeners();

                //    }
                //});

            }
            else
                Debug.Log("drone not assigned");
        }
    }

    public void AssignDroneToField()
    {
        var currentDroneContract = droneManager.GetCurrentDroneManagerContract();


        if (currentDroneContract == null)
        {
            UIController.Instance.DisplayAutomaticLogText(2f, "Please first select a field, to assign drone");
            Debug.Log("test");
            return;
        }
  

        droneStats = new DroneStats();
        droneStats.DroneBattery = 100;
        droneStats.isDroneAssigned = true;
        droneStats.DroneContractID = currentDroneContract.contractStats.ContractID;
        droneStats.DroneFieldID = currentDroneContract.GetCurrentContractFieldManager().field.StaticFieldID;
        droneStats.isDroneBusy = true;
        droneStats.DroneID = DroneStaticID;
        droneStats.droneType = dropeType.ToString();

        string serializedJson = JsonUtility.ToJson(droneStats);

        UIController.Instance.DisplayAutomaticLogText(2f, "Drone assigned successfully to  contract " + droneStats.DroneContractID + " and field " + droneStats.DroneFieldID);


        FirebaseReferenceManager.reference.Child("USERS").Child(LogInAndRegister.Instance.UserName).Child("farmdata").Child("contract" + currentDroneContract.contractStats.ContractID).Child("Field" + currentDroneContract.GetCurrentContractFieldManager().field.StaticFieldID).Child("Drone" + droneStats.DroneID).SetRawJsonValueAsync(serializedJson).ContinueWith(task =>
        {
            if (task.IsFaulted)
            {

                //  Debug.Log("POST REQUEST FAILED  FOR CONTRACT " + contractStats.ContractID);
                // Handle the error...
            }
            else if (task.IsCompleted)
            {

                // Debug.Log("POST REQUEST SUCCESS  FOR CONTRACT" + contractStats.ContractID);
                // LoadContractDataA();

                Debug.Log(task.IsCompleted);

            }
        });

    }


    public void GetDroneData()
    {


        var currentDroneContract = droneManager.GetCurrentDroneManagerContract();

        FirebaseDatabase.DefaultInstance
           .GetReference("USERS").Child(LogInAndRegister.Instance.UserName).Child("farmdata").Child("contract" + currentDroneContract.contractStats.ContractID).Child("Field" + currentDroneContract.GetCurrentContractFieldManager().field.StaticFieldID).Child("Drone" + DroneStaticID)
                  .GetValueAsync().ContinueWith(task =>
                  {
                      if (task.IsFaulted)
                      {
                   // Handle the error...
               }
                      else if (task.IsCompleted)
                      {
                          DataSnapshot snapshot = task.Result;

                   droneStats = JsonUtility.FromJson<DroneStats>(snapshot.GetRawJsonValue());

                     
                      }
                  });

        Debug.Log(currentDroneContract.contractStats.ContractID + " field " + currentDroneContract.GetCurrentContractFieldManager().field.StaticFieldID);

    }

    void AssignDroneStaticID()
    {
        DroneStaticID += droneManager.DronesCount();
    }


    [Serializable]
    public class DroneStats
    {
        public int DroneID;
        public int DroneContractID;
        public int DroneFieldID;
        public bool isDroneBusy;
        public float DroneBattery;
        public string droneType;
        public bool isDroneAssigned;
    }

    public enum DroneType
    {
        LadyBugFertilizer,
        AdvardWaterVehicle,
        Spiderwaterdelivery,
        Crabweeder,
        Boarfieldtiller,
        Elephantharvester,
        Beaverplanter,
        Storkmedic


    }

}
