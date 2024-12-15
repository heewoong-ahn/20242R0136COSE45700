using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//유니티 내에서 쓸 전역변수 설정.
[CreateAssetMenu(fileName = "GameData", menuName = "Global/GameData")]
public class GameData : ScriptableObject
{
    public int playerId;
    public float gameTime;//public string roomId;
}
