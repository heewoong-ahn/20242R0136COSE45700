using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using UnityEngine.SceneManagement;
using Fusion.Sockets;
using System;

public class BasicSpawner : MonoBehaviour, INetworkRunnerCallbacks
{

    private NetworkRunner networkRunner; //��� callback ����?

    //network�󿡼� ���� ���� ������Ʈ �������� ����.
    [SerializeField] private NetworkPrefabRef networkPrefabRef1;
    [SerializeField] private NetworkPrefabRef networkPrefabRef2;

    //playerRef: ����� �ĺ���, NetworkObject: ��Ʈ��ũ�� ���� ����ȭ�ǰ� ��ŵ� �� �ִ� object 
    private Dictionary<PlayerRef, NetworkObject> spawnCharacter = new Dictionary<PlayerRef, NetworkObject>();

    //�� �̸� �ο�
    private string roomCode = "";
    private string inputRoomCode = "";

    //Host 
    async void  GameStart(GameMode mode)
    {
        //creating runner and saying user it giving input
        networkRunner = gameObject.AddComponent<NetworkRunner>();
        networkRunner.ProvideInput = true; //letting the network know we are going to perform? 

        //scene info �߰�. ��Ʈ��ũ �󿡼� ����ȭ�� Scene.
        var scene = SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex);
        var sceneInfo = new NetworkSceneInfo();
        if (scene.IsValid)
        {
            sceneInfo.AddSceneRef(scene, LoadSceneMode.Additive);
        }

        //�� �����. 
        string sessionName = "";

        if (mode == GameMode.Host)
        {
            // ���� ���� (��: 6�ڸ� ����)
            roomCode = UnityEngine.Random.Range(100000, 999999).ToString();
            sessionName = roomCode;
        }

        else if (mode == GameMode.Client)
        {
            // �Էµ� �� ��ȣ ���
            sessionName = inputRoomCode;
        }

        //create session 
        await networkRunner.StartGame(new StartGameArgs()
        {
            GameMode = mode,
            SessionName = sessionName,
            Scene = scene,
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>(),
            PlayerCount = 2,
             
        }
    );
    }

    //gui�� �������. 
    private void OnGUI()
    {
        if (networkRunner == null || !networkRunner.IsRunning)
        {
            GUILayout.BeginArea(new Rect(10, 10, 300, 200));

            if (GUILayout.Button("ȣ��Ʈ"))
            {
                GameStart(GameMode.Host);
            }

            GUILayout.Space(20);
            GUILayout.Label("������ �� ��ȣ �Է�:");
            inputRoomCode = GUILayout.TextField(inputRoomCode, 25);

            if (GUILayout.Button("����"))
            {
                GameStart(GameMode.Client);
            }

            GUILayout.EndArea();
        }
        else
        {
            // ȣ��Ʈ���� �� ��ȣ�� ǥ��
            if (networkRunner.GameMode == GameMode.Host)
            {
                GUILayout.BeginArea(new Rect(10, 10, 300, 200));
                GUILayout.Label($"�� ��ȣ: {roomCode}");
                GUILayout.EndArea();
            }
        }
    }


    public void OnConnectedToServer(NetworkRunner runner)
    {
      
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
        
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
      
    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {
       
    }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {
       
    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
        
    }

    private float lastDPressTime = -1f; // ���������� D�� ���� �ð�
    private float lastAPressTime = -1f; // ���������� D�� ���� �ð�
    private float doublePressInterval = 0.5f; // 2�� �Է� �ν� ����
    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        var data = new NetworkInputData();

        if (Input.GetKey(KeyCode.W))
        {
            data.direction += Vector3.left;
        }
        if (Input.GetKey(KeyCode.A))
        {
            data.direction += Vector3.back;
        }
        if (Input.GetKey(KeyCode.S))
        {
            data.direction += Vector3.right;
        }
        if (Input.GetKey(KeyCode.D))
        {
            data.direction += Vector3.forward;
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            data.leftPunch = true;
        }

        //host�� �����͸� �Ѱ���. 
        input.Set(data);

        /*   if (Input.GetKey(KeyCode.D))
               data.buttons |= NetworkInputData.BUTTON_FORWARD;

           // Use GetKeyDown to detect key presses for double-tap
           if (Input.GetKeyDown(KeyCode.D))
           {
               float currentTime = runner.SimulationTime;

               if (currentTime - lastDPressTime <= doublePressInterval)
               {
                   // Double-tap detected
                   data.buttons |= NetworkInputData.BUTTON_DODGE_FRONT;
                   lastDPressTime = -doublePressInterval; // Reset to prevent immediate retrigger
               }
               else
               {
                   // First tap detected
                   lastDPressTime = currentTime;
               }
           }

           if (Input.GetKey(KeyCode.A))
               data.buttons |= NetworkInputData.BUTTON_BACKWARD;

           if (Input.GetKeyDown(KeyCode.A))
           {
               float currentTime = runner.SimulationTime;

               if (currentTime - lastAPressTime <= doublePressInterval)
               {
                   // Double-tap detected
                   data.buttons |= NetworkInputData.BUTTON_DODGE_BACK;
                   lastAPressTime = -doublePressInterval; // Reset to prevent immediate retrigger
               }
               else
               {
                   // First tap detected
                   lastAPressTime = currentTime;
               }
           }

           if (Input.GetKeyDown(KeyCode.T))
               data.buttons |= NetworkInputData.BUTTON_ATTACK_T;

           if (Input.GetKeyDown(KeyCode.Y))
               data.buttons |= NetworkInputData.BUTTON_ATTACK_Y;

           if (Input.GetKeyDown(KeyCode.U))
               data.buttons |= NetworkInputData.BUTTON_ATTACK_U;

           if (Input.GetKeyDown(KeyCode.G))
               data.buttons |= NetworkInputData.BUTTON_GUARD;

           input.Set(data);*/

    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
     
    }

    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
   
    }

    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
      
    }

    private Quaternion spawnRotation;
    private Vector3  playerPos;
    private NetworkObject networkObject;
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if (runner.IsServer)
        {
            Debug.Log(runner.GameMode);
            Debug.Log(player.IsMasterClient);
            Debug.Log(player.PlayerId);
            Debug.Log(player.AsIndex);

            if (player.PlayerId % 2 == 1)
            {
                playerPos = new Vector3(0f, 10f, -4f);
                spawnRotation = Quaternion.Euler(0, 0, 0);
                //networkPrefabRef�� ���� prefab���� ���ο� object����� ��.
                networkObject = runner.Spawn(networkPrefabRef1, playerPos, spawnRotation, player);
            }
           else
            {
                playerPos = new Vector3(1.5f, 10f, 4f);
                spawnRotation = Quaternion.Euler(0, 180, 0); 
                //networkPrefabRef�� ���� prefab���� ���ο� object����� ��.
                networkObject = runner.Spawn(networkPrefabRef2, playerPos, spawnRotation, player);
            }


           
            spawnCharacter.Add(player, networkObject);

        }
        
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        if (spawnCharacter.TryGetValue(player, out NetworkObject networkObject))
        {
            runner.Despawn(networkObject);
            spawnCharacter.Remove(player);
        }
     
    }

    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
    {
       
    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
    {

    }

    public void OnSceneLoadDone(NetworkRunner runner)
    { 

    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {

    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
       
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
       
    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
       
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
