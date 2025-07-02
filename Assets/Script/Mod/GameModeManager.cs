using System.Collections;
using System.Linq;
using UnityEngine;

public enum GameMode { Measurement, WrongBasket, DropOnly, Unreachable, Static, PerceptionEvaluation, None }

public class GameModeManager : MonoBehaviour
{
    public static GameModeManager Instance { get; private set; }
    
    public GameMode CurrentMode { get; private set; } = GameMode.Measurement;

    [Header("Scene References")]
    public MeasurementData data;                 // GameSystems objesindeki component
    public GridAppleSpawner spawner;             // sahnedeki mevcut spawner
    public GameObject gameModeUI;
    // (insp.’dan sürükle bırak)
    
    private int remainingApples;
    
    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }
    

    // ——  MODE ENTRY  ————————————————————————————————
    public void StartMeasurement()
    {
        gameModeUI.SetActive(false);
        CurrentMode = GameMode.Measurement;
        data.ResetData();

        spawner.isMeasureMode = true;
    }

    public void StartWrongBasket()
    {
        gameModeUI.SetActive(false);
        CurrentMode = GameMode.WrongBasket;
        spawner.RealignToHeadset();

        // Ölçümden gelen sonuçlar arasından sadece WrongBasket olan grid’leri çek
        var wrongGrids = data.Filter(AppleOutcome.WrongBasket);

        // Hiç yanlış elma yoksa UI’den uyarı göstermek isteyebilirsin
        if (wrongGrids.Count == 0)
        {
            Debug.Log("Ölçüm sonuçlarında yanlış sepete konan elma yok.");
            return;
        }

        // Spawner’a sadece bu grid’lerde tekrar elma oluşturmasını söyle
        spawner.SpawnCustomApples(wrongGrids.ToArray());
    }
    

    public void StartDropOnly()
    {
        gameModeUI.SetActive(false);
        
        CurrentMode = GameMode.DropOnly;
        spawner.RealignToHeadset();
        /* 1️⃣  Ölçüm sonuçlarını topla */
        var drops   = data.Filter(AppleOutcome.Drop);       // Sepete giderken düşen
        var misses  = data.Filter(AppleOutcome.GrabMiss);   // Dokundu ama kavrayamadı

        /* 2️⃣  Listeleri birleştir, tekrarı önle */
        var allGrids = drops.Concat(misses)
            .Distinct()          // aynı grid iki outcome ise tek olsun
            .ToList();

        /* 3️⃣  Hiç elma yoksa UI’yi açıp çık */
        if (allGrids.Count == 0)
        {
            FinishCurrentMode();
            return;
        }

        /* 4️⃣  Sayaç + event aboneliği */
        remainingApples = allGrids.Count;
        Apple.OnAnyApplePicked += HandleApplePicked;

        /* 5️⃣  Elmaların spawn’u */
        spawner.SpawnCustomApples(allGrids.ToArray());
        
    }

    public void StartUnreachable()
    {
        gameModeUI.SetActive(false);
        
        CurrentMode = GameMode.Unreachable;
        spawner.RealignToHeadset();
        /* 1️⃣ Ölçüm sonuçlarından sadece Unreachable grid’lerini al */
        var unreachableGrids = data.Filter(AppleOutcome.Unreachable);

        /* 2️⃣ Hiç elma yoksa modu bitmiş say ve UI'yi aç */
        if (unreachableGrids.Count == 0)
        {
            FinishCurrentMode();
            return;
        }

        /* 3️⃣ Sayaç ayarla ve global elma-olayına abone ol */
        remainingApples = unreachableGrids.Count;
        Apple.OnAnyApplePicked += HandleApplePicked;

        /* 4️⃣ Spawn */
        spawner.SpawnCustomApples(unreachableGrids.ToArray());
        
    }

    public void StartStatic()
    {
        gameModeUI.SetActive(false);
        CurrentMode = GameMode.Static;
        spawner.RealignToHeadset();
        // Static mod, elma oluşturma işlemini manuel olarak yapacağımız bir mod
        // Bu modda, elmaların konumlarını manuel olarak belirleyeceğiz
        Vector3Int[] manualGrids = new Vector3Int[]
        {
            new Vector3Int(0, 0, 0), // Örnek grid konumları
            new Vector3Int(1, 0, 0),
            new Vector3Int(2, 0, 0)
        };
        StartStatic(manualGrids);
    }

    public void StartStatic(Vector3Int[] manualGrids)
    {
        CurrentMode = GameMode.Static;
        spawner.SpawnCustomApples(manualGrids);  // bu helper’ı spawner’a az sonra ekleyeceğiz
    }
    public void StartPerceptionEvaluation()
    {
        gameModeUI.SetActive(false);
        CurrentMode = GameMode.PerceptionEvaluation;
        spawner.RealignToHeadset();

        /* 1️⃣ Tüm başarılı sonuçları al - bu elmalar doğru tanınmış olmalı */
        var successGrids = data.Filter(AppleOutcome.Success);
        
        /* 2️⃣ Yanlış sepete konan elmaları al - bu elmalar yanlış tanınmış */
        var wrongBasketGrids = data.Filter(AppleOutcome.WrongBasket);
        
        /* 3️⃣ Her iki listeyi birleştir - algı değerlendirmesi için tüm elmaları kullan */
        var allPerceptionGrids = successGrids.Concat(wrongBasketGrids)
            .Distinct()
            .ToList();

        /* 4️⃣ Hiç elma yoksa modu bitmiş say ve UI'yi aç */
        if (allPerceptionGrids.Count == 0)
        {
            Debug.Log("Algı değerlendirmesi için yeterli veri yok.");
            FinishCurrentMode();
            return;
        }

        /* 5️⃣ Sayaç ayarla ve global elma-olayına abone ol */
        remainingApples = allPerceptionGrids.Count;
        Apple.OnAnyApplePicked += HandleApplePicked;

        /* 6️⃣ Elmaların spawn'u - karışık sağlıklı ve çürük elmalar */
        spawner.SpawnCustomApples(allPerceptionGrids.ToArray());
    }

    // ——  HELPERS  ————————————————————————————————————
    private void SpawnFiltered(AppleOutcome outcome)
    {
        var grids = data.Filter(outcome);
        spawner.SpawnCustomApples(grids.ToArray());
    }
    
    private void HandleApplePicked(Apple _)
    {
        if (--remainingApples > 0) return;

        Apple.OnAnyApplePicked -= HandleApplePicked;
        FinishCurrentMode();
    }

    /*───────── Ortak bitiş fonksiyonu ───────*/
    private void FinishCurrentMode()
    {
        gameModeUI.SetActive(true);   // Gamemode UI aç
        CurrentMode = GameMode.None;  // Pasif bekleme
    }
    
    public void EndModeByButton()
{
    // Aboneliği temizle (aktifse)
    Apple.OnAnyApplePicked -= HandleApplePicked;

    // Elma kalabalığını yok et
    spawner.DestroyAllApples();

    // UI’yi geri aç + state’i sıfırla
    FinishCurrentMode();
    }
 
}