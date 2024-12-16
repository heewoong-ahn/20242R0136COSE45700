using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//데이터 서버측으로 보내고 받을 때 사용할 형식.
[Serializable]
public class PlayerInputData
{
    public int playerId;
    //public string roomId;
    public string action; // 예: "MoveForward", "Punch"
    public Vector3 position; // 위치 정보 (필요한 경우)
    public float timestamp; // 시간 동기화용 (필요한 경우)
}
