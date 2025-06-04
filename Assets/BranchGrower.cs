using UnityEngine;

public class BranchGrower : MonoBehaviour
{
    public GameObject branchPrefab;
    public Transform tree;       // assign in inspector
    public Transform apple;      // assign in inspector
    public float growSpeed = 2f; // units per second

    private GameObject currentBranch;

    void Start()
    {
        GrowBranch(tree.position, apple.position);
    }

    void GrowBranch(Vector3 start, Vector3 end)
    {
        Vector3 direction = end - start;
        float distance = direction.magnitude;

        // Instantiate branch at tree position
        currentBranch = Instantiate(branchPrefab, start, Quaternion.identity);

        // Make it look at the apple
        currentBranch.transform.rotation = Quaternion.LookRotation(direction);

        // Set initial small scale
        currentBranch.transform.localScale = new Vector3(0.1f, 0.1f, 0.01f);

        // Start coroutine to grow it
        StartCoroutine(GrowOverTime(distance));
    }

    System.Collections.IEnumerator GrowOverTime(float finalLength)
    {
        float currentLength = 0f;

        while (currentLength < finalLength)
        {
            currentLength += growSpeed * Time.deltaTime;

            // Update scale to stretch toward apple
            currentBranch.transform.localScale = new Vector3(0.1f, 0.1f, currentLength);

            yield return null;
        }

        // Snap to exact length
        currentBranch.transform.localScale = new Vector3(0.1f, 0.1f, finalLength);
    }
}