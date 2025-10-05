using UnityEngine;

[CreateAssetMenu(fileName = "CategorySO", menuName = "Scriptable Objects/CategorySO")]
public class CategorySO : ScriptableObject
{
    public string ID;
    public string DisplayName;

    public Sprite Icon;
    public Color Color;

    public StorageAreaType StorageAreaType;
}
