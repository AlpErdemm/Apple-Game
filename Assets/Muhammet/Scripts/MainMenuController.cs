using TMPro;
using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    [Header("Canvases")]
    [SerializeField] private GameObject mainMenuCanvas;
    [SerializeField] private GameObject addPatientModal;

    [Header("InputFields")]
    [SerializeField] private TMP_InputField nameInputField;
    [SerializeField] private TMP_InputField surnameInputField;

    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI errorText;


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
    }

    public void OpenAddPatientModalBtn()
    {
        addPatientModal.SetActive(true);
    }
    public void CloseAddPatientModal()
    {
        addPatientModal.SetActive(false);
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
}
