using System;
using System.ComponentModel;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [Header("Canvases")]
    [SerializeField] private GameObject mainMenuCanvas;
    [SerializeField] private GameObject addPatientModal;
    [SerializeField] private GameObject patientDatailModal;
    [SerializeField] private GameObject gameConfigurationModal;

    [Header("InputFields")]
    [SerializeField] private TMP_InputField nameInputField;
    [SerializeField] private TMP_InputField surnameInputField;
    [SerializeField] private TMP_InputField basketPositionX;
    [SerializeField] private TMP_InputField basketPositionY;
    [SerializeField] private TMP_InputField basketPositionZ;
    [SerializeField] private TMP_InputField applePositionX;
    [SerializeField] private TMP_InputField applePositionY;
    [SerializeField] private TMP_InputField applePositionZ;

    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI errorText;
    [SerializeField] private TextMeshProUGUI patientNameSurnameText;
    [SerializeField] private TextMeshProUGUI patientRangeOfMotionText;

    [Header("Buttons")]
    [SerializeField] private Button startNewGameBtn;
    [SerializeField] private Button endGameBtn;
    [SerializeField] private Button calibrationOpenBtn;
    [SerializeField] private Button calibrationCloseBtn;
    [SerializeField] private Button handSelectLeftBtn;
    [SerializeField] private Button handSelectRightBtn;


    private Vector3 basketPosition;
    private Vector3 applePosition;

    private bool isSelectedRightHand;
    private bool isCalibrationOpen;

    private PatientDashBoardCreator patientDashBoard;
    private void Start()
    {
        errorText.text = string.Empty;
        patientDashBoard = GetComponent<PatientDashBoardCreator>();
        DisableAllScene();
        mainMenuCanvas.SetActive(true);

    }

    public void DisableAllScene()
    {
        mainMenuCanvas.SetActive(false);
        addPatientModal.SetActive(false);
        patientDatailModal.SetActive(false);
        gameConfigurationModal.SetActive(false);
    }

    public void OpenAddPatientModalBtn()
    {
        addPatientModal.SetActive(true);
    }
    public void CloseAddPatientModal()
    {
        addPatientModal.SetActive(false);
    }
    public void ClosePatientDetailsModal()
    {
        patientDatailModal.SetActive(false);


    }
    public void CloseGameConfigurationModal()
    {
        gameConfigurationModal.SetActive(false);
    }

    public void OpenGameCanfigurationModal()
    {
        gameConfigurationModal.SetActive(true);

    }

    private void ResetGameConfigurationMenu()
    {
        isSelectedRightHand = false;
        isCalibrationOpen = false;
        basketPositionX.text = "0";
        basketPositionY.text = "0";
        basketPositionZ.text = "0";
        applePositionX.text = "0";
        applePositionY.text = "0";
        applePositionZ.text = "0";

        calibrationOpenBtn.GetComponent<Image>().color = Color.red;
        calibrationCloseBtn.GetComponent<Image>().color = Color.green;
        handSelectLeftBtn.GetComponent<Image>().color = Color.green;
        handSelectRightBtn.GetComponent<Image>().color = Color.red;
    }


    public void StartGameBtn()
    {
        applePosition = new Vector3(float.Parse(applePositionX.text), float.Parse(applePositionY.text), float.Parse(applePositionZ.text));
        basketPosition = new Vector3(float.Parse(basketPositionX.text), float.Parse(basketPositionY.text), float.Parse(basketPositionZ.text));

        Debug.Log($"Game Starting !!\n" +
            $"Calibration = {isCalibrationOpen}\n" +
            $"SelectedRightHand = {isSelectedRightHand}\n" +
            $"Basket Position = {basketPosition}\n" +
            $"Apple Position{applePosition}");
    }

    public void OpenCalibrationBtn()
    {
        isCalibrationOpen = true;
        calibrationOpenBtn.GetComponent<Image>().color = Color.green;
        calibrationCloseBtn.GetComponent<Image>().color = Color.red;
    }
    public void CloseCalibrationBtn()
    {
        isCalibrationOpen = false;
        calibrationOpenBtn.GetComponent<Image>().color = Color.red;
        calibrationCloseBtn.GetComponent<Image>().color = Color.green;
    }

    public void SelectRightHand()
    {
        isSelectedRightHand = true;
        handSelectLeftBtn.GetComponent<Image>().color = Color.red;
        handSelectRightBtn.GetComponent<Image>().color = Color.green;
    }
    public void SelectLeftHand()
    {
        isSelectedRightHand = false;
        handSelectLeftBtn.GetComponent<Image>().color = Color.green;
        handSelectRightBtn.GetComponent<Image>().color = Color.red;
    }



    public void AddPatientBtn()
    {
        if (string.IsNullOrWhiteSpace(nameInputField.text) || string.IsNullOrWhiteSpace(surnameInputField.text))
        {
            errorText.text = "Ad veya soyad bos olamaz !";
            return;
        }
        Patient patient = new Patient();
        patient.patientId = System.Guid.NewGuid().ToString();
        patient.patientName = nameInputField.text + " " + surnameInputField.text;
        MuhammetDataBase.patients.Add(patient);
        Debug.Log("New Patient Added To Data Base");
        CloseAddPatientModal();
        patientDashBoard.AddNewPatientToDashBoard(patient);
        nameInputField.text = string.Empty;
        surnameInputField.text = string.Empty;
        errorText.text = string.Empty;
    }


    public void OnPatientButtonClicked(Patient patient)
    {
        Debug.Log("Clicked on: " + patient.patientName);
        ResetGameConfigurationMenu();
        
        patientNameSurnameText.text = patient.patientName;
        if (patient.rangeOfMotion == 0)
        {
            patientRangeOfMotionText.text = "Hareket Mesafesi : NULL !";
        }
        else
        {
            patientRangeOfMotionText.text = "Hareket Mesafesi : " + patient.rangeOfMotion.ToString();
        }

        if (patient.isInGame)
        {
            startNewGameBtn.interactable = false;
            endGameBtn.interactable = true;
        }
        else
        {
            startNewGameBtn.interactable = true;
            endGameBtn.interactable = false;
        }
        patientDatailModal.SetActive(true);

    }
}
