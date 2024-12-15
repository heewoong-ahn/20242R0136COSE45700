using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections.Generic;
using System.Threading;
using Newtonsoft.Json;

namespace GameServer
{
    class UDPServer
    {

        private static UdpClient udpServer;
        private static Dictionary<string, Room> rooms = new Dictionary<string, Room>();
        private static List<ClientInfo> clients = new List<ClientInfo>();
        private static int serverPort = 8080;

        static void Main(string[] args)
        {
            udpServer = new UdpClient(serverPort);
            Console.WriteLine($"UDP 서버 실행 중 (포트 {serverPort})");

            Thread receiveThread = new Thread(ReceiveClients);
            receiveThread.Start();

            Console.WriteLine("종료하려면 Enter 키를 누르세요...");
            Console.ReadLine();

            //broadcastTimer.Dispose();
            udpServer.Close();
        }

        static void ReceiveClients()
        {
            //모든 ip로부터 요청을 허용하고, 0 ->알아서 서버상 사용 가능한 port랑 매칭시켜줌. 
            IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);

            while (true)
            {
                try
                {
                    byte[] data = udpServer.Receive(ref remoteEP);
                    string message = Encoding.UTF8.GetString(data);
                    Console.WriteLine($"수신: {message} (보낸 사람: {remoteEP})");

                    HandleMessage(message, remoteEP);
                }
                catch (SocketException ex)
                {
                    Console.WriteLine($"소켓 예외 발생: {ex.Message}");
                    break;
                }
            }
        }

        static object lockObj = new object();
        static void HandleMessage(string message, IPEndPoint clientEP)
        {
            // 메시지를 '|'로 분리하여 명령과 데이터를 추출
            string[] parts = message.Split('|');
            string command = parts[0];

            lock (lockObj)
            {
                // 클라이언트 정보 가져오기 또는 생성
                ClientInfo client = clients.Find(c => c.EndPoint.Equals(clientEP));
                if (client == null)
                {
                    client = new ClientInfo
                    {
                        EndPoint = clientEP,
                        RoomId = null
                    };
                    clients.Add(client);
                }

                switch (command)
                {
                    case "CreateRoom":
                        if (parts.Length >= 2)
                        {
                            string roomId = parts[1];
                            HandleCreateRoom(client, roomId);
                        }
                        break;

                    case "JoinRoom":
                        if (parts.Length >= 2)
                        {
                            string roomId = parts[1];
                            HandleJoinRoom(client, roomId);
                        }
                        break;

                    // 추가적인 명령 처리 예정

                    case "PlayerInput":
                        if (parts.Length >= 2)
                        {
                            string jsonData = parts[1];
                            HandlePlayerInput(client, jsonData);
                        }
                        break;

                    default:
                        SendResponse(client.EndPoint, "Error|알 수 없는 명령입니다.");
                        break;
                }
            }
            
        }

        static void HandleCreateRoom(ClientInfo client, string roomId)
        {
            if (!rooms.ContainsKey(roomId))
            {
                Room room = new Room(roomId);
                room.Clients.Add(client);
                rooms.Add(roomId, room);
                client.RoomId = roomId;
                client.PlayerInput.playerId = 1;
                client.PlayerInput.position = new Vector3(-0.8517556f, 1.3f, -1.216699f);
                // client.PlayerId = 1; //방을 만든 사람은 1 
                Console.WriteLine($"{client.EndPoint}님이 방 {roomId}을 생성했습니다.");

                SendResponse(client.EndPoint, $"RoomCreated|{roomId}|방이 생성되었습니다.");
            }
            else
            {
                SendResponse(client.EndPoint, "Error|이미 존재하는 방 번호입니다.");
            }
        }

        private static Timer broadcastTimer;

        static void HandleJoinRoom(ClientInfo client, string roomId)
        {
            if (rooms.ContainsKey(roomId))
            {
                Room room = rooms[roomId];
                if (room.Clients.Count < 2)
                {
                    room.Clients.Add(client);
                    client.RoomId = roomId;
                    client.PlayerInput.playerId = 2;
                    client.PlayerInput.position = new Vector3(-0.47f, 1.3f, 3f);
                    // client.PlayerId = 2; //방에 들어간 사람은 2
                    Console.WriteLine($"{client.EndPoint}님이 방 {roomId}에 참여했습니다.");

                    SendResponse(client.EndPoint, $"RoomJoined|{roomId}|방에 참여했습니다.");

                    if (room.Clients.Count == 2)
                    {
                        // 두 명의 플레이어가 참여하면 게임 시작 메시지 전송
                        foreach (var c in room.Clients)
                        {
                            SendResponse(c.EndPoint, $"GameStart|{roomId}|게임을 시작합니다.");
                        }
                        // 게임 시작 후에 타이머 시작
                        broadcastTimer = new Timer(BroadcastAllRooms, null, 3100, 100);
                        Console.WriteLine("위치 동기화 브로드캐스트 시작.");
                    }
                }
                else
                {
                    SendResponse(client.EndPoint, "Error|방에 이미 두 명의 플레이어가 있습니다.");
                }
            }
            else
            {
                SendResponse(client.EndPoint, "Error|존재하지 않는 방 번호입니다.");
            }
        }

        static void HandlePlayerInput(ClientInfo client, string jsonData)
        {
            if (client.RoomId != null && rooms.ContainsKey(client.RoomId))
            {
                Room room = rooms[client.RoomId];

                try
                {
                    client.PlayerInput = JsonConvert.DeserializeObject<PlayerInputData>(jsonData);
                }
                catch (Exception ex)
                {
                    SendResponse(client.EndPoint, $"Error|JSON 역직렬화 오류: {ex.Message}");
                    return;
                }

                //client.Position = inputData.position; //서버상의 client의 위치 업데이트 

                foreach (var c in room.Clients)
                {

                    SendResponse(c.EndPoint, $"PlayerInput|{jsonData}");

                }
            }
        }

        static void BroadcastAllRooms(object state)
        {
            lock (lockObj)
            {
                try
                {
                    foreach (var room in rooms.Values)
                    {
                        foreach (var client in room.Clients)
                        {
                            if (client.PlayerInput == null) continue;

                            client.PlayerInput.action = "SyncPos";
                            string jsonData = JsonConvert.SerializeObject(client.PlayerInput);

                            foreach (var otherClient in room.Clients)
                            {
                                if (!otherClient.Equals(client))
                                {
                                    SendResponse(otherClient.EndPoint, $"Broadcast|{jsonData}");
                                    Console.WriteLine($"위치동기화: {client.PlayerInput.playerId} 의 정보를 {otherClient.PlayerInput.playerId}에게로");
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("BroadcastAllRooms 예외 발생: " + ex.Message);
                }
            }
          
        }

        static void SendResponse(IPEndPoint clientEP, string message)
        {
            byte[] responseData = Encoding.UTF8.GetBytes(message);
            udpServer.Send(responseData, responseData.Length, clientEP);
        }
    }

    public class ClientInfo
    {
        public IPEndPoint EndPoint { get; set; } // 클라이언트의 IP와 포트
        public string RoomId { get; set; }       // 클라이언트가 속한 방 ID
        //public Vector3 Position { get; set; }     // 클라이언트의 현재 위치
                                                  // public int PlayerId { get; set; }        // 플레이어 ID (1 또는 2)

        public PlayerInputData PlayerInput { get; set;  }

        public ClientInfo()
        {
            PlayerInput = new PlayerInputData();
        }



    }

    public class Room
    {
        public string RoomId { get; set; }                 // 방 ID
        public List<ClientInfo> Clients { get; set; }      // 방에 참여한 클라이언트 목록

        //생성자
        public Room(string roomId)
        {
            RoomId = roomId;
            Clients = new List<ClientInfo>();
        }
    }

    public struct Vector3
    {
        public float x;
        public float y;
        public float z;

        public Vector3(float X, float Y, float Z)
        {
            x = X;
            y = Y;
            z = Z;
        }
    }

    // 클라이언트로부터 받은 입력 데이터 클래스
    public class PlayerInputData
    {
        public int playerId { get; set; }          // 플레이어 ID
        public string action { get; set; }         // 액션 (예: "MoveForward")
        public Vector3 position { get; set; }      // 현재 위치
        //public Vector3 MoveDirection { get; set; } // 이동 방향
        //public float Speed { get; set; }           // 이동 속도
        public float timestamp { get; set; }        // 클라이언트에서 보낸 시간 (밀리초)

        public PlayerInputData()
        {
            playerId = 0;
            action = "Idle";
            position = new Vector3(0, 0, 0);
            timestamp = 0f;
        }
    }       
}





