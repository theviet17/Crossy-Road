using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
[CreateAssetMenu(fileName = "Game Data", menuName = "Game Data")]

public class GameData : ScriptableObject
{
    public int highestPoint;
    public int gold;
    public List<Character> characters = new List<Character>();
}
