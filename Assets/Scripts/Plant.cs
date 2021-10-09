using Firebase.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Plant : MonoBehaviour
{

    public PlantStats plantStats;
    public PlantGrowthFactors plantGrowthFactors;
    DateTime lastVisitedDroneTime;



    /// <summary>
    /// Initial Plants setting when creating contract
    /// </summary>
    /// <param name="contid"></param>
    /// <param name="plantid"></param>
    public void SetInitialPlants(int contid, int plantid,int fieldID)
    {
 
        Debug.Log(fieldID);
        plantStats = new PlantStats();
        plantStats.isDroneAssigned = false;
        plantStats.isPlantPlanted = true;
        plantStats.isPlantInContract = true;
        plantStats.FieldID = contid;
        plantStats.ContractID = contid;
        plantStats.PlantID = plantid;
        plantStats.LastVisitedDroneTime = DateManager.dateManager.DateTimeToTicks();

        plantGrowthFactors = new PlantGrowthFactors();
        plantGrowthFactors.ColorofPlant = 10;
        plantGrowthFactors.HeatofPlant = 10;
        plantGrowthFactors.Height = 10;
        plantGrowthFactors.LeavesQuantity = 10;
        plantGrowthFactors.LeavesWidth = 10;
        plantGrowthFactors.SoilCover = 10;
        plantGrowthFactors.Weight = 10;
        plantGrowthFactors.FieldID = fieldID;

        string growthFactorsJson = JsonUtility.ToJson(plantGrowthFactors);
        string statsJson = JsonUtility.ToJson(plantStats);

        FirebaseReferenceManager.reference.Child("USERS").Child(LogInAndRegister.Instance.UserName).Child("farmdata").Child("contract" + contid).Child("Field" + fieldID).Child("stats").Child("plant" + plantid).SetRawJsonValueAsync(statsJson);
        FirebaseReferenceManager.reference.Child("USERS").Child(LogInAndRegister.Instance.UserName).Child("farmdata").Child("contract" + contid).Child("Field" + fieldID).Child("growthfactors").Child("plant" + plantid).SetRawJsonValueAsync(growthFactorsJson);
     
    }


    /// <summary>
    /// RemovePlants Data
    /// </summary>
    public void ClearPlantStats()
    {
        plantStats = null;
        plantGrowthFactors = null; //check if growth factors needs to be reset
    }


    /// <summary>
    /// Get Plants Growth Factors Data
    /// </summary>
    /// <param name="StaticConttractID"></param>
    /// <param name="plantID"></param>
    /// <param name="fieldid"></param>
    public void GetFieldPlantsGrwothFactorsData(int StaticConttractID, int plantID,int fieldid)
    {
        FirebaseDatabase.DefaultInstance
        .GetReference("USERS").Child(LogInAndRegister.Instance.UserName).Child("farmdata").Child("contract" + StaticConttractID).Child("Field"+ fieldid).Child("growthfactors").Child("plant" + plantID)
        .GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                // Handle the error...
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;

                Debug.Log(snapshot.GetRawJsonValue());
                plantGrowthFactors = JsonUtility.FromJson<PlantGrowthFactors>(snapshot.GetRawJsonValue());

                //   StartCoroutine(PlantsGrowthFactorsDataCoroutine(snapshot));
            }
        });
    }


    /// <summary>
    /// Get Plants Stats Data
    /// </summary>
    /// <param name="StaticConttractID"></param>
    /// <param name="plantID"></param>
    /// <param name="fieldid"></param>
    public void GetFieldPlantStatsData(int StaticConttractID, int plantID, int fieldid)
    {
        FirebaseDatabase.DefaultInstance
           .GetReference("USERS").Child(LogInAndRegister.Instance.UserName).Child("farmdata").Child("contract" + StaticConttractID).Child("Field" + fieldid).Child("stats").Child("plant" + plantID)
           .GetValueAsync().ContinueWith(task =>
           {
               if (task.IsFaulted)
               {
                   // Handle the error...
               }
               else if (task.IsCompleted)
               {
                   DataSnapshot snapshot = task.Result;

                   // Debug.Log(snapshot.GetRawJsonValue());

                   plantStats = JsonUtility.FromJson<PlantStats>(snapshot.GetRawJsonValue());
                   lastVisitedDroneTime = DateManager.dateManager.TicksToDateTime(plantStats.LastVisitedDroneTime);// JsonUtility.FromJson<JsonDateTime>(plantStats.LastVisitedDroneTime);
                   Debug.Log(DateManager.dateManager.GetFullTimeDifference(lastVisitedDroneTime));

               }
           });

    }


    int initialPlants = 15;



    [Serializable]
    public class PlantStats
    {
        public bool isDroneAssigned;
        public bool isPlantPlanted;
        public bool isPlantInContract;
        public int PlantID;
        public int FieldID;
        public int ContractID;
        public int GrowthDays;
        public string LastVisitedDroneTime;
        public int Tultip;
        public int SoilMoisture;
        public int SoilDensity;
        public int SoilOrganicMaterial;
        public int Fertilizer;
        public int Weed;
        public int Disease;
        public int Toxicity;
        public int Acidity;
    }

    [Serializable]
    public class PlantGrowthFactors
    {
        public int Height;
        public int LeavesQuantity;
        public int LeavesWidth;
        public int Weight;
        public int HeatofPlant;
        public int ColorofPlant;
        public int SoilCover;
        public int FieldID;
    }

}
