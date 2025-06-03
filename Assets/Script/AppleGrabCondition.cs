using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Hands;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class AppleGrabCondition : MonoBehaviour
{
    [Header("Grabbing Settings")]
    private float fingerProximityThreshold = 0.1f; // 4cm radius
    public int requiredFingersToGrab = 4;
    
    private List<Transform> leftHandTips;
    private List<Transform> rightHandTips;
    
    private XRBaseInteractor leftHandInteractor;
    private XRBaseInteractor rightHandInteractor;
    
    private XRBaseInteractor currentHandInteractor;
    public XRInteractionManager interactionManager; // Assign this in inspector or find via script


    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable;
    private GridAppleSpawner parentSpawner;
    private void Awake()
    {
        interactionManager = FindAnyObjectByType<XRInteractionManager>();
        parentSpawner = transform.parent.GetComponent<GridAppleSpawner>();
        grabInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        grabInteractable.selectEntered.AddListener(OnGrabbed);
        
        leftHandTips = parentSpawner.leftHandTips;
        rightHandTips = parentSpawner.rightHandTips;
        
        leftHandInteractor = parentSpawner.leftHandInteractor;
        rightHandInteractor = parentSpawner.rightHandInteractor;

    }

    private void OnEnable()
    {
        grabInteractable.enabled = false;
    }

    private void Update()
    {
        int leftFingersNear = CountFingersNearApple(leftHandTips);
        int rightFingersNear = CountFingersNearApple(rightHandTips);

        if (!grabInteractable.isSelected && (leftFingersNear >= requiredFingersToGrab || rightFingersNear >= requiredFingersToGrab))
        {
            grabInteractable.enabled = true;

            // Try to find a valid interactor (hand near the apple)
            currentHandInteractor = FindNearestHandInteractor(); // Implement this function

            if (currentHandInteractor != null)
            {
                interactionManager.SelectEnter(
                    (IXRSelectInteractor)currentHandInteractor,
                    (IXRSelectInteractable)grabInteractable
                );

                Debug.Log("Grab triggered manually!");
            }
        }

    }
    private XRBaseInteractor FindNearestHandInteractor()
    {
        float leftDist = Vector3.Distance(leftHandInteractor.transform.position, transform.position);
        float rightDist = Vector3.Distance(rightHandInteractor.transform.position, transform.position);

        if (leftDist < rightDist && leftDist < 0.2f)
            return leftHandInteractor;
        if (rightDist < 0.2f)
            return rightHandInteractor;

        return null;
    }


    private int CountFingersNearApple(List<Transform> fingerTips)
    {
        int count = 0;
        foreach (var tip in fingerTips)
        {
            if (tip == null) continue;
            float distance = Vector3.Distance(tip.position, transform.position);
            if (distance <= fingerProximityThreshold)
                count++;
        }
        return count;
    }

    private void OnGrabbed(SelectEnterEventArgs args)
    {
        // Optional: play sound or visual feedback
        Debug.Log("Apple successfully grabbed!");
    }
}