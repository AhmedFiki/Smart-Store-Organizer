using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class ItemPlacer : MonoBehaviour
{
    [SerializeField] private Transform zonesParent;
    [SerializeField] private SODatabase database;
    [SerializeField] private StorageAreaType currentArea;

    [Header("Saving")]
    [SerializeField] private bool loadOnStart = false;
    [SerializeField] private bool clearSaveOnQuit = false;

    [Header("UI")]
    [SerializeField] private CanvasGroup tryAgainNotification;
    [SerializeField] private CanvasGroup tutorialNotification;
    [SerializeField] private CanvasGroup winNotification;
    [SerializeField] private float fadeDelay = 0.75f;
    [SerializeField] private float fadeDuration = 0.3f;
    [SerializeField] private float tutFadeDelay = 1f;
    [SerializeField] private float tutFadeDuration = 1f;

    [Header("SFX")]

    [SerializeField] private AudioClip wrongItemSFX;
    [SerializeField] private AudioClip poofSFX;
    [SerializeField] private AudioClip correctItemSFX;
    [SerializeField] private AudioClip winSFX;

    private ItemDragger dragger;
    private List<DropZone> zones = new List<DropZone>();
    private UnityEvent<DraggableItem, DropZone> onItemPlaceFailed = new UnityEvent<DraggableItem, DropZone>();
    private IEnumerator fadeCoroutine;
    public UnityEvent<DraggableItem, DropZone> OnItemPlaceFailed => onItemPlaceFailed;
    private bool hasWon = false;
    private bool isTutCompleted = false;
    private SFXHandler sfxHandler;
    private void Awake()
    {
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 1;

        dragger = FindAnyObjectByType<ItemDragger>();
        sfxHandler = FindAnyObjectByType<SFXHandler>();
        isTutCompleted = SaveManager.LoadTutorialData();

        if (zonesParent != null)
        {
            zones.AddRange(zonesParent.GetComponentsInChildren<DropZone>());
            for (int i = 0; i < zones.Count; i++)
            {
                zones[i].Initialize(currentArea, i.ToString());
            }
        }
    }
    private void OnApplicationQuit()
    {
        if (clearSaveOnQuit)
            ClearSaves();

        SaveManager.ClearTutorialData();
    }
    public static void ClearSaves()
    {
        SaveManager.ClearSaveData();
    }
    private void Start()
    {
        if (loadOnStart)
            LoadZones();

        if (!isTutCompleted && !hasWon)
        {
            ShowTutorialNotification();
        }
    }
    private void OnEnable()
    {
        if (dragger != null)
            dragger.OnItemDragEnded.AddListener(OnItemDragEnded);
    }
    private void OnDisable()
    {
        if (dragger != null)
            dragger.OnItemDragEnded.RemoveListener(OnItemDragEnded);
    }
    private void OnItemDragEnded(IDraggable item, DropZone zone)
    {
        if (zone == null || item == null || !(item is DraggableItem))
            return;

        DraggableItem draggableItem = item as DraggableItem;

        if (!zone.CanAccept(draggableItem))
        {
            ShowTryAgainNotification();

            sfxHandler?.PlaySFX(wrongItemSFX);
            sfxHandler?.PlaySFX(poofSFX);

            onItemPlaceFailed?.Invoke(draggableItem, zone);
            return;
        }

        if (!isTutCompleted)
        {
            isTutCompleted = true;
            SaveManager.SaveTutorialData(isTutCompleted);
            HideTutorialNotification();
        }

        sfxHandler?.PlaySFX(correctItemSFX);
        zone.RegisterItem(draggableItem);
        SaveZones();
        CheckWinCondition();
    }
    public void SaveZones()
    {
        if (zones.Count == 0)
        {
            Debug.Log($"FF->No Zones To Save");
            return;
        }

        SaveManager.SaveZones(zones, currentArea);
    }
    private void CheckWinCondition()
    {
        if (hasWon) return;

        for (int i = 0; i < zones.Count; i++)
        {
            if (!zones[i].HasItem)
                return;
        }

        hasWon = true;
        sfxHandler?.PlaySFX(winSFX);
        ShowWinNotification();
    }
    public void LoadZones()
    {
        GameSaveData saveData = SaveManager.LoadSaveData(currentArea);
        if (saveData == null) return;

        foreach (var zoneData in saveData.zones)
        {
            DropZone zone = zones.FirstOrDefault(z => z.ZoneID == zoneData.zoneId);
            if (zone == null) continue;

            ItemDataSO itemSO = database.GetItemById(zoneData.itemId);
            if (itemSO == null) continue;

            DraggableItem itemInstance = Instantiate(itemSO.Prefab, zone.DropPoint.position, Quaternion.identity, zonesParent);

            itemInstance.Initialize(itemSO);
            zone.RegisterItem(itemInstance);
        }

        CheckWinCondition();
    }
    private void ShowWinNotification()
    {
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = FadeTo(winNotification, 0, fadeDuration, 0, 1);
        StartCoroutine(fadeCoroutine);
    }
    private void ShowTryAgainNotification()
    {
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = FadeTo(tryAgainNotification, fadeDelay, fadeDuration, 1, 0);
        StartCoroutine(fadeCoroutine);
    }
    private void ShowTutorialNotification()
    {
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = FadeTo(tutorialNotification, tutFadeDelay, tutFadeDuration, 0, 1);
        StartCoroutine(fadeCoroutine);
    }
    private void HideTutorialNotification()
    {
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);
        fadeCoroutine = FadeTo(tutorialNotification, fadeDelay, fadeDuration, 1, 0);
        StartCoroutine(fadeCoroutine);
    }
    private IEnumerator FadeTo(CanvasGroup canvas, float delay, float duration, float start, float target)
    {
        canvas.alpha = start;
        yield return new WaitForSeconds(delay);

        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            canvas.alpha = Mathf.Lerp(start, target, time / duration);
            yield return null;
        }

        canvas.alpha = target;
    }
}