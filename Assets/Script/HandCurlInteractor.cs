using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Hands;
using UnityEngine.XR.Hands.Gestures;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Interactors.Visuals;

/// <summary>
/// Interactor that turns a whole-hand curl ≥ threshold into a Select action,
/// so you can grab XRGrabInteractables without pressing any buttons.
/// </summary>
[RequireComponent(typeof(XRInteractorLineVisual))]    // keep inspector happy
public class HandCurlInteractor : XRDirectInteractor
{
    [Header("Curl Settings")]
    [Range(0f, 1f)]
    public float curlThreshold = 0.5f;

    [Tooltip("Thumb counts toward the curled-finger tally?")]
    public bool includeThumb = false;

    [Tooltip("How many fingers must exceed the threshold for a grab")]
    [Range(1,5)]
    public int requiredFingers = 4;

    /* ───────── internal state ───────── */
    XRHandSubsystem _handSubsystem;
    bool            _isCurlActive;

    protected override void OnEnable()
    {
        base.OnEnable();

        // find a running XRHandSubsystem once
        var subs = new List<XRHandSubsystem>();
        SubsystemManager.GetSubsystems(subs);
        if (subs.Count == 0)
        {
            Debug.LogWarning("No XRHandSubsystem found – enable XR Hands in Project Settings.");
            enabled = false;
            return;
        }
        _handSubsystem = subs[0];
        _handSubsystem.updatedHands += OnHandsUpdated;
    }

    protected override void OnDisable()
    {
        if (_handSubsystem != null)
            _handSubsystem.updatedHands -= OnHandsUpdated;

        base.OnDisable();
    }
/* ---------- interactor plumbing ---------- */
// XR Interaction Toolkit checks this read-only PROPERTY each frame.
// Returning true tells XRI “the grab button is down”.
    public override bool isSelectActive => _isCurlActive;

    /* ---------- hand-update callback ---------- */
    void OnHandsUpdated(XRHandSubsystem _, XRHandSubsystem.UpdateSuccessFlags __, XRHandSubsystem.UpdateType ___)
    {
        var hand = handedness == InteractorHandedness.Left ? _handSubsystem.leftHand
                                                  : _handSubsystem.rightHand;
        if (!hand.isTracked)
        {
            _isCurlActive = false;
            return;
        }

        int curled = 0;
        foreach (var finger in FingersArray)
        {
            if (!includeThumb && finger == XRHandFingerID.Thumb) continue;

            var shape = XRFingerShapeMath.CalculateFingerShape(
                            hand, finger, XRFingerShapeTypes.FullCurl);

            if (shape.TryGetFullCurl(out float curl) && curl >= curlThreshold)
                ++curled;
        }
        _isCurlActive = curled >= requiredFingers;
    }

    /* ---------- helpers ---------- */
    static readonly XRHandFingerID[] FingersArray =
    {
        XRHandFingerID.Thumb, XRHandFingerID.Index, XRHandFingerID.Middle,
        XRHandFingerID.Ring,  XRHandFingerID.Little
    };
}
