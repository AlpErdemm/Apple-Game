using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using Unity.XR.CoreUtils;
using UnityEngine.UI;


// NOTE: Serialized fields use PascalCase; private fields use _camelCase.

[Serializable]
public struct GridPosition
{
    public Vector3Int Grid;
    public Vector3 WorldPosition;

    public GridPosition(Vector3Int grid, Vector3 worldPosition)
    {
        Grid = grid;
        WorldPosition = worldPosition;
    }
}

public class GridAppleSpawner : MonoBehaviour
{
    [Header("Hand & Interaction")]
    public List<Transform> LeftHandTips = new();
    public List<Transform> RightHandTips = new();
    public UnityEngine.XR.Interaction.Toolkit.Interactors.XRBaseInteractor LeftHandInteractor;
    public UnityEngine.XR.Interaction.Toolkit.Interactors.XRBaseInteractor RightHandInteractor;

    [Header("Prefab & Grid")]
    public GameObject ApplePrefab;
    public int Range;               // e.g. –3…+3
    public float Spacing;          // world units
    public int StartCountdown = 15;

    [Header("Materials & Spawn Odds")]
    public Material HealthyMaterial;
    public Material RottenMaterial;
    public Material TransparentMaterial;
    [Range(0f, 1f)]
    public float RottenChance = 0.3f;

    [Header("Baskets")]
    public XROrigin XrOrigin;
    public GameObject HealthyBasket;
    public Vector3 HealthyBasketOffset;
    public GameObject RottenBasket;
    public Vector3 RottenBasketOffset;

    public GrabEffect GrabEffect;
    public Text BasketLabel;
    public float BasketMoveDuration = 0.5f;

    [Header("Arc Settings")]
    [Range(-180f, 180f)]
    public float ArcRotation = 0f;

    [Space]
    public List<GridPosition> Positions = new();
    public List<GridPosition> CalibratedPositions = new();

    public float LastPickSeconds { get; private set; } = -1f;
    public Vector3Int LastPickGrid { get; private set; }

    private GameObject _currentApple;
    private Vector3Int _currentGrid;
    private float _spawnTime;
    private readonly System.Random _rng = new();

    private void Awake()
    {
        Apple.PickedCorrectBasket += OnApplePicked;
        Apple.PickedWrongBasket   += OnApplePicked;
        Apple.PickedCorrectBasket += ShowCorrectLabel;
        Apple.PickedWrongBasket   += ShowWrongLabel;
    }

    private void OnDestroy()
    {
        Apple.PickedCorrectBasket -= OnApplePicked;
        Apple.PickedWrongBasket   -= OnApplePicked;
        Apple.PickedCorrectBasket -= ShowCorrectLabel;
        Apple.PickedWrongBasket   -= ShowWrongLabel;
    }

    public void OnStartButton()
    {
        PositionBasketsRelativeToPlayer();
        GenerateGridPositions();
        SpawnAllApples();
        StartCoroutine(CalibrationCountdown());
    }

    private IEnumerator CalibrationCountdown()
    {
        yield return new WaitForSecondsRealtime(StartCountdown);
        foreach (Transform child in transform)
            Destroy(child.gameObject);

        SpawnRandomApple();
    }

    public void OnAppleRelease(Vector3 releasePos, Apple apple)
    {
        if (apple == null)
        {
            Debug.LogWarning("OnAppleRelease: apple is null.");
            return;
        }

        if (TryGetReleaseZone(releasePos, apple, out var targetBasket))
            AnimateAppleToBasket(apple, targetBasket);
        else
            HandleDroppedApple(apple);
    }

    private bool TryGetReleaseZone(Vector3 pos, Apple apple, out Transform basket)
    {
        var healthyZone = new Bounds(HealthyBasket.transform.position+Vector3.up*0.25f, new Vector3(0.25f, 0.5f, 0.25f));
        var rottenZone  = new Bounds(RottenBasket.transform.position+Vector3.up*0.25f,  new Vector3(0.25f, 0.5f, 0.25f));

        bool inHealthy = healthyZone.Contains(pos);
        bool inRotten  = rottenZone.Contains(pos);

        basket = inHealthy ? HealthyBasket.transform :
            inRotten  ? RottenBasket.transform : null;

        return basket != null;
    }

    private void AnimateAppleToBasket(Apple apple, Transform basket)
    {
        bool isCorrect = (basket == HealthyBasket.transform && apple.appleType == AppleType.Healthy)
                      || (basket == RottenBasket.transform  && apple.appleType == AppleType.Rotten);

        Vector3 randomOffset = new Vector3(
            UnityEngine.Random.Range(-0.05f, 0.05f),
            UnityEngine.Random.Range(-0.05f, 0f),
            UnityEngine.Random.Range(-0.05f, 0.05f)
        );

        Vector3 targetPos = basket.position + randomOffset;
        apple.transform
             .DOMove(targetPos, 0.5f)
             .SetEase(Ease.InOutSine)
             .OnComplete(() => apple.Pick(isCorrect));
    }

    private void HandleDroppedApple(Apple apple)
    {
        apple.Pick(false);
        Destroy(apple.gameObject);
    }

    private void PositionBasketsRelativeToPlayer()
    {
        XrOrigin.MoveCameraToWorldLocation(new Vector3(0, 1.36f, 0));
        XrOrigin.RotateAroundCameraUsingOriginUp(0);
        Vector3 basePos = Camera.main.transform.position;
        HealthyBasket.transform.position = basePos + HealthyBasketOffset;
        RottenBasket.transform.position  = basePos + RottenBasketOffset;
    }

    private void GenerateGridPositions()
    {
        Positions.Clear();

        const int layerCount = 3;
        const int horCount = 8, verCount = 4;
        const float radiusStart = 0.4f, radiusStep = 0.2f;
        const float hSpan = 90f, vSpan = 40f;

        Transform cam = Camera.main.transform;
        Vector3 arcCenter = cam.position + new Vector3(0.2f, 0f, -0.2f);
        Vector3 forward = Quaternion.Euler(0, ArcRotation, 0) * cam.forward;
        Vector3 right = Quaternion.AngleAxis(90f, Vector3.up) * forward;

        for (int layer = 0; layer < layerCount; layer++)
        {
            float radius = radiusStart + layer * radiusStep;

            for (int y = 0; y < verCount; y++)
            {
                float vAngle = Mathf.Lerp(-vSpan / 2f, vSpan / 2f, y / (float)(verCount - 1));

                for (int x = 0; x < horCount; x++)
                {
                    float hAngle = Mathf.Lerp(-hSpan / 2f, hSpan / 2f, x / (float)(horCount - 1));
                    Quaternion rot = Quaternion.AngleAxis(hAngle, Vector3.up) * Quaternion.AngleAxis(vAngle, right);

                    Vector3 pos = arcCenter + rot * forward * radius;
                    Positions.Add(new GridPosition(Vector3Int.zero, pos));
                }
            }
        }

        Debug.Log($"Generated {Positions.Count} grid positions.");
    }

    private void SpawnRandomApple()
    {
        if (!EnsureSetup()) return;
        if (CalibratedPositions.Count == 0)
        {
            Debug.LogWarning("No calibrated positions to spawn.");
            return;
        }

        var gridPos = CalibratedPositions[_rng.Next(CalibratedPositions.Count)];
        _currentGrid = gridPos.Grid;
        _spawnTime = Time.time;

        _currentApple = Instantiate(ApplePrefab, gridPos.WorldPosition, Quaternion.identity, transform);
        _currentApple.transform.localScale = Vector3.zero;
        _currentApple.transform.DOScale(Vector3.one * 0.04f, 0.5f);

        var apple = _currentApple.GetComponent<Apple>();
        var renderer = _currentApple.GetComponentInChildren<Renderer>();
        bool isRotten = _rng.NextDouble() < RottenChance;

        apple.position = gridPos;
        apple.isCalibrating = false;
        apple.appleType = isRotten ? AppleType.Rotten : AppleType.Healthy;
        renderer.material = isRotten ? RottenMaterial : HealthyMaterial;
    }

    private void SpawnAllApples()
    {
        if (!EnsureSetup()) return;

        foreach (var gp in Positions)
        {
            var go = Instantiate(ApplePrefab, gp.WorldPosition, Quaternion.identity, transform);
            go.transform.localScale = Vector3.zero;
            go.transform.DOScale(Vector3.one * 0.04f, 0.5f);

            var apple = go.GetComponent<Apple>();
            var renderer = go.GetComponentInChildren<Renderer>();
            bool isRotten = _rng.NextDouble() < RottenChance;

            apple.position = gp;
            apple.isCalibrating = true;
            apple.appleType = isRotten ? AppleType.Rotten : AppleType.Healthy;
            renderer.material = isRotten ? RottenMaterial : HealthyMaterial;
        }

        Debug.Log($"Spawned {Positions.Count} apples for calibration.");
    }

    private bool EnsureSetup()
    {
        bool ok = ApplePrefab && HealthyMaterial && RottenMaterial;
        if (!ok)
            Debug.LogError($"{name}: Missing prefab or materials.");
        return ok;
    }

    private void OnApplePicked(Apple apple)
    {
        if (apple.gameObject != _currentApple) return;

        LastPickSeconds = Time.time - _spawnTime;
        LastPickGrid    = _currentGrid;
        Debug.Log($"Picked in {LastPickSeconds:F2}s at {_currentGrid}");

        var mgr = FindAnyObjectByType<FirestoreAppointmentManager>();
        mgr?.SavePickAnalytics(_currentGrid, LastPickSeconds);

        SpawnRandomApple();
    }

    private void ShowCorrectLabel(Apple _) => BasketLabel.text = "Doğru";
    private void ShowWrongLabel(Apple _)   => BasketLabel.text = "Yanlış";

    public Material CalibratePosition(GridPosition position)
    {
        CalibratedPositions.Add(position);
        return TransparentMaterial;
    }
}
