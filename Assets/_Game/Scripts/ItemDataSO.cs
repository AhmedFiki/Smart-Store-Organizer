using UnityEngine;
using UnityEngine.Localization;

[CreateAssetMenu(fileName = "ItemDataSO", menuName = "Scriptable Objects/ItemDataSO")]
public class ItemDataSO : ScriptableObject
{
    public string ID;
    public string DisplayName;
    public LocalizedString LocalizedName;

    public DraggableItem Prefab;
    public Sprite Icon;

    public CategorySO Category; // book, food, clothing, electronics, etc.
    public StorageAreaType StorageAreaType => Category != null ? Category.StorageAreaType : StorageAreaType.Shelf;
}
