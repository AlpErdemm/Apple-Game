using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class Apple : MonoBehaviour
{
    public static event Action<Apple> PickedCorrectBasket;
    public static event Action<Apple> PickedWrongBasket;
    public AppleType appleType;
    public GridPosition position;
    public bool isGrabbed { get; private set; } = false;
    public bool isReleased =false;
    public bool isCalibrating =false;
    public bool isCalibrationTouched = false;

    private XRGrabInteractable grabInteractable;

    private GridAppleSpawner parentSpawner;

    private void Start()
    {
        parentSpawner = transform.parent.GetComponent<GridAppleSpawner>();
        grabInteractable = GetComponent<XRGrabInteractable>();
        if (isCalibrating)
        {
            Destroy(GetComponent<AppleGrabCondition>());
            Destroy(grabInteractable);
        }

        else
        {
            // Subscribe to XR Interaction events
            grabInteractable.selectEntered.AddListener(OnGrabbed);
            grabInteractable.selectExited.AddListener(OnReleased);
        }
    }

    private void OnDestroy()
    {
        // Clean up the event listeners to avoid memory leaks
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.RemoveListener(OnGrabbed);
            grabInteractable.selectExited.RemoveListener(OnReleased);
        }
    }

    private void OnGrabbed(SelectEnterEventArgs args)
    {
        parentSpawner.Grabbed();
        parentSpawner.grabEffect.FireEffect(transform.position);
        isGrabbed = true;
    }

    private void OnReleased(SelectExitEventArgs args)
    {
        Debug.Log("Hey");
        if (isReleased) return;
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.RemoveListener(OnGrabbed);
            grabInteractable.selectExited.RemoveListener(OnReleased);
        }
        //GetComponent<Rigidbody>().isKinematic = false;
        //GetComponent<Rigidbody>().useGravity = true;
        parentSpawner.OnReleased(transform.position, position, this);
        isGrabbed = false;
        isReleased = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isCalibrationTouched || !isCalibrating) return;
        isCalibrationTouched = true;
        
        Material material = parentSpawner.OnCalibrationTouched(position);
        Renderer renderer = transform.GetChild(0).GetComponent<Renderer>();
        if (renderer != null && material != null)
        {
            // Assign the same material to all slots
            Material[] newMaterials = new Material[renderer.materials.Length];
            for (int i = 0; i < newMaterials.Length; i++)
            {
                newMaterials[i] = material;
            }
            renderer.materials = newMaterials;
        }
    }

    public void Pick(bool isCorrectBasket)
    {
        if (isCorrectBasket) PickedCorrectBasket?.Invoke(this); else PickedWrongBasket?.Invoke(this);
        Destroy(GetComponent<Apple>());
        Destroy(GetComponent<XRGrabInteractable>());
    }
    
}
