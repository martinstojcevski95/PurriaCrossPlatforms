using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using Purria;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
        gameObject.name = dropeType.ToString();

        FindDroneManager();
        var droneBtn = this.GetComponent<Button>();
        if (droneBtn != null)
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
        if (droneStats != null)
        {
            if (droneStats.isDroneAssigned)
            {
                var currentDroneContract = droneManager.GetCurrentDroneManagerContract();

                droneStats = null;
                UIController.Instance.CloseDeleteDialog();
                UIController.Instance.DeleteDialogYesButton.onClick.RemoveAllListeners();

                MainManager.firebase().Child("contract" + currentDroneContract.contractStats.ContractID).Child("Field" + currentDroneContract.GetCurrentContractFieldManager().field.StaticFieldID).Child("Drone" + DroneStaticID).RemoveValueAsync();//.ContinueWith(task =>

            }
            else
                MainManager.OnUIInfoOpen("Drone not assigned", 7f);

        }
    }

    public void AssignDroneToField()
    {
        var currentDroneContract = droneManager.GetCurrentDroneManagerContract();


        if (currentDroneContract == null)
        {
            MainManager.OnUIInfoOpen("Please first select a field, to assign drone", 3f);
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



        MainManager.firebase().Child("contract" + currentDroneContract.contractStats.ContractID).Child("Field" + currentDroneContract.GetCurrentContractFieldManager().field.StaticFieldID).Child("Drone" + droneStats.DroneID).SetRawJsonValueAsync(serializedJson).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                MainManager.OnUIInfoOpen("Drone assigned successfully to  contract " + droneStats.DroneContractID + " and field " + droneStats.DroneFieldID, 3f);

            }
        });

    }

    public void GetDroneData()
    {

        var currentDroneContract = droneManager.GetCurrentDroneManagerContract();

        MainManager.firebase().Child("contract" + currentDroneContract.contractStats.ContractID).Child("Field" + currentDroneContract.GetCurrentContractFieldManager().field.StaticFieldID).Child("Drone" + DroneStaticID)
                  .GetValueAsync().ContinueWithOnMainThread(task => OnDroneDataReceived(task.Result));

    }

    void OnDroneDataReceived(DataSnapshot result)
    {
        droneStats = JsonUtility.FromJson<DroneStats>(result.GetRawJsonValue());
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

    public void OnDroneAssignDateChange()
    {
        if (droneStats == null) { MainManager.OnUIInfoOpen("Drone is not assigned to any contract!", 3f); return; }
        var currentDroneContract = droneManager.GetCurrentDroneManagerContract();


        Dictionary<string, object> droneParameters = new Dictionary<string, object>();

        droneParameters.Add("LastVisitedDroneTime", currentDroneContract.plants[0].plantStats.LastVisitedDroneTime = DateManager.dateManager.DateTimeToTicks());

        for (int i = 0; i < currentDroneContract.plants.Count; i++)
        {

            MainManager.firebase().Child("contract" + droneStats.DroneContractID).Child("Field" + droneStats.DroneFieldID).Child("stats").Child("plant" + i).UpdateChildrenAsync(droneParameters);

        }
        MainManager.OnUIInfoOpen("Plant drone working hours updated!", 3f);
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
