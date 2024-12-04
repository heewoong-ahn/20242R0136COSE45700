using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections.Generic;
using System.Threading;

class UDPServer
{

    private static UdpClient udpServer;
    private static Dictionary<string, Room> rooms = new Dictionary<string, Room>();
    private static List<ClientInfo> clients = new List<ClientInfo>();
    private static int serverPort = 8080;

    static void Main(string[] args)
    {
        udpServer = new UdpClient(serverPort);
        Console.WriteLine($"UDP ���� ���� �� (��Ʈ {serverPort})");

        Thread receiveThread = new Thread(ReceiveClients);
        receiveThread.Start();

        Console.WriteLine("�����Ϸ��� Enter Ű�� ��������...");
        Console.ReadLine();

        udpServer.Close();
    }

    static void ReceiveClients()
    {
        //��� ip�κ��� ��û�� ����ϰ�, 0 ->�˾Ƽ� ������ ��� ������ port�� ��Ī������. 
        IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);

        while (true)
        {
            try
            {
                byte[] data = udpServer.Receive(ref remoteEP);
                string message = Encoding.UTF8.GetString(data);
                Console.WriteLine($"����: {message} (���� ���: {remoteEP})");

                HandleMessage(message, remoteEP);
            }
            catch (SocketException ex)
            {
                Console.WriteLine($"���� ���� �߻�: {ex.Message}");
                break;
            }
        }
    }

    static void HandleMessage(string message, IPEndPoint clientEP)
    {
        // �޽����� '|'�� �и��Ͽ� ��ɰ� �����͸� ����
        string[] parts = message.Split('|');
        string command = parts[0];

        // Ŭ���̾�Ʈ ���� �������� �Ǵ� ����
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

            // �߰����� ��� ó�� ����

            default:
                SendResponse(client.EndPoint, "Error|�� �� ���� ����Դϴ�.");
                break;
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
            Console.WriteLine($"{client.EndPoint}���� �� {roomId}�� �����߽��ϴ�.");

            SendResponse(client.EndPoint, $"RoomCreated|{roomId}|���� �����Ǿ����ϴ�.");
        }
        else
        {
            SendResponse(client.EndPoint, "Error|�̹� �����ϴ� �� ��ȣ�Դϴ�.");
        }
    }

    static void HandleJoinRoom(ClientInfo client, string roomId)
    {
        if (rooms.ContainsKey(roomId))
        {
            Room room = rooms[roomId];
            if (room.Clients.Count < 2)
            {
                room.Clients.Add(client);
                client.RoomId = roomId;
                Console.WriteLine($"{client.EndPoint}���� �� {roomId}�� �����߽��ϴ�.");

                SendResponse(client.EndPoint, $"RoomJoined|{roomId}|�濡 �����߽��ϴ�.");

                if (room.Clients.Count == 2)
                {
                    // �� ���� �÷��̾ �����ϸ� ���� ���� �޽��� ����
                    foreach (var c in room.Clients)
                    {
                        SendResponse(c.EndPoint, $"GameStart|{roomId}|������ �����մϴ�.");
                    }
                }
            }
            else
            {
                SendResponse(client.EndPoint, "Error|�濡 �̹� �� ���� �÷��̾ �ֽ��ϴ�.");
            }
        }
        else
        {
            SendResponse(client.EndPoint, "Error|�������� �ʴ� �� ��ȣ�Դϴ�.");
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
    public IPEndPoint EndPoint { get; set; } // Ŭ���̾�Ʈ�� IP�� ��Ʈ
    public string RoomId { get; set; }       // Ŭ���̾�Ʈ�� ���� �� ID
}

public class Room
{
    public string RoomId { get; set; }                 // �� ID
    public List<ClientInfo> Clients { get; set; }      // �濡 ������ Ŭ���̾�Ʈ ���
    
    //������
    public Room(string roomId)
    {
        RoomId = roomId;
        Clients = new List<ClientInfo>();
    }
}

