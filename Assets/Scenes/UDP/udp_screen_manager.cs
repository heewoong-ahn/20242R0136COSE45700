using UnityEngine;
using UnityEngine.UI;

public class UDPScreenManager : MonoBehaviour
{
    public UDPClient udpClient;
    public InputField roomInputField;
    public Text logText;

    void Start()
    {
        // �α� �ʱ�ȭ
        logText.text = "";
    }

    // �� ���� ��ư Ŭ�� �� ȣ��
    public void OnCreateRoomButton()
    {
        string roomId = roomInputField.text.Trim();
        //Debug.Log(roomId);  
        if (!string.IsNullOrEmpty(roomId))
        {
            udpClient.CreateRoom(roomId);
            //Debug.Log("AAAA");
            Log($"�� ���� ��û: {roomId}, ���� �����Ǿ����ϴ�.");
        }
        else
        {
            Log("�� ��ȣ�� �Է��ϼ���.");
        }
    }
    
    // �� ���� ��ư Ŭ�� �� ȣ��
    public void OnJoinRoomButton()
    {
        string roomId = roomInputField.text.Trim();
        if (!string.IsNullOrEmpty(roomId))
        {
            udpClient.JoinRoom(roomId);
            Log($"�� ���� ��û: {roomId}, �濡 �����߽��ϴ�.");
        }
        else
        {
            Log("�� ��ȣ�� �Է��ϼ���.");
        }
    }

    // �α� ���
    public void Log(string message)
    {
        logText.text = message;
    }
}