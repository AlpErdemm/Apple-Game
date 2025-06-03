using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Hands;
using UnityEngine.XR.Hands.Gestures;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

/// <summary>
/// While <see cref="interactor"/> is grabbing, freezes every joint whose
/// finger’s Full-Curl exceeds <see cref="clampCurl"/> so fingers stop
/// exactly at the apple surface.
/// </summary>
[RequireComponent(typeof(XRHandSkeletonDriver))]
public class FingerCurlLimiter : MonoBehaviour
{
    [Tooltip("Reference to the curl-driven interactor on this hand")]
    public HandCurlInteractor interactor;

    [Range(0f, 1f)]
    [Tooltip("Maximum curl allowed while grabbing")]
    public float clampCurl = 0.5f;

    XRHandSubsystem        _hands;
    XRHandSkeletonDriver   _skeleton;
    readonly Dictionary<XRHandJointID, Quaternion> _frozen = new();
    readonly Dictionary<XRHandJointID, Transform>  _jointMap = new();

    static readonly XRHandFingerID[] k_Fingers =
    {
        XRHandFingerID.Thumb, XRHandFingerID.Index, XRHandFingerID.Middle,
        XRHandFingerID.Ring,  XRHandFingerID.Little
    };

    void Awake()
    {
        _skeleton = GetComponent<XRHandSkeletonDriver>();

        // Build quick lookup: jointID → Transform
        foreach (var jtr in _skeleton.jointTransformReferences)
            if (jtr.jointTransform != null)
                _jointMap[jtr.xrHandJointID] = jtr.jointTransform;
    }

    void OnEnable()
    {
        var subs = new List<XRHandSubsystem>();
        SubsystemManager.GetSubsystems(subs);
        if (subs.Count == 0)
        {
            Debug.LogError("XRHandSubsystem not found.");
            enabled = false;
            return;
        }
        _hands = subs[0];
    }

    void LateUpdate()   // after skeleton driver has posed the hand
    {
        // Not grabbing → let fingers move
        if (interactor == null || !interactor.isSelectActive)
        {
            _frozen.Clear();
            return;
        }

        XRHand hand = interactor.handedness == InteractorHandedness.Left
                        ? _hands.leftHand : _hands.rightHand;
        if (!hand.isTracked) { _frozen.Clear(); return; }

        // 1) Identify fingers that exceed the clamp
        var clampList = new List<XRHandFingerID>();
        foreach (var finger in k_Fingers)
        {
            var shape = XRFingerShapeMath.CalculateFingerShape(
                            hand, finger, XRFingerShapeTypes.FullCurl);          // :contentReference[oaicite:2]{index=2}
            if (shape.TryGetFullCurl(out float v) && v > clampCurl)              // :contentReference[oaicite:3]{index=3}
                clampList.Add(finger);
        }
        if (clampList.Count == 0) { _frozen.Clear(); return; }

        // 2) Freeze every joint of those fingers
        foreach (var finger in clampList)
        {
            var first = finger.GetFrontJointID();                                // :contentReference[oaicite:4]{index=4}
            var last  = finger.GetBackJointID();                                 // :contentReference[oaicite:5]{index=5}

            for (int i = first.ToIndex(); i <= last.ToIndex(); ++i)
            {
                var jointId = XRHandJointIDUtility.FromIndex(i);                 // :contentReference[oaicite:6]{index=6}
                if (!_jointMap.TryGetValue(jointId, out Transform jt)) continue;

                if (!_frozen.TryGetValue(jointId, out Quaternion rot))
                {
                    rot = jt.localRotation;     // save first over-limit rotation
                    _frozen[jointId] = rot;
                }
                jt.localRotation = rot;         // clamp!
            }
        }
    }
}
