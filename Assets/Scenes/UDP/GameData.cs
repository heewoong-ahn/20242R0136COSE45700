using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//����Ƽ ������ �� �������� ����.
[CreateAssetMenu(fileName = "GameData", menuName = "Global/GameData")]
public class GameData : ScriptableObject
{
    public int playerId;
    public float gameTime;//public string roomId;
}
