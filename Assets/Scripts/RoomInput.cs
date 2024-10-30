using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomInput : MonoBehaviour
{
    public InputField myInputField;
    public int roomNumber;

    public void Start()
    {
        myInputField.onEndEdit.AddListener(OnInputEnd);
    }

    private void OnInputEnd(string text)
    {
        if (int.TryParse(text, out roomNumber))
        {

            Debug.Log("사용자가 입력한 방 번호: " + roomNumber);
        }
   
    }
}
