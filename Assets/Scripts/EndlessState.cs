using System;
using System.Collections.Generic;
using System.IO;
using Constants;
using UnityEngine;

[Serializable]
public class EndlessState
{
    [field: SerializeField] public Scene Level { get; set; }
    [field: SerializeField] public int Score { get; set; }
    [field: SerializeField] public float PlayerHealth { get; set; }
    [field: SerializeField] public Vector3 PlayerPosition { get; set; }
    [field: SerializeField] public Vector3[] EnemyPositions { get; set; }

    private static string SavePath => Path.Join(Application.persistentDataPath, "endless.json");

    public void Save()
    {
        var json = JsonUtility.ToJson(this);

        Debug.Log($"Saved state: {json}");

        File.WriteAllText(SavePath, json);
        PlayerPrefs.SetInt(PlayerPrefsKeys.EndlessInProgress, 1);
    }

    public static EndlessState Load()
    {
        var json = File.ReadAllText(SavePath);
        return JsonUtility.FromJson<EndlessState>(json);
    }

    public static void Clear()
    {
        File.Delete(SavePath);
        PlayerPrefs.SetInt(PlayerPrefsKeys.EndlessInProgress, 0);
    }
}