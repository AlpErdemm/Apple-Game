using System;
using System.Collections.Generic;
using System.ComponentModel;
using Michsky.MUIP;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [Header("Canvases")]
    [SerializeField] private GameObject startCanvas;
    [SerializeField] private GameObject mainMenuCanvas;
    [SerializeField] private GameObject addPatientModal;
    [SerializeField] private GameObject patientDatailModal;
    [SerializeField] private GameObject gameConfigurationFirstModal;
    [SerializeField] private GameObject gameConfigurationSecondModal;
    [SerializeField] private GameObject gameInProgressModal;

    [Header("InputFields")]
    [SerializeField] private TMP_InputField nameInputField;
    [SerializeField] private TMP_InputField surnameInputField;
    [SerializeField] private TMP_InputField basketPositionX;
    [SerializeField] private TMP_InputField basketPositionY;
    [SerializeField] private TMP_InputField basketPositionZ;
    [SerializeField] private TMP_InputField applePositionX;
    [SerializeField] private TMP_InputField applePositionY;
    [SerializeField] private TMP_InputField applePositionZ;
    [SerializeField] private HorizontalSelector handSelector;
    [SerializeField] private CustomDropdown modeSelector;
    [SerializeField] private CustomDropdown curlSelector;
    [SerializeField] private HorizontalSelector levelSelector;
    
    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI errorText;
    [SerializeField] private TextMeshProUGUI patientNameSurnameText;

    [Header("Buttons")]
    [SerializeField] private Button startNewGameBtn;
    [SerializeField] private Button endGameBtn;
    [SerializeField] private Button handSelectLeftBtn;
    [SerializeField] private Button handSelectRightBtn;
    [SerializeField] private Button firstLevelBtn;
    [SerializeField] private Button lastLevelBtn;

    [Header("Images")]
    [SerializeField] private Image[] modeImages;

    public DatabaseHandler databaseHandler;

    private Vector3 basketPosition;
    private Vector3 applePosition;

    private int selectedCurlValue = 2;
    private int selectedLevel = 0; 
    private int selectedHand = 0;
    private bool isCalibrationOpen;

    private int activeModeId;
    private PatientDashBoardCreator patientDashBoard;
    private void Start()
    {
        errorText.text = string.Empty;
        patientDashBoard = GetComponent<PatientDashBoardCreator>();
        DisableAllScene();
        mainMenuCanvas.SetActive(true);
        GameMode1();
    }
    

    private void DisableAppleAndBastekInuts()
    {
        basketPositionX.textComponent.GetComponent<TextMeshProUGUI>().color = Color.grey;
        basketPositionY.textComponent.GetComponent<TextMeshProUGUI>().color = Color.grey;
        basketPositionZ.textComponent.GetComponent<TextMeshProUGUI>().color = Color.grey;
        basketPositionX.enabled = false;
        basketPositionY.enabled = false;
        basketPositionZ.enabled = false;

        applePositionX.textComponent.GetComponent<TextMeshProUGUI>().color = Color.grey;
        applePositionY.textComponent.GetComponent<TextMeshProUGUI>().color = Color.grey;
        applePositionZ.textComponent.GetComponent<TextMeshProUGUI>().color = Color.grey;
        applePositionX.enabled = false;
        applePositionY.enabled = false;
        applePositionZ.enabled = false;
    }
    private void EnableAppleAndBastekInuts()
    {
        basketPositionX.textComponent.GetComponent<TextMeshProUGUI>().color = Color.black;
        basketPositionY.textComponent.GetComponent<TextMeshProUGUI>().color = Color.black;
        basketPositionZ.textComponent.GetComponent<TextMeshProUGUI>().color = Color.black;
        basketPositionX.enabled = true;
        basketPositionY.enabled = true;
        basketPositionZ.enabled = true;

        applePositionX.textComponent.GetComponent<TextMeshProUGUI>().color = Color.black;
        applePositionY.textComponent.GetComponent<TextMeshProUGUI>().color = Color.black;
        applePositionZ.textComponent.GetComponent<TextMeshProUGUI>().color = Color.black;
        applePositionX.enabled = true;
        applePositionY.enabled = true;
        applePositionZ.enabled = true;
    }

    private void DisableAllModeImages()
    {
        foreach (Image image in modeImages)
        {
            image.color = Color.red;
        }
    }
    public void HandChanged()
    {
        selectedHand = handSelector.index;
    }
    public void LevelChanged()
    {
        selectedLevel = levelSelector.index;
    }
    public void GameModeChanged()
    {
        activeModeId = modeSelector.index;
    }
    public void GameMode1()
    {
        DisableAllModeImages();
        EnableAppleAndBastekInuts();
        modeImages[0].color = Color.green;
        activeModeId = 1;
    }
    public void GameMode2()
    {
        DisableAllModeImages();
        DisableAppleAndBastekInuts();
        modeImages[1].color = Color.green;
        activeModeId = 2;
    }
    public void GameMode3()
    {
        DisableAllModeImages();
        DisableAppleAndBastekInuts();
        modeImages[2].color = Color.green;
        activeModeId = 3;
    }
    public void GameMode4()
    {
        DisableAllModeImages();
        DisableAppleAndBastekInuts();
        modeImages[3].color = Color.green;
        activeModeId = 4;
    }
    public void GameMode5()
    {
        DisableAllModeImages();
        DisableAppleAndBastekInuts();
        modeImages[4].color = Color.green;
        activeModeId = 5;
    }


    public void DisableAllScene()
    {
        mainMenuCanvas.SetActive(false);
        addPatientModal.SetActive(false);
        patientDatailModal.SetActive(false);
        gameConfigurationFirstModal.SetActive(false);
        gameConfigurationSecondModal.SetActive(false);
    }
    public void OpenPatientList()
    {
        startCanvas.SetActive(false);
        mainMenuCanvas.SetActive(true);
    }

    public void OpenAddPatientModalBtn()
    {
        addPatientModal.SetActive(true);
    }
    public void CloseAddPatientModal()
    {
        addPatientModal.SetActive(false);
    }
    public void OpenPatientDetailsModal()
    {
        mainMenuCanvas.SetActive(false);
        patientDatailModal.SetActive(true);
        gameConfigurationFirstModal.SetActive(false);
    }
    public void ClosePatientDetailsModal()
    {
        patientDatailModal.SetActive(false);
        
    }
    public void CloseGameConfigurationFirstModal()
    {
        gameConfigurationFirstModal.SetActive(false);
    }

    public void OpenGameConfigurationFirstModal()
    {
        patientDatailModal.SetActive(false);
        gameConfigurationFirstModal.SetActive(true);
        gameConfigurationSecondModal.SetActive(false);
    }
    public void CloseGameConfigurationSecondModal()
    {
        gameConfigurationSecondModal.SetActive(false);
    }

    public void OpenGameConfigurationSecondModal()
    {
        gameConfigurationFirstModal.SetActive(false);
        gameConfigurationSecondModal.SetActive(true);
    }

    private void ResetGameConfigurationMenu()
    {
        selectedHand = 0;
        selectedCurlValue = 2;
        isCalibrationOpen = false;
        basketPositionX.text = "0";
        basketPositionY.text = "0";
        basketPositionZ.text = "0";
        applePositionX.text = "0";
        applePositionY.text = "0";
        applePositionZ.text = "0";
        
        handSelectLeftBtn.GetComponent<Image>().color = Color.green;
        handSelectRightBtn.GetComponent<Image>().color = Color.red;
        
    }
    
    public void StartGameBtn()
    {
        gameConfigurationSecondModal.SetActive(false);
        if (activeModeId == 1)
        {
            applePosition = new Vector3(float.Parse(applePositionX.text), float.Parse(applePositionY.text), float.Parse(applePositionZ.text));
            basketPosition = new Vector3(float.Parse(basketPositionX.text), float.Parse(basketPositionY.text), float.Parse(basketPositionZ.text));
            
            Debug.Log($"Game Starting !!\n" +
                $"Calibration = {isCalibrationOpen}\n" +
                $"SelectedRightHand = {selectedHand}\n" +
                $"GameModeId = {activeModeId}\n" +
                $"Basket Position = {basketPosition}\n" +
                $"Apple Position{applePosition}");
        }
        else
        {
            Debug.Log($"Game Starting !!\n" +
                $"Calibration = {isCalibrationOpen}\n" +
                $"SelectedRightHand = {selectedHand}\n" +
                $"GameModeId = {activeModeId}");
        }
        
        databaseHandler.AddSessionCall(activeModeId, selectedHand, selectedCurlValue, selectedLevel);
    }
    public void ContinueBtn()
    {
        gameConfigurationFirstModal.SetActive(false);
        gameConfigurationSecondModal.SetActive(true);
    }

    public void SelectRightHand()
    {
        selectedHand = 0;
        handSelectLeftBtn.GetComponent<Image>().color = Color.red;
        handSelectRightBtn.GetComponent<Image>().color = Color.green;
    }
    public void SelectLeftHand()
    {
        selectedHand = 1;
        handSelectLeftBtn.GetComponent<Image>().color = Color.green;
        handSelectRightBtn.GetComponent<Image>().color = Color.red;
    }
    public void HandCurlChanged()
    {
        selectedCurlValue = curlSelector.index;
    }
    public void SelectFirstLevel()
    {
        selectedLevel = 0;
        firstLevelBtn.GetComponent<Image>().color = Color.green;
        lastLevelBtn.GetComponent<Image>().color = Color.red;
    }
    
    public void SelectLastLevel()
    {
        selectedLevel = 1;
        firstLevelBtn.GetComponent<Image>().color = Color.red;
        lastLevelBtn.GetComponent<Image>().color = Color.green;
    }
    
    public void AddPatientBtn()
    {
        if (string.IsNullOrWhiteSpace(nameInputField.text) || string.IsNullOrWhiteSpace(surnameInputField.text))
        {
            errorText.text = "Ad veya soyad bos olamaz !";
            return;
        }
        
        databaseHandler.AddPatientCall(nameInputField.text, surnameInputField.text);
        Patient patient = new Patient();
        patient.patientId = System.Guid.NewGuid().ToString();
        patient.patientName = nameInputField.text + " " + surnameInputField.text;
        MuhammetDataBase.patients.Add(patient);
        Debug.Log("New Patient Added To Data Base");
        CloseAddPatientModal();
        patientDashBoard.AddNewPatientToDashBoard(databaseHandler.patient);
        nameInputField.text = string.Empty;
        surnameInputField.text = string.Empty;
        errorText.text = string.Empty;
    }


    public void OnPatientButtonClicked(Dictionary<string, object> patientData)
    {
        string name = patientData.TryGetValue("name", out var _name) ? _name.ToString() : "Unknown";
        string surname = patientData.TryGetValue("surname", out var _surname) ? _surname.ToString() : "Unknown";
        ResetGameConfigurationMenu();
        
        databaseHandler.patient = patientData;
        
        patientNameSurnameText.text = name + " " + surname;

        startNewGameBtn.interactable = true;
        endGameBtn.interactable = false;
        patientDatailModal.SetActive(true);
        mainMenuCanvas.SetActive(false);

    }
    public void OnEndGameButtonClicked()
    {
        databaseHandler.DeactivateSessionByIdCall(databaseHandler.currentPatientId, databaseHandler.currentSessionId);
        gameConfigurationFirstModal.SetActive(false);
        patientDatailModal.SetActive(true);
    }
}
