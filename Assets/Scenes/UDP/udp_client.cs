using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine.SceneManagement;
using System.Collections.Generic; // Queue<>�� ��Ÿ ���׸� �÷�


public class UDPClient : MonoBehaviour
{
    
    public static UDPClient Instance; //�̱������� ������ ���� ������ udpclient�� playercontroller����� ����� ������ �ѱ�. 
    private UdpClient udpClient;

    private IPEndPoint serverEndPoint;
    private Thread receiveThread;
    private bool isRunning = false;

    public string serverIP = "127.0.0.1"; // ���� IP �ּ� (���� �׽�Ʈ�̹Ƿ� localhost)
    public int serverPort = 8080;         // ���� ��Ʈ ��ȣ
    public int localPort = 0; // 0�̸� �ڵ� �Ҵ�.

    public GameData gameData;

    public Queue<string> inputQueue = new Queue<string>(); //action���� �ֱ� ����. 
    public Queue<string> statusQueue = new Queue<string>(); //playerInput ������ �����Ͱ� �ƴ� �����͸� ó���ϱ� ����. 
    public Queue<string> timeQueue = new Queue<string>(); //playerInput ������ �����Ͱ� �ƴ� �����͸� ó���ϱ� ����.

    void Awake()
    {
        // Singleton �ν��Ͻ� �ʱ�ȭ
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // �� ��ȯ �� �ı����� ����
        }
        else
        {
            Destroy(gameObject); // �ߺ� ���� ����
        }
    }

    public UdpClient GetUdpClient()
    {
        return udpClient; // udpClient ��ü ��ȯ
    }

    void Start()
    {
        // Ŭ���̾�Ʈ �ʱ�ȭ
        udpClient = new UdpClient(localPort);
        serverEndPoint = new IPEndPoint(IPAddress.Parse(serverIP), serverPort);

        // �����κ����� ������ ���� ����
        isRunning = true;
        receiveThread = new Thread(new ThreadStart(ReceiveData));
        receiveThread.Start();
    }

    void OnDestroy()
    {
        // ������ �� UDP Ŭ���̾�Ʈ ����
        isRunning = false;
        udpClient.Close();
        if (receiveThread != null)
            receiveThread.Abort();
    }

    // ������ �޽��� ����
    public void SendMessageToServer(string message)
    {
        byte[] data = Encoding.UTF8.GetBytes(message);
        udpClient.Send(data, data.Length, serverEndPoint);
    }

    // ������ ����
    private void ReceiveData()
    {
        IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);

        while (isRunning)
        {
            try
            {
                byte[] data = udpClient.Receive(ref remoteEP);
                string message = Encoding.UTF8.GetString(data);
                Debug.Log($"[����] {message}");

                // ���ŵ� �޽��� ó��
                HandleReceivedMessage(message);
            }
            catch (SocketException ex)
            {
                Debug.Log($"���� ���� �߻�: {ex.Message}");
                break;
            }
        }
    }

    // ���ŵ� �޽��� ó��
    private void HandleReceivedMessage(string message)
    {
        //Debug.Log("HandleReceiveMessage Called");
        // Unity ���� �����忡�� ����ǵ��� ť�� �߰�
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            string[] parts = message.Split('|');
            string command = parts[0];
            Debug.Log(command);
            switch (command)
            {
                case "RoomCreated":

                    //Debug.Log("message received from server");
                    Debug.Log("���� ���������� �����Ǿ����ϴ�.");
                    // �߰� ���� ����
                    gameData.playerId = 1;
                    //gameData.roomId = parts[1];
                    break;

                case "RoomJoined":
                    Debug.Log("�濡 ���������� �����߽��ϴ�.");
                    gameData.playerId = 2;
                    //gameData.roomId = parts[1];
                    // �߰� ���� ����
                    break;

                case "GameStart":
                    Debug.Log("������ ���۵Ǿ����ϴ�!");
                    if (gameData.playerId == 1) {
                        SceneManager.LoadScene("MainScene_Multi");
                    }
                    else
                    {
                        SceneManager.LoadScene("MainScene_Multi_Enemy");
                    }
                    
                    // ���� ���� ���� ����
                    break;

                case "PlayerInput":
                    Debug.Log("call player input function");
                    string jsonData = parts[1];
                    inputQueue.Enqueue(jsonData);
                    break;

                case "Broadcast":
                    Debug.Log("player ��ġ�� broadcast�޾ҽ��ϴ�."); 
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
                    Debug.LogError($"����: {parts[1]}");
                    break;

                default:
                    Debug.Log("�� �� ���� �޽����Դϴ�.");
                    break;
            }
        });
    }

    // �� ���� ��û
    public void CreateRoom(string roomId)
    {
        string message = $"CreateRoom|{roomId}";
        //Debug.Log("createRoom");    
        SendMessageToServer(message);
    }

    // �� ���� ��û
    public void JoinRoom(string roomId)
    {
        string message = $"JoinRoom|{roomId}";
        SendMessageToServer(message);
    }
}