using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


[Serializable]
public class PlayerInputData
{
    public int playerId;
    //public string roomId;
    public string action; // ��: "MoveForward", "Punch"
    public Vector3 position; // ��ġ ���� (�ʿ��� ���)
    public float timestamp; // �ð� ����ȭ�� (�ʿ��� ���)
}
