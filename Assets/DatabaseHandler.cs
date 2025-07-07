using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase.Firestore;
using Firebase.Extensions;
using UnityEngine;

public class DatabaseHandler : MonoBehaviour
{
    FirebaseFirestore db;
    public string currentSessionId;
    public string currentPatientId;
    public  List<Dictionary<string, object>> Patients;
    public Dictionary<string, object> patient { get; set; }
    private void Awake()
    {
        db = FirebaseFirestore.DefaultInstance;
    }

    async void Start()
    {
        /*db = FirebaseFirestore.DefaultInstance;

        // Yeni Hasta Eklemek
        await AddPatient("Arda", "Turan");
        
        // Yeni oyun eklemek
        await AddSessionWithNameSurname("Arda", "Turan", 1, 1);
        
        // Oyunu sonlandırmak
        await DeactivateSessionById(currentPatientId, currentSessionId);
        
        // Tüm hastaların listesini almak
        var patients = await GetAllPatients();
        foreach (var patient in patients)
        {
            Debug.Log($"Hasta: {patient["name"]} {patient["surname"]}, ROM: {patient["rom"]}, ID: {patient["id"]}");
        }*/
    }
    async public void NextLevelCall()
    {
        await NextLevel();
    }

    async public void AddPatientCall(string patientName, string patientSurname)
    {
        await AddPatient(patientName, patientSurname);
    }
    
    async public void AddSessionCall(int mode, int hand, int curl, int level)
    {
        await AddSessionWithNameSurname(mode, hand, curl,level);
    }
    
    async public void DeactivateSessionByIdCall(string patientId, string sessionId)
    {
        await DeactivateSessionById(patientId, sessionId);
    }

    async Task AddPatient(string patientName, string patientSurname)
    {
        var snapshot = await db.Collection("patients")
            .WhereEqualTo("name", patientName)
            .WhereEqualTo("surname", patientSurname)
            .GetSnapshotAsync();

        if (snapshot.Count > 0)
        {
            Debug.Log($"'{patientName} {patientSurname}' adlı hasta zaten mevcut.");
        }
        else
        {
            var checkpoints = new Dictionary<string, object>
            {
                { "mode1", 1 },
                { "mode2", 1 },
                { "mode3", 1 },
                { "mode4", 1 }
            };

            var patientData = new Dictionary<string, object>
            {
                { "name", patientName },
                { "surname", patientSurname },
                { "rom", 0 },
                { "checkpoints", checkpoints }
            };

            await db.Collection("patients").AddAsync(patientData);
            Debug.Log("Hasta başarıyla eklendi.");
        }
    }

    
    
    async Task AddSessionToPatient(bool isActive, int mode, int hand, int curl, int level)
    {
        string patientId = patient.TryGetValue("id", out var _id) ? _id.ToString() : null;

        if (string.IsNullOrEmpty(patientId))
        {
            Debug.LogError("Hasta ID'si bulunamadı, seans eklenemedi.");
            return;
        }

        int gameLevel = 1; // default

        if (level == 0)
        {
            gameLevel = 1; // start from first level
        }
        else if (level == 1)
        {
            // get patient document
            var patientDoc = await db.Collection("patients").Document(patientId).GetSnapshotAsync();

            if (patientDoc.Exists && patientDoc.TryGetValue("checkpoints", out Dictionary<string, object> checkpoints))
            {
                string modeKey = $"mode{mode}";
                if (checkpoints.TryGetValue(modeKey, out var checkpointLevelObj) && int.TryParse(checkpointLevelObj.ToString(), out int checkpointLevel))
                {
                    gameLevel = checkpointLevel;
                }
                else
                {
                    Debug.LogWarning($"Belirtilen mod için checkpoint bulunamadı, varsayılan olarak 1 kullanılacak: {modeKey}");
                }
            }
            else
            {
                Debug.LogWarning("Hasta dökümanı veya checkpoint verisi yok, varsayılan olarak 1 kullanılacak.");
            }
        }

        var sessionData = new Dictionary<string, object>
        {
            { "active", isActive },
            { "mode", mode },
            { "hand", hand },
            { "curl", curl },
            { "level", gameLevel }
        };

        var result = await db.Collection("patients")
            .Document(patientId)
            .Collection("sessions")
            .AddAsync(sessionData);

        Debug.Log($"Seans eklendi. ID: {result.Id}");
        currentPatientId = patientId;
        currentSessionId = result.Id;
    }


    async Task AddSessionWithNameSurname(int mode, int hand, int curl, int level)
    {
        await AddSessionToPatient( true, mode, hand, curl, level);
    }
    
    // 🔍 Hasta ad ve soyadına göre ID'yi döndürür
    public async Task<string> GetPatientIdByNameSurname(string name, string surname)
    {
        try
        {
            var snapshot = await db.Collection("patients")
                .WhereEqualTo("name", name)
                .WhereEqualTo("surname", surname)
                .GetSnapshotAsync();

            if (snapshot.Count > 0)
            {
                return snapshot[0].Id;
            }
            else
            {
                return null;
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Hasta arama sırasında hata oluştu: " + ex.Message);
            return null;
        }
    }
    // 📴 Verilen sessionId'ye sahip seansı pasif yapar
    public async Task DeactivateSessionById(string patientId, string sessionId)
    {
        Debug.Log("Deactivate Patient ID: " + patientId);
        Debug.Log("Deactivate Session ID: " + sessionId);

        DocumentReference sessionRef = db.Collection("patients")
            .Document(patientId)
            .Collection("sessions")
            .Document(sessionId);

        Dictionary<string, object> updateData = new Dictionary<string, object>
        {
            { "active", false }
        };

        try
        {
            await sessionRef.UpdateAsync(updateData);
            Debug.Log($"Session '{sessionId}' başarıyla pasif hale getirildi.");
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Seans güncellemesi başarısız: " + ex.Message);
        }
    }
    public async Task<List<Dictionary<string, object>>> GetAllPatients()
    {
        List<Dictionary<string, object>> patientList = new List<Dictionary<string, object>>();

        try
        {
            var snapshot = await db.Collection("patients").GetSnapshotAsync();

            foreach (DocumentSnapshot doc in snapshot.Documents)
            {
                Dictionary<string, object> patientData = doc.ToDictionary();
                patientData.Add("id", doc.Id); // Include patient ID in the result
                patientList.Add(patientData);
            }

            Debug.Log($"Toplam {patientList.Count} hasta bulundu.");
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Hasta listesi alınırken hata oluştu: " + ex.Message);
        }

        return patientList;
    }
    
    public async Task NextLevel()
    {
        if (string.IsNullOrEmpty(currentPatientId) || string.IsNullOrEmpty(currentSessionId))
        {
            Debug.LogError("Aktif bir hasta veya seans yok. NextLevel çalıştırılamadı.");
            return;
        }

        var sessionRef = db.Collection("patients")
            .Document(currentPatientId)
            .Collection("sessions")
            .Document(currentSessionId);

        try
        {
            var sessionSnapshot = await sessionRef.GetSnapshotAsync();

            if (sessionSnapshot.Exists)
            {
                int currentLevel = 1;

                if (sessionSnapshot.TryGetValue("level", out int existingLevel))
                {
                    currentLevel = existingLevel;
                }

                int newLevel = Mathf.Min(currentLevel + 1, 5); // max level 5

                await sessionRef.UpdateAsync(new Dictionary<string, object>
                {
                    { "level", newLevel }
                });

                Debug.Log($"Seans seviyesi güncellendi: {currentLevel} → {newLevel}");

                // Optionally, you could do:
                if (newLevel >= 5)
                {
                    Debug.Log("Maksimum seviye ulaşıldı. Butonu devre dışı bırakabilirsiniz.");
                }
            }
            else
            {
                Debug.LogError("Seans bulunamadı.");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("NextLevel sırasında hata oluştu: " + ex.Message);
        }
    }



}