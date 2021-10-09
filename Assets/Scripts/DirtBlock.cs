using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DirtBlock : MonoBehaviour
{
    public Plant blockPlant;

    [SerializeField] Button dirtBlockButton;

    // Start is called before the first frame update
    void Start()
    {

    }

    private void OnPlantClicked()
    {
        Debug.Log("Plant id " + blockPlant.plantStats.PlantID + " from contract id " +  blockPlant.plantStats.ContractID);
    }

    public void SetDirtBlock(Plant plant)
    {
        blockPlant = plant;
        if (blockPlant == null) return;
        dirtBlockButton = GetComponent<Button>();
        dirtBlockButton.onClick.AddListener(OnPlantClicked);
    }




    // Update is called once per frame
    void Update()
    {
        
    }
}
