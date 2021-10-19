using DG.Tweening;
using Purria;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField] ContractController contractController;

    public static UIController Instance;

    [Header("REGISTER AND LOGIN UI SCREENS")]
    public Canvas FullDashboardUI;
    [SerializeField] List<Button> AppMainHeaderButtons = new List<Button>();
    

    public RectTransform FullDashboard;
    public RectTransform ContractsUI;
    public RectTransform ContractsAuctionUI;
    public RectTransform DronesUI;
    public RectTransform FieldUI;
    public RectTransform WeatherUI;
    public Canvas LogInAndRegisterScreen;
    public RectTransform LoginUI, RegisterUI, LogAndRegInitialButtonsScreen;
    public GameObject FieldOnly;
    public RectTransform DashBoardUI;

    [Header("MainHeaderButtons")]
    [SerializeField] Button DroneBtn, ContractBtn, MessagesBtn, FieldBtn, WeatherBtn;


    [Header("CreatingContractUI")]
    public RectTransform ContractLinkedFieldUI;
    public Button CreateContract;
    public InputField ContractFieldIDInput;


    [Header("ContractsFieldsUI")]
    public RectTransform ContractsFieldUI;
    public Dropdown ContractsFieldsDropdown;


    [SerializeField]
    Text currentTime;

    [Header("LOG INFO")]
    public Text LogText,DeleteDialogText;
    [SerializeField]
    public Canvas LogPanel, StaticLogPanel;
    public Canvas DeleteContractDialogConfirmationPanel;
    public Button DeleteDialogYesButton;   
    public Canvas InfoDialogCanvas;

    private void Awake()
    {
        Instance = this;
    }

  

    #region RegisterAndLogin

    public void OpenRegister()
    {
        LogAndRegInitialButtonsScreen.DOAnchorPos(new Vector2(-800, -360), 0.4f);
        RegisterUI.DOAnchorPos(new Vector2(0, 7.8f), 0.4f);
    }

    public void OpenLogin()
    {
        LogAndRegInitialButtonsScreen.DOAnchorPos(new Vector2(800, -360), 0.4f);
        LoginUI.DOAnchorPos(new Vector2(0, 7.8f), 0.4f);
    }

    public void CloseRegister()
    {
        LogAndRegInitialButtonsScreen.DOAnchorPos(new Vector2(0, -360), 0.4f);
        RegisterUI.DOAnchorPos(new Vector2(-800, 7.8f), 0.4f);
    }

    public void CloseLogin()
    {
        LogAndRegInitialButtonsScreen.DOAnchorPos(new Vector2(0, -360), 0.4f);
        LoginUI.DOAnchorPos(new Vector2(800, 7.8f), 0.4f);
    }

    #endregion

    #region FullDashboard

    private void Update()
    {
        currentTime.text = DateTime.Now.ToString();
    }

    public void LoginIn()
    {
        LogPanel.enabled = true;
        LogText.text = "LOADING ...";

    }

    public void OpenAuctionUI()
    {
        ContractsAuctionUI.DOAnchorPos(new Vector2(0, 0), 0.4f);
    }

    public void CloseAuctionUI()
    {
        ContractsAuctionUI.DOAnchorPos(new Vector2(0, -1400), 0.4f);
        contractController.countDownCounter = 5;
        contractController.StartCountDown(false);
        contractController.ResetBidding();
    }

    public void OpenDashBoard()
    {

        LogInAndRegisterScreen.enabled = false;
        LogText.text = "";
        LogPanel.enabled = false;
        FullDashboardUI.enabled = true;
        FullDashboard.GetComponent<RectTransform>().DOAnchorPos(new Vector2(0f, -1253.06f), 0.5f);

    }
    #endregion

    #region ContractUI

    public void OpenContractsUI()
    {

        ContractsUI.DOAnchorPos(new Vector2(0, 0), 0.4f);
        ContractBtn.interactable = false;
        CloseDronetUI();
        CloseFieldUI();
        CloseWeatherUI();

    }

    public void CloseContractUI()
    {
        ContractsUI.DOAnchorPos(new Vector2(0, -1400), 0.4f);
        ContractBtn.interactable = true;
    }

    public void CloseCreateContractUI()
    {

        CreateContract.onClick.RemoveAllListeners();
        ContractLinkedFieldUI.DOAnchorPos(new Vector2(-1400, 0), 0.4f);
    }

    #endregion

    #region DroneUI

    public void OpenDroneUI()
    {
        DronesUI.DOAnchorPos(new Vector2(0, 0), 0.4f);
        DroneBtn.interactable = false;
        CloseContractUI();
        CloseFieldUI();
        CloseWeatherUI();
    }

    public void CloseDronetUI()
    {
        DronesUI.DOAnchorPos(new Vector2(0, -1400), 0.4f);
        DroneBtn.interactable = true;
    }

    #region DroneTeamMenus
    public void OpenDroneTeam(RectTransform droneTeam)
    {
        droneTeam.DOAnchorPos(new Vector2(0, 0), 0.4f);
    }

    public void CloseDroneTeam(RectTransform droneTeam)
    {
        droneTeam.DOAnchorPos(new Vector2(1318, 0), 0.4f);
    }

    #endregion

    #endregion


    #region FieldUI

    public void OpenFieldUI()
    {
        FieldUI.DOAnchorPos(new Vector2(0, 0), 0.4f);
        FieldBtn.interactable = false;
        FieldsDropdownData();
        CloseDronetUI();
        CloseContractUI();
        CloseWeatherUI();
        CloseFieldOnly();
    }

    public void CloseFieldUI()
    {
        FieldUI.DOAnchorPos(new Vector2(0, -2537), 0.4f);
       // CloseFieldOnly();
        FieldBtn.interactable = true;
       // OpenFieldUI();
    }

    public void CloseFieldOnly()
    {
        FieldOnly.GetComponent<RectTransform>().DOAnchorPos(new Vector2(-1251, -107), 0.4f);
        // 1272
    }

    public void OpenContractsFieldsUI()
    {
        ContractsFieldUI.DOAnchorPos(new Vector2(0, 0), 0.4f);
    }

    public void CloseContractsFieldsUI()
    {
        ContractsFieldUI.DOAnchorPos(new Vector2(-1400, 0), 0.4f);
    }



    public void FieldsDropdownData()
    {
        ContractsFieldsDropdown.options.Clear();
        contractController.ContractIDS.Sort();
       // ContractsFieldsDropdown.options.Add(new Dropdown.OptionData() { text = "" });
        foreach (var item in contractController.ContractIDS)
        {

            ContractsFieldsDropdown.options.Add(new Dropdown.OptionData() { text = "Contract " + item.ToString() });
        }
    }

    #endregion

    #region WeatherUI

    public void OpenWeatherUI()
    {
        WeatherUI.DOAnchorPos(new Vector2(0, 0), 0.4f);
        WeatherBtn.interactable = false;
        CloseDronetUI();
        CloseFieldUI();
        CloseContractUI();
    }
    public void CloseWeatherUI()
    {
        WeatherUI.DOAnchorPos(new Vector2(0, -1400), 0.4f);
        WeatherBtn.interactable = true;
    }


    #endregion

    public void DashBoard()
    {
        CloseDronetUI();
        CloseContractUI();
        CloseWeatherUI();
        CloseFieldUI();
        ManiButtonsActiveStatus(true);
    }

    void ManiButtonsActiveStatus(bool isActive)
    {
        AppMainHeaderButtons.ForEach(b => b.interactable = isActive);
    }


    public void CloseDeleteDialog()
    {
        DeleteContractDialogConfirmationPanel.enabled = false;
        DeleteDialogText.text = "";
    }

    public void DeleteDialog(bool isPanelActive,string description)
    {
        DeleteDialogText.text = description;
        if (isPanelActive)
        {
            DeleteContractDialogConfirmationPanel.enabled = isPanelActive;
        }
        else
        {
            DeleteContractDialogConfirmationPanel.enabled = isPanelActive;
        }


    }
}
