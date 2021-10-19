using Firebase.Database;
using Firebase.Extensions;
using Purria;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class ContractGrid : MonoBehaviour
{
    [SerializeField]
    Color pickingColor;
    public GridStats gridStats;

    List<string> GridPlaces = new List<string>();

    

    void SetGridPlaces()
    {
        GridPlaces.Add("Valley");
        GridPlaces.Add("Mountains");
        GridPlaces.Add("Forest");
        GridPlaces.Add("Mountains");
    }

    private void OnEnable()
    {
        SetGridPlaces();
    }

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

    string randomPlace()
    {
        string place = GridPlaces[UnityEngine.Random.Range(0, GridPlaces.Count - 1)];

        return place;
    }

    public void CheckCurrentGridStatus()
    {
        if (gridStats != null)
        {
            if (gridStats.isGridTaken == true)
            {
                MainManager.OnUIInfoOpen("Place used ", 1.5f);
               
            }
            else
            {
                MainManager.OnUIInfoOpen("Your field place is " + randomPlace() + " go back and create contract ", 1.5f);
                OnCurrentChosenGrid();
            }

        }
        else
        {
            MainManager.OnUIInfoOpen("Your field place is " + randomPlace() + " go back and create contract ", 1.5f);
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
        MainManager.firebase().Child("contract" + contract.contractStats.ContractID).Child("GridStats").SetRawJsonValueAsync(serializedJson).ContinueWith(task => { });

    }

    /// <summary>
    /// Retrieving the grid data
    /// </summary>
    /// <param name="contrctID"></param>
    public void GetGridData(int contrctID)
    {
        MainManager.firebase().Child("contract" + contrctID).Child("GridStats")
        .GetValueAsync().ContinueWithOnMainThread(task =>
        {
            OnContractGridResponseReceived(task.Result, task);
        });

    }
    void OnContractGridResponseReceived(DataSnapshot snap, Task task)
    {
        if (task.IsFaulted)
        {
            Debug.LogError("failed");
            return;
        }

        gridStats = JsonUtility.FromJson<GridStats>(snap.GetRawJsonValue());
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
