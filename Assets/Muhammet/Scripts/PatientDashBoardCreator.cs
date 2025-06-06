using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PatientDashBoardCreator : MonoBehaviour
{
    public GameObject patientButtonPrefab; // Inspector'dan atanacak prefab
    public Transform contentParent; // Scroll View > Viewport > Content nesnesi
    private MainMenuController mainMenuController;

    void Start()
    {
        mainMenuController = GetComponent<MainMenuController>();
        MuhammetDataBase.CreateRandomPatients(10);

        LoadAllPatientsToDashBoard();
    }

    public void LoadAllPatientsToDashBoard()
    {
        foreach (Patient patient in MuhammetDataBase.patients)
        {
            GameObject newButton = Instantiate(patientButtonPrefab, contentParent);

            // Butonun üstündeki yazý
            TextMeshProUGUI buttonText = newButton.GetComponentInChildren<TextMeshProUGUI>();
            buttonText.text = patient.patientName;

            // Renk Ayarlama
            Image background = newButton.GetComponent<Image>();
            if (background != null)
            {
                background.color = patient.isInGame ? Color.cyan : Color.red;
            }

            // Ekstra olarak týklama iþlevi eklenecekse:
            Button btn = newButton.GetComponent<Button>();
            btn.onClick.AddListener(() => mainMenuController.OnPatientButtonClicked(patient));
        }
    }
    public void AddNewPatientToDashBoard(Patient patient)
    {
        GameObject newButton = Instantiate(patientButtonPrefab, contentParent);
        TextMeshProUGUI buttonText = newButton.GetComponentInChildren<TextMeshProUGUI>();
        buttonText.text = patient.patientName;

        Image background = newButton.GetComponent<Image>();
        if (background != null)
        {
            background.color = patient.isInGame ? Color.cyan : Color.red;
        }
        // Ekstra olarak týklama iþlevi eklenecekse:
        Button btn = newButton.GetComponent<Button>();
        btn.onClick.AddListener(() => mainMenuController.OnPatientButtonClicked(patient));

        Debug.Log("New Patient Added To Dash Board");
    }
}
