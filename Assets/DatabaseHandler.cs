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

    async public void AddPatientCall(string patientName, string patientSurname)
    {
        await AddPatient(patientName, patientSurname);
    }
    
    async public void AddSessionCall(string patientName, string patientSurname, int mode, int hand, int curl)
    {
        await AddSessionWithNameSurname(patientName, patientSurname, mode, hand, curl);
    }
    
    async public void DeactivateSessionByIdCall(string patientId, string sessionId)
    {
        await DeactivateSessionById(patientId, sessionId);
    }

    // ✅ Hasta ekleme artık `async` oldu ve sonuç bekleniyor
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
            var patientData = new Dictionary<string, object>
            {
                { "name", patientName },
                { "surname", patientSurname },
                { "rom", 0 }
            };

            await db.Collection("patients").AddAsync(patientData);
            Debug.Log("Hasta başarıyla eklendi.");
        }
    }
    
    
    async Task AddSessionToPatient(string patientId, bool isActive, int mode, int hand, int curl)
    {
        var sessionData = new Dictionary<string, object>
        {
            { "active", isActive },
            { "mode", mode },
            { "hand", hand },
            { "curl", curl }
        };

        var result = await db.Collection("patients")
            .Document(patientId)
            .Collection("sessions")
            .AddAsync(sessionData);

        Debug.Log("Seans eklendi. ID: " + result.Id);
        currentPatientId = patientId;
        currentSessionId = result.Id;
    }

    async Task AddSessionWithNameSurname(string patientName, string patientSurname, int mode, int hand, int curl)
    {
        string id = await GetPatientIdByNameSurname(patientName, patientSurname);
        await AddSessionToPatient(id, true, mode, hand, curl);
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


}