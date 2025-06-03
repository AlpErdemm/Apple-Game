using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Hands;
using UnityEngine.XR.Hands.Gestures;   // XRFingerShapeMath & enums

public class Example : MonoBehaviour
{
    [Header("Curl criteria")]
    [Tooltip("Full-curl value (0-1) that each finger must exceed")]
    [Range(0f, 1f)] public float fullCurlThreshold = 0.95f;

    [Tooltip("How many fingers must be fully curled before we call the hand a fist")]
    [Range(1, 5)] public int requiredFingers = 4;

    [Tooltip("Count the thumb as one of the fingers?")]
    public bool includeThumb = false;

    /* ---------- runtime ---------- */
    XRHandSubsystem _handSubsystem;
    readonly Dictionary<Handedness, bool> _wasHandCurled = new();

    static readonly XRHandFingerID[] _allFingers =
    {
        XRHandFingerID.Thumb,
        XRHandFingerID.Index,
        XRHandFingerID.Middle,
        XRHandFingerID.Ring,
        XRHandFingerID.Little
    };

    void OnEnable()
    {
        // Grab the first running XRHandSubsystem (there is typically only one)
        var list = new List<XRHandSubsystem>();
        SubsystemManager.GetSubsystems(list);
        if (list.Count == 0) { Debug.LogWarning("XRHandSubsystem not found."); enabled = false; return; }

        _handSubsystem = list[0];
        _handSubsystem.updatedHands += OnHandsUpdated;   // low-latency hand callback :contentReference[oaicite:0]{index=0}
    }

    void OnDisable()
    {
        if (_handSubsystem != null)
            _handSubsystem.updatedHands -= OnHandsUpdated;
    }

    /* ---------- callbacks ---------- */
    void OnHandsUpdated(XRHandSubsystem sender,
                        XRHandSubsystem.UpdateSuccessFlags flags,
                        XRHandSubsystem.UpdateType type)
    {
        CheckHand(sender.leftHand);
        CheckHand(sender.rightHand);
    }

    void CheckHand(in XRHand hand)
    {
        if (!hand.isTracked) return;

        int curledFingers = 0;

        foreach (var finger in _allFingers)
        {
            if (!includeThumb && finger == XRHandFingerID.Thumb) continue;

            // Calculate only the FullCurl metric we need
            var shape = XRFingerShapeMath.CalculateFingerShape(
                            hand, finger, XRFingerShapeTypes.FullCurl);      // :contentReference[oaicite:1]{index=1}

            if (shape.TryGetFullCurl(out float curl) && curl >= fullCurlThreshold)  // :contentReference[oaicite:2]{index=2}
                curledFingers++;
        }

        bool isCurledNow = curledFingers >= requiredFingers;
        _wasHandCurled.TryGetValue(hand.handedness, out bool wasCurled);

        // Fire once on the transition into the fist pose
        if (isCurledNow && !wasCurled)
            Debug.Log($"{hand.handedness}-hand fist detected! ({curledFingers} fingers ≥ {fullCurlThreshold:0.00})");

        _wasHandCurled[hand.handedness] = isCurledNow;
    }
}
