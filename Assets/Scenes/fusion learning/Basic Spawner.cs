using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using UnityEngine.SceneManagement;
using Fusion.Sockets;
using System;

public class BasicSpawner : MonoBehaviour, INetworkRunnerCallbacks
{

    private NetworkRunner networkRunner; //모든 callback 실행?

    //network상에서 사용될 게임 오브젝트 프리팹을 참조.
    [SerializeField] private NetworkPrefabRef networkPrefabRef1;
    [SerializeField] private NetworkPrefabRef networkPrefabRef2;

    //playerRef: 사용자 식별자, NetworkObject: 네트워크를 통해 동기화되고 통신될 수 있는 object 
    private Dictionary<PlayerRef, NetworkObject> spawnCharacter = new Dictionary<PlayerRef, NetworkObject>();

    //방 이름 부여
    private string roomCode = "";
    private string inputRoomCode = "";

    //Host 
    async void  GameStart(GameMode mode)
    {
        //creating runner and saying user it giving input
        networkRunner = gameObject.AddComponent<NetworkRunner>();
        networkRunner.ProvideInput = true; //letting the network know we are going to perform? 

        //scene info 추가. 네트워크 상에서 동기화된 Scene.
        var scene = SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex);
        var sceneInfo = new NetworkSceneInfo();
        if (scene.IsValid)
        {
            sceneInfo.AddSceneRef(scene, LoadSceneMode.Additive);
        }

        //방 만들기. 
        string sessionName = "";

        if (mode == GameMode.Host)
        {
            // 난수 생성 (예: 6자리 숫자)
            roomCode = UnityEngine.Random.Range(100000, 999999).ToString();
            sessionName = roomCode;
        }

        else if (mode == GameMode.Client)
        {
            // 입력된 방 번호 사용
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

    //gui를 만들어줌. 
    private void OnGUI()
    {
        if (networkRunner == null || !networkRunner.IsRunning)
        {
            GUILayout.BeginArea(new Rect(10, 10, 300, 200));

            if (GUILayout.Button("호스트"))
            {
                GameStart(GameMode.Host);
            }

            GUILayout.Space(20);
            GUILayout.Label("참가할 방 번호 입력:");
            inputRoomCode = GUILayout.TextField(inputRoomCode, 25);

            if (GUILayout.Button("참가"))
            {
                GameStart(GameMode.Client);
            }

            GUILayout.EndArea();
        }
        else
        {
            // 호스트에게 방 번호를 표시
            if (networkRunner.GameMode == GameMode.Host)
            {
                GUILayout.BeginArea(new Rect(10, 10, 300, 200));
                GUILayout.Label($"방 번호: {roomCode}");
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

    private float lastDPressTime = -1f; // 마지막으로 D가 눌린 시간
    private float lastAPressTime = -1f; // 마지막으로 D가 눌린 시간
    private float doublePressInterval = 0.5f; // 2번 입력 인식 간격
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

        //host에 데이터를 넘겨줌. 
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
                //networkPrefabRef를 통해 prefab에서 새로운 object만들어 냄.
                networkObject = runner.Spawn(networkPrefabRef1, playerPos, spawnRotation, player);
            }
           else
            {
                playerPos = new Vector3(1.5f, 10f, 4f);
                spawnRotation = Quaternion.Euler(0, 180, 0); 
                //networkPrefabRef를 통해 prefab에서 새로운 object만들어 냄.
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
