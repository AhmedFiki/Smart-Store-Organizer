using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Item Database")]
public class SODatabase : ScriptableObject
{
    public List<ItemDataSO> allItems;
    public List<CategorySO> allCategories;

    private List<ItemDataSO> lastItems = new List<ItemDataSO>();
    private const int historySize = 3;

    public ItemDataSO GetItemById(string id)
    {
        return allItems.FirstOrDefault(i => i.ID == id);
    }
    public CategorySO GetCategoryById(string id)
    {
        return allCategories.FirstOrDefault(c => c.ID == id);
    }
    public ItemDataSO GetRandomItem()
    {
        if (allItems == null || allItems.Count == 0) return null;

        ItemDataSO randomItem;
        int iterations = 0;

        do
        {
            randomItem = allItems[UnityEngine.Random.Range(0, allItems.Count)];
            iterations++;
        }
        while (lastItems.Contains(randomItem) && iterations < 10);

        lastItems.Add(randomItem);
        if (lastItems.Count > historySize)
            lastItems.RemoveAt(0);

        return randomItem;
    }
}