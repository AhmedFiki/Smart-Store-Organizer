using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ConveyerItem
{
    public DraggableItem Item;
    public Transform PosTransform;
}
public class ItemSpawner : MonoBehaviour
{
    [SerializeField] private ConveyorBelt conveyorBelt;
    [SerializeField] private SODatabase database;
    [SerializeField] private float conveyerItemsSpeed = 2;
    [SerializeField] private float autoSpawnInterval = 2.5f;
    [SerializeField] private bool autoSpawnOnStart = true;

    private List<ConveyerItem> conveyerItems = new List<ConveyerItem>();
    private Dictionary<DraggableItem, ConveyerItem> itemDictionary = new Dictionary<DraggableItem, ConveyerItem>();
    private IEnumerator autoSpawnEnum;
    private ItemDragger dragger;

    private void Awake()
    {
        dragger = FindAnyObjectByType<ItemDragger>();
    }
    private void Start()
    {
        StartAutoSpawn();
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
        if (item == null || !(item is DraggableItem) || zone == null)
            return;

        DraggableItem draggableItem = item as DraggableItem;
        ConveyerItem conveyerItem = GetConveyerItem(draggableItem);

        if (!zone.CanAccept(draggableItem))
        {
            if (conveyerItem == null)
                Destroy(draggableItem.gameObject);
            
            return;
        }

        if (conveyerItem != null)
        {
            conveyerItems.Remove(conveyerItem);
            itemDictionary.Remove(conveyerItem.Item);
            Destroy(conveyerItem.PosTransform.gameObject);
        }
    }
    private ConveyerItem GetConveyerItem(DraggableItem item)
    {
        if (itemDictionary.TryGetValue(item, out var conveyerItem))
        {
            return conveyerItem;
        }
        return null;
    }
    private void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.S))
        {
            SpawmItem();
        }
#endif
        for (int i = conveyerItems.Count - 1; i >= 0; i--)
        {
            ConveyerItem conveyerItem = conveyerItems[i];
            if (conveyorBelt.IsActive)
            {
                conveyerItem.PosTransform.position = Vector3.MoveTowards(conveyerItem.PosTransform.position, conveyorBelt.EndTransform.position, conveyerItemsSpeed * Time.deltaTime);
                if (conveyerItem.Item.IsDragging == false && !conveyerItem.Item.IsPlaced)
                    conveyerItem.Item.transform.position = conveyerItem.PosTransform.position;
            }
            float distanceToEnd = Vector3.Distance(conveyerItem.Item.transform.position, conveyorBelt.EndTransform.position);
            
            if (distanceToEnd < 0.1f)
            {
                Destroy(conveyerItem.PosTransform.gameObject);
                Destroy(conveyerItem.Item.gameObject);
                itemDictionary.Remove(conveyerItem.Item);
                conveyerItems.RemoveAt(i);
                i--;
            }
        }
    }
    [ContextMenu("Spawn Item")]
    public void SpawmItem()
    {
        Vector3 spawnPos = conveyorBelt.StartTransform.position;

        ItemDataSO itemDataSO = database.GetRandomItem();

        if (itemDataSO == null)
        {
            Debug.LogError("No item data found in database!");
            return;
        }

        DraggableItem newItem = Instantiate(itemDataSO.Prefab, spawnPos, Quaternion.identity);
        newItem.Initialize(itemDataSO);
        Transform itemPosTransform = new GameObject("ConveyerItemTransform").transform;
        itemPosTransform.position = spawnPos;

        ConveyerItem conveyerItem = new ConveyerItem
        {
            Item = newItem,
            PosTransform = itemPosTransform
        };

        conveyerItems.Add(conveyerItem);
        itemDictionary.Add(newItem, conveyerItem);
    }
    private void StartAutoSpawn()
    {
        if (autoSpawnEnum != null)
            StopCoroutine(autoSpawnEnum);

        autoSpawnEnum = AutoSpawn();
        StartCoroutine(autoSpawnEnum);
    }
    private void StopAutoSpawn()
    {
        if (autoSpawnEnum != null)
            StopCoroutine(autoSpawnEnum);
    }
    private IEnumerator AutoSpawn()
    {
        while (true)
        {
            SpawmItem();
            yield return new WaitForSeconds(autoSpawnInterval);
        }
    }
}
