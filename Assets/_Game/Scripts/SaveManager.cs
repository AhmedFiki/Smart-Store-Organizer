using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ZoneSaveData
{
    public string zoneId;
    public string itemId;
}
[Serializable]
public class GameSaveData
{
    public List<ZoneSaveData> zones = new List<ZoneSaveData>();
}
public static class SaveManager
{
    private const string saveKey = "SAVE_DATA";
    private const string tutorialSaveKey = "TUTORIAL_SAVE_DATA";

    public static void SaveZones(List<DropZone> allZones, StorageAreaType area)
    {
        string areaSaveKey = $"{saveKey}_{area.ToString()}";
        GameSaveData saveData = new GameSaveData();

        for (int i = 0; i < allZones.Count; i++)
        {
            DropZone zone = allZones[i];

            if (zone.RegisteredItem == null) continue;

            var zoneSaveData = new ZoneSaveData
            {
                zoneId = zone.ZoneID,
                itemId = zone.RegisteredItem.ItemData.ID
            };

            saveData.zones.Add(zoneSaveData);
        }

        string json = JsonUtility.ToJson(saveData);
        PlayerPrefs.SetString(areaSaveKey, json);
        PlayerPrefs.Save();
    }
    public static GameSaveData LoadSaveData(StorageAreaType area)
    {
        string areaSaveKey = $"{saveKey}_{area.ToString()}";

        if (!PlayerPrefs.HasKey(areaSaveKey))
            return null;

        string json = PlayerPrefs.GetString(areaSaveKey);
        return JsonUtility.FromJson<GameSaveData>(json);
    }
    public static void SaveTutorialData(bool isCompleted)
    {
        PlayerPrefs.SetInt(tutorialSaveKey, isCompleted ? 1 : 0);
        PlayerPrefs.Save();
    }
    public static bool LoadTutorialData()
    {
        if (!PlayerPrefs.HasKey(tutorialSaveKey))
            return false;
        int value = PlayerPrefs.GetInt(tutorialSaveKey);
        return value == 1;
    }
    public static void ClearSaveData()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
    }
    public static void ClearAreaSaveData(StorageAreaType area)
    {
        string areaSaveKey = $"{saveKey}_{area.ToString()}";
        PlayerPrefs.DeleteKey(areaSaveKey);
        PlayerPrefs.Save();
    }
    public static void ClearTutorialData()
    {
        PlayerPrefs.DeleteKey(tutorialSaveKey);
        PlayerPrefs.Save();
    }
}