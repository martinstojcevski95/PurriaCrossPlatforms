using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    public RectTransform RegisterUI, LoginUI, LogAndRegButtons, DashboardUI, ContractsUI, ContractStatsUI, FieldUI, FieldStatsUI, DronesUI, DroneStatsUI, WeatherUI;
    public Canvas FullDashboard, FullRegLogUI;
    public Text CurrentTime;
    public Text LoginText;
    public static UIManager Instance;
    public Text DroneID, DroneActivePlants, DroneContractID, Tultips;
    public Text ActiveContracts, AvailableDrones, PlantedPlants, WeatherInfo;
    public Text ActiceCycle0, ActiceCycle1, ActiceCycle2, ActiceCycle3, DoneCycle0, DoneCycle1, DoneCycle2, DoneCycle3;
    public Text LOG;
    public Canvas LOGCanvas;
    public Canvas CyclesUI;

    public List<Text> ActiveCycleContractText, DoneCycleContractText;

    
    // Start is called before the first frame update

    void Awake()
    {
        Instance = this;
    }


    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //CurrentTime.text = "Date - Time " + DateTime.Now;
    }


    public void OpenRegister()
    {
        LogAndRegButtons.DOAnchorPos(new Vector2(-800, 0), 0.4f);
        RegisterUI.DOAnchorPos(new Vector2(0, 7.8f), 0.4f);
    }

    public void OpenLogin()
    {
        LogAndRegButtons.DOAnchorPos(new Vector2(800, 0), 0.4f);
        LoginUI.DOAnchorPos(new Vector2(0, 7.8f), 0.4f);
    }

    public void FromRegisterToLogin()
    {
        LogAndRegButtons.DOAnchorPos(new Vector2(-800, 0), 0.4f);
        RegisterUI.DOAnchorPos(new Vector2(-800, 7.8f), 0.4f);
        LoginUI.DOAnchorPos(new Vector2(0, 7.8f), 0.4f);
    }

    public void CloseRegister()
    {
        LogAndRegButtons.DOAnchorPos(new Vector2(0, 0), 0.4f);
        RegisterUI.DOAnchorPos(new Vector2(-800, 7.8f), 0.4f);
    }

    public void CloseLogin()
    {
        LogAndRegButtons.DOAnchorPos(new Vector2(0, 0), 0.4f);
        LoginUI.DOAnchorPos(new Vector2(800, 7.8f), 0.4f);
    }

    public void CloseLogRegUI()
    {
        StartCoroutine(WaitForLogIn());

    }

    public void FakeLogScreen()
    {
        StartCoroutine(WaitForFakeLogIn());

    }


    public void OpenContractsUI()
    {
        ContractsUI.DOAnchorPos(new Vector2(0, 0), 0.4f);
    }
    public void CloseContractUI()
    {
        ContractsUI.DOAnchorPos(new Vector2(0, -1400), 0.4f);
    }

    public void OpenContractStats()
    {
        ContractStatsUI.DOAnchorPos(new Vector2(0, 0), 0.4f);

    }

    public void CloseContractStats()
    {
        ContractStatsUI.DOAnchorPos(new Vector2(940, 0), 0.4f);
    }

    public void OpenDroneUI()
    {

        DronesUI.DOAnchorPos(new Vector2(0, 0), 0.4f);

    }
    public void CloseDronetUI()
    {
        DronesUI.DOAnchorPos(new Vector2(0, -1400), 0.4f);
    }

    public void OpenDroneStats()
    {
        DroneStatsUI.DOAnchorPos(new Vector2(0, 0), 0.4f);
    }

    public void CloseDroneStats()
    {
        DroneStatsUI.DOAnchorPos(new Vector2(940, 0), 0.4f);
    }
    public void OpenFieldUI()
    {
        FieldUI.DOAnchorPos(new Vector2(0, 0), 0.4f);
    }
    public void CloseFieldUI()
    {
        FieldUI.DOAnchorPos(new Vector2(0, -1400), 0.4f);
    }

    public void OpenFieldStats()
    {
        FieldStatsUI.DOAnchorPos(new Vector2(0, 0), 0.4f);
    }

    public void CloseFieldStats()
    {
        FieldStatsUI.DOAnchorPos(new Vector2(940, 0), 0.4f);
    }

    public void OpenWeatherUI()
    {
        WeatherUI.DOAnchorPos(new Vector2(0, 0), 0.4f);
    }
    public void CloseWeatherUI()
    {
        WeatherUI.DOAnchorPos(new Vector2(0, -1400), 0.4f);
    }
    IEnumerator WaitForLogIn()
    {
        FullRegLogUI.enabled = false;
        FullDashboard.enabled = true;
        LoginText.text = "login in please wait ...";
        yield return new WaitForSeconds(0f);
       
    }


    IEnumerator WaitForFakeLogIn()
    {
        FullRegLogUI.enabled = false;
        FullDashboard.enabled = true;
        LoginText.text = "getting data please wait ...";
        yield return new WaitForSeconds(2f);
        LoginText.text = "";
        DashboardUI.DOAnchorPos(new Vector3(0, -149), 0.4f);
        //  ContractManager.ContractManagerInstance.GetDataForContractsAfterLogIn();
        // ContractManager.ContractManagerInstance.ReloadDataAfterFakeSignOut();
    }

    public void LogUIInfo(float waitSeconds, string LOGInfo)
    {
        StartCoroutine(LogCoroutine(waitSeconds,LOGInfo));
    }

    IEnumerator LogCoroutine(float waitSeconds,string LOGInfo)
    {
        LOGCanvas.enabled = true;
        LOG.text = LOGInfo;
        yield return new WaitForSeconds(waitSeconds);
        LOG.text = "";
        LOGCanvas.enabled = false;
    }
}
