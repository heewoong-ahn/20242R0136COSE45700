using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//서버로부터의 input을 분기처리하기 위함.
public class InputManager : MonoBehaviour
{
    public static InputManager Instance;
    public GameData gamedata;

    private Queue<PlayerInputData> playerInputs = new Queue<PlayerInputData>();
    private Queue<PlayerInputData> enemyInputs = new Queue<PlayerInputData>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        // 매 프레임마다 서버 데이터를 처리
        ProcessReceivedInputs();
    }

    public void ProcessReceivedInputs()
    {
        while (UDPClient.Instance.inputQueue.Count > 0)
        {
            string jsonData = UDPClient.Instance.inputQueue.Dequeue();
            PlayerInputData inputData = JsonUtility.FromJson<PlayerInputData>(jsonData);

            if (inputData.playerId == gamedata.playerId)
            {
                playerInputs.Enqueue(inputData);
            }
            else
            {
                enemyInputs.Enqueue(inputData);
            }
        }
    }

    public PlayerInputData GetPlayerInput()
    {
        if (playerInputs.Count > 0)
        {
            return playerInputs.Dequeue();
        }
        return null;
    }

    public PlayerInputData GetEnemyInput()
    {
        if (enemyInputs.Count > 0)
        {
            return enemyInputs.Dequeue();
        }
        return null;
    }
}
