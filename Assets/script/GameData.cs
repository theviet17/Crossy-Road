using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using Newtonsoft.Json;

[CreateAssetMenu(fileName = "Game Data", menuName = "Game Data")]

public class GameData : ScriptableObject
{
    public int highestPoint;
    public int gold;
    public List<Character> characters = new List<Character>();

    // Lưu GameData thành tệp JSON
    public void SaveToJson()
    {
        string jsonData = JsonConvert.SerializeObject(this, Formatting.Indented);
        File.WriteAllText(Application.dataPath + "/GameData.json", jsonData);
    }

    // Đọc GameData từ tệp JSON
    public static GameData LoadFromJson()
    {
        string jsonData = File.ReadAllText(Application.dataPath + "/GameData.json");
        return JsonConvert.DeserializeObject<GameData>(jsonData);
    }
    public GameData LoadNewGameData()
    {
        GameData loadData = LoadFromJson();
        for (int i = 0; i < loadData.characters.Count; i++)
        {
            loadData.characters[i] = new Character(characters[i].prefab, loadData.characters[i].type, loadData.characters[i].cost );
        }
        return loadData;
    }
}
