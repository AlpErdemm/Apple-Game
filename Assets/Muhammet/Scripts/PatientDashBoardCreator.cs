using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PatientDashBoardCreator : MonoBehaviour
{
    public GameObject patientButtonPrefab; // Inspector'dan atanacak prefab
    public Transform contentParent; // Scroll View > Viewport > Content nesnesi


    void Start()
    {
        MuhammetDataBase.CreateRandomPatients(10);

        LoadAllPatientsToDashBoard();
    }

    public void LoadAllPatientsToDashBoard()
    {
        foreach (Patient patient in MuhammetDataBase.patients)
        {
            GameObject newButton = Instantiate(patientButtonPrefab, contentParent);
            TextMeshProUGUI buttonText = newButton.GetComponentInChildren<TextMeshProUGUI>();
            buttonText.text = patient.patientName;

            // Ekstra olarak týklama iþlevi eklenecekse:
            Button btn = newButton.GetComponent<Button>();
            btn.onClick.AddListener(() => OnPatientButtonClicked(patient));
        }
    }
    public void AddNewPatientToDashBoard(Patient patient)
    {
        GameObject newButton = Instantiate(patientButtonPrefab, contentParent);
        TextMeshProUGUI buttonText = newButton.GetComponentInChildren<TextMeshProUGUI>();
        buttonText.text = patient.patientName;

        // Ekstra olarak týklama iþlevi eklenecekse:
        Button btn = newButton.GetComponent<Button>();
        btn.onClick.AddListener(() => OnPatientButtonClicked(patient));

        Debug.Log("New Patient Added To Dash Board");
    }
    void OnPatientButtonClicked(Patient patient)
    {
        Debug.Log("Clicked on: " + patient.patientName);
        // Daha fazla bilgi gösterme ekraný vs. burada açýlabilir.
    }
}
