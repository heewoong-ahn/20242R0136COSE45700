using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

//struct : cannot implement, inherit 
public struct NetworkInputData : INetworkInput
{
    public Vector3 direction;
    public bool leftPunch;

    //bit 연산자 사용한 버튼값 저장.
    public const uint BUTTON_FORWARD = 1 << 0;
    public const uint BUTTON_BACKWARD = 1 << 1;
    public const uint BUTTON_ATTACK_T = 1 << 2;
    public const uint BUTTON_ATTACK_Y = 1 << 3;
    public const uint BUTTON_ATTACK_U = 1 << 4;
    public const uint BUTTON_GUARD = 1 << 5;

    public const uint BUTTON_DODGE_FRONT = 1 << 6;
    public const uint BUTTON_DODGE_BACK = 1 << 7;

    public const uint BUTTON_ATTACK_TT = 1 << 8;
    public const uint BUTTON_ATTACK_TTT = 1 << 9;
    public const uint BUTTON_ATTACK_YY = 1 << 10;

    public uint buttons;
}
