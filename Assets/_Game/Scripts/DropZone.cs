using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DropZone : MonoBehaviour
{
    [SerializeField] private Transform dropPoint;
    [SerializeField] private List<CategorySO> allowedCategories = new List<CategorySO>();

    [Header("VFX")]
    [SerializeField] private ParticleSystem wrongVFX;
    [SerializeField] private ParticleSystem correctVFX;

    private StorageAreaType storageType;
    private DraggableItem registeredItem;
    private UnityEvent<DraggableItem> onItemRegistered = new UnityEvent<DraggableItem>();
    private string id;

    public string ZoneID => id;
    public UnityEvent<DraggableItem> OnItemRegistered => onItemRegistered;
    public Transform DropPoint => dropPoint;
    public DraggableItem RegisteredItem => registeredItem;
    public bool HasItem => registeredItem != null;

    public void Initialize(StorageAreaType type, string id)
    {
        storageType = type;
        this.id = $"{id}_{type.ToString()}";
    }
    public bool CanAccept(DraggableItem item)
    {
        if (HasItem)
        {
            Debug.Log($"Zone {name} already has an item");
            return false;
        }

        if (item?.ItemData == null)
        {
            Debug.Log("Item or ItemData is null");
            return false;
        }

        ItemDataSO itemData = item.ItemData;

        if (!allowedCategories.Contains(itemData.Category))
        {
            Debug.Log($"Zone {name} does not accept category {itemData.Category}");
            PlayVFX(false);
            return false;
        }

        if (itemData.StorageAreaType != storageType)
        {
            Debug.Log($"Zone {name} does not accept storage type {storageType}");
            PlayVFX(false);
            return false;
        }

        return true;
    }
    public void RegisterItem(DraggableItem item)
    {
        item.Collider.enabled = false;
        item.IsPlaced = true;
        registeredItem = item;
        onItemRegistered?.Invoke(item);
        PlayVFX(true);
    }
    public void ClearItem()
    {
        registeredItem = null;
    }
    private void PlayVFX(bool correct)
    {
        if (correct)
        {
            if (correctVFX != null)
                correctVFX.Play();
        }
        else
        {
            if (wrongVFX != null)
                wrongVFX.Play();
        }
    }
}
