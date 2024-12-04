using UnityEngine;
using UnityEngine.UI;

public class UDPScreenManager : MonoBehaviour
{
    public UDPClient udpClient;
    public InputField roomInputField;
    public Text logText;

    void Start()
    {
        // 로그 초기화
        logText.text = "";
    }

    // 방 생성 버튼 클릭 시 호출
    public void OnCreateRoomButton()
    {
        string roomId = roomInputField.text.Trim();
        //Debug.Log(roomId);  
        if (!string.IsNullOrEmpty(roomId))
        {
            udpClient.CreateRoom(roomId);
            //Debug.Log("AAAA");
            Log($"방 생성 요청: {roomId}, 방이 생성되었습니다.");
        }
        else
        {
            Log("방 번호를 입력하세요.");
        }
    }
    
    // 방 참여 버튼 클릭 시 호출
    public void OnJoinRoomButton()
    {
        string roomId = roomInputField.text.Trim();
        if (!string.IsNullOrEmpty(roomId))
        {
            udpClient.JoinRoom(roomId);
            Log($"방 참여 요청: {roomId}, 방에 참가했습니다.");
        }
        else
        {
            Log("방 번호를 입력하세요.");
        }
    }

    // 로그 출력
    public void Log(string message)
    {
        logText.text = message;
    }
}