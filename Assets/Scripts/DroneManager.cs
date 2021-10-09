using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneManager : MonoBehaviour
{

    [SerializeField] List<Drone> AllDrones = new List<Drone>();
    public static DroneManager Instance;
    [SerializeField] Contract CurrentContract;
    [SerializeField] FieldsManager fieldManager;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(this.gameObject);
    }


    public int DronesCount()
    {
        return AllDrones.Count;
    }

    public void LinkDronesWithCurrenContract()
    {
        if (fieldManager != null)
        {
            CurrentContract = fieldManager.GetFieldCurrentContract();
            UIController.Instance.OpenDroneUI();
            SetAllDronesDataForField();
        }
    }

    public Contract GetCurrentDroneManagerContract()
    {
        return CurrentContract;
    }


    // Start is called before the first frame update
    void Start()
    {
        FindAllDronesAndSetToList();
        AssignStaticIDsForAllDrones();

    }


    void SetAllDronesDataForField()
    {
        foreach (var item in AllDrones)
        {
            item.droneStats = null;
        }

        for (int i = 0; i < AllDrones.Count; i++)
        {

            AllDrones[i].GetDroneData();
        }
    }

    void FindAllDronesAndSetToList()
    {
        var drones = FindObjectsOfType<Drone>();
        if (drones != null)
            for (int i = 0; i < drones.Length; i++)
            {
                AddDroneToList(drones[i]);
            }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void AddDroneToList(Drone drone)
    {

        if (AllDrones.Contains(drone))
            Debug.LogError("drone already exists in the list " + drone.dropeType);

        else
        {
            AllDrones.Add(drone);
            AssignStaticIDsForAllDrones();
        }
    }

    public void AssignStaticIDsForAllDrones()
    {

        for (int i = 0; i < AllDrones.Count; i++)
        {
            AllDrones[i].DroneStaticID = i;


        }
        AllDrones.Reverse();
    }
}
