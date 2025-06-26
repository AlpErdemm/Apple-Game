using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PatientDashBoardCreator : MonoBehaviour
{
    public GameObject patientButtonPrefab; // Inspector'dan atanacak prefab
    public Transform contentParent; // Scroll View > Viewport > Content nesnesi
    private MainMenuController mainMenuController;
    public DatabaseHandler databaseHandler;
    
    async void Start()
    {
        mainMenuController = GetComponent<MainMenuController>();

        // Veritabanından hastaları çek
        var patients = await databaseHandler.GetAllPatients();

        // Çekilen hastaları göster
        await LoadAllPatientsToDashBoard(patients);
    }

    public async Task LoadAllPatientsToDashBoard(List<Dictionary<string, object>> patients)
    {
        foreach (var patientData in patients)
        {
            string name = patientData.ContainsKey("name") ? patientData["name"].ToString() : "Unknown";
            string surname = patientData.ContainsKey("surname") ? patientData["surname"].ToString() : "";
            bool isInGame = patientData.ContainsKey("isInGame") && (bool)patientData["isInGame"];

            // İsim birleştir
            string fullName = $"{name} {surname}";

            GameObject newButton = Instantiate(patientButtonPrefab, contentParent);

            // Buton üzerindeki yazı
            TextMeshProUGUI buttonText = newButton.GetComponentInChildren<TextMeshProUGUI>();
            buttonText.text = fullName;

            // Renk Ayarı
            Image background = newButton.GetComponent<Image>();
            if (background != null)
            {
                background.color = isInGame ? Color.cyan : Color.red;
            }

            // Tıklama işlevi
            Button btn = newButton.GetComponent<Button>();
            btn.onClick.AddListener(() =>
            {
                // Hasta nesnesi oluştur (ID gerekli ise eklenebilir)
                var patient = new Patient
                {
                    patientName = name,
                    patientSurname = surname,
                    isInGame = isInGame
                };
                mainMenuController.OnPatientButtonClicked(patient);
            });
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
        // Ekstra olarak t�klama i�levi eklenecekse:
        Button btn = newButton.GetComponent<Button>();
        btn.onClick.AddListener(() => mainMenuController.OnPatientButtonClicked(patient));

        Debug.Log("New Patient Added To Dash Board");
    }
}
