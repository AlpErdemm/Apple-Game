using System.Collections.Generic;
using UnityEngine;

public static class MuhammetDataBase
{
    public static List<Patient> patients = new List<Patient>();

    public static void CreateRandomPatients(int randomCount)
    {
        patients.Clear(); // Listeyi temizle

        string[] maleNames = { "John", "Michael", "David", "James", "Robert", "Daniel", "William", "Thomas" };
        string[] femaleNames = { "Emily", "Sarah", "Jessica", "Ashley", "Amanda", "Emma", "Olivia", "Sophia" };
        string[] surnames = { "Smith", "Johnson", "Brown", "Taylor", "Anderson", "Thomas", "Jackson", "White" };

        for (int i = 0; i < randomCount; i++)
        {
            bool isMale = Random.Range(0, 2) == 0;
            string firstName = isMale
                ? maleNames[Random.Range(0, maleNames.Length)]
                : femaleNames[Random.Range(0, femaleNames.Length)];

            string lastName = surnames[Random.Range(0, surnames.Length)];

            Patient newPatient = new Patient
            {
                patientId = System.Guid.NewGuid().ToString(),
                patientName = firstName + " " + lastName,
                isMale = isMale
            };

            patients.Add(newPatient);
        }

        Debug.Log(randomCount + " random patients created.");
    }

}
