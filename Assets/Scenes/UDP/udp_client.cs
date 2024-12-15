using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine.SceneManagement;
using System.Collections.Generic; // Queue<>와 기타 제네릭 컬렉


public class UDPClient : MonoBehaviour
{
    
    public static UDPClient Instance; //싱글톤으로 생성해 같은 정보의 udpclient를 playercontroller등에서도 사용해 정보를 넘김. 
    private UdpClient udpClient;

    private IPEndPoint serverEndPoint;
    private Thread receiveThread;
    private bool isRunning = false;

    public string serverIP = "127.0.0.1"; // 서버 IP 주소 (로컬 테스트이므로 localhost)
    public int serverPort = 8080;         // 서버 포트 번호
    public int localPort = 0; // 0이면 자동 할당.

    public GameData gameData;

    public Queue<string> inputQueue = new Queue<string>(); //action들을 넣기 위함. 
    public Queue<string> statusQueue = new Queue<string>(); //playerInput 형식의 데이터가 아닌 데이터를 처리하기 위함. 
    public Queue<string> timeQueue = new Queue<string>(); //playerInput 형식의 데이터가 아닌 데이터를 처리하기 위함.

    void Awake()
    {
        // Singleton 인스턴스 초기화
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬 전환 시 파괴되지 않음
        }
        else
        {
            Destroy(gameObject); // 중복 생성 방지
        }
    }

    public UdpClient GetUdpClient()
    {
        return udpClient; // udpClient 객체 반환
    }

    void Start()
    {
        // 클라이언트 초기화
        udpClient = new UdpClient(localPort);
        serverEndPoint = new IPEndPoint(IPAddress.Parse(serverIP), serverPort);

        // 서버로부터의 데이터 수신 시작
        isRunning = true;
        receiveThread = new Thread(new ThreadStart(ReceiveData));
        receiveThread.Start();
    }

    void OnDestroy()
    {
        // 스레드 및 UDP 클라이언트 종료
        isRunning = false;
        udpClient.Close();
        if (receiveThread != null)
            receiveThread.Abort();
    }

    // 서버로 메시지 전송
    public void SendMessageToServer(string message)
    {
        byte[] data = Encoding.UTF8.GetBytes(message);
        udpClient.Send(data, data.Length, serverEndPoint);
    }

    // 데이터 수신
    private void ReceiveData()
    {
        IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);

        while (isRunning)
        {
            try
            {
                byte[] data = udpClient.Receive(ref remoteEP);
                string message = Encoding.UTF8.GetString(data);
                Debug.Log($"[수신] {message}");

                // 수신된 메시지 처리
                HandleReceivedMessage(message);
            }
            catch (SocketException ex)
            {
                Debug.Log($"소켓 예외 발생: {ex.Message}");
                break;
            }
        }
    }

    // 수신된 메시지 처리
    private void HandleReceivedMessage(string message)
    {
        //Debug.Log("HandleReceiveMessage Called");
        // Unity 메인 스레드에서 실행되도록 큐에 추가
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            string[] parts = message.Split('|');
            string command = parts[0];
            Debug.Log(command);
            switch (command)
            {
                case "RoomCreated":

                    //Debug.Log("message received from server");
                    Debug.Log("방이 성공적으로 생성되었습니다.");
                    // 추가 로직 구현
                    gameData.playerId = 1;
                    //gameData.roomId = parts[1];
                    break;

                case "RoomJoined":
                    Debug.Log("방에 성공적으로 참여했습니다.");
                    gameData.playerId = 2;
                    //gameData.roomId = parts[1];
                    // 추가 로직 구현
                    break;

                case "GameStart":
                    Debug.Log("게임이 시작되었습니다!");
                    if (gameData.playerId == 1) {
                        SceneManager.LoadScene("MainScene_Multi");
                    }
                    else
                    {
                        SceneManager.LoadScene("MainScene_Multi_Enemy");
                    }
                    
                    // 게임 시작 로직 구현
                    break;

                case "PlayerInput":
                    Debug.Log("call player input function");
                    string jsonData = parts[1];
                    inputQueue.Enqueue(jsonData);
                    break;

                case "Broadcast":
                    Debug.Log("player 위치를 broadcast받았습니다."); 
                    string jsonDataPos = parts[1];
                    inputQueue.Enqueue(jsonDataPos);
                    break;

                case "HealthSync":
                    string playerId = parts[1];
                    string damage = parts[2];
                    string formattedHealth = $"{playerId}|{damage}";
                    statusQueue.Enqueue(formattedHealth);
                    break;

                case "TimerSync":
                    string serverTime = parts[1];
                    timeQueue.Enqueue(serverTime);
                    break;

                case "Error":
                    Debug.LogError($"에러: {parts[1]}");
                    break;

                default:
                    Debug.Log("알 수 없는 메시지입니다.");
                    break;
            }
        });
    }

    // 방 생성 요청
    public void CreateRoom(string roomId)
    {
        string message = $"CreateRoom|{roomId}";
        //Debug.Log("createRoom");    
        SendMessageToServer(message);
    }

    // 방 참여 요청
    public void JoinRoom(string roomId)
    {
        string message = $"JoinRoom|{roomId}";
        SendMessageToServer(message);
    }
}