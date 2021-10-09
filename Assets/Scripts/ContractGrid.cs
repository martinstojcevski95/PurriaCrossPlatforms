using Firebase.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ContractGrid : MonoBehaviour
{
    [SerializeField]
    Color pickingColor;
    public GridStats gridStats;


    public void SetGrid(int staticGrid)
    {
        if (gridStats != null)
        {
            StaticGridID = staticGrid;
            gridStats.GridID = staticGrid;
        }
    }

    public void RemoveLinkedGridWithContract(int contractID)
    {
        if (gridStats != null)
        {
            if (gridStats.LinkedGridContractID == contractID)
            {
                Debug.Log(gridStats.LinkedGridContractID);
                gridStats = null;
            }
        }
    }

    public void CheckCurrentGridStatus()
    {
        if (gridStats != null)
        {
            if (gridStats.isGridTaken == true)
            {
                Debug.Log("grid used" + StaticGridID);
            }
            else
            {
                Debug.Log("grid not used" + StaticGridID);
                OnCurrentChosenGrid();
            }

        }
        else
        {
            Debug.Log("grid not used" + StaticGridID);
            OnCurrentChosenGrid();
        }

    }


    /// <summary>
    /// Set grid to chosen when clicking on UI grid
    /// </summary>
    void OnCurrentChosenGrid()
    {
        ContractController.Instance.isGridsContractChosen = true;
    }


    /// <summary>
    /// Setting Grid Stats in DB for contract
    /// </summary>
    /// <param name="contract"></param>
    public void SetGridStats(Contract contract)
    {

        gridStats = new GridStats();
        gridStats.isGridTaken = true;
        gridStats.LinkedGridContractID = contract.contractStats.ContractID;
        string serializedJson = JsonUtility.ToJson(gridStats);
        FirebaseReferenceManager.reference.Child("USERS").Child(LogInAndRegister.Instance.UserName).Child("farmdata").Child("contract" + contract.contractStats.ContractID).Child("GridStats").SetRawJsonValueAsync(serializedJson).ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                // Handle the error...
            }
            else if (task.IsCompleted)
            {
                Debug.Log(gridStats);
            }
        });

    }

    /// <summary>
    /// Retrieving the grid data
    /// </summary>
    /// <param name="contrctID"></param>
    public void GetGridData(int contrctID)
    {
        FirebaseDatabase.DefaultInstance.GetReference("USERS").Child(LogInAndRegister.Instance.UserName).Child("farmdata").Child("contract" + contrctID).Child("GridStats")
        .GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                //  Debug.Log("GET REQUEST FAILED FOR CONTRACT " + contractPublicInfo.StaticConttractID);

            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;

                if (snapshot.GetRawJsonValue().Length >= 5)
                    gridStats = JsonUtility.FromJson<GridStats>(snapshot.GetRawJsonValue());

            }
        });

    }



    //PUBLIC VARIABLES
    public int StaticGridID;

    [Serializable]
    public class GridStats
    {
        public bool isGridTaken;
        public int GridID;
        public int LinkedGridContractID;
    }

}
