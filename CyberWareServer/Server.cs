using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using static System.Console;

namespace CyberWareServer
{
    class Server
    {
        public static int MaxPlayers { get; protected set; }
        public static int Port { get; private set; }
        public static Dictionary<int,Client> clients = new Dictionary<int,Client>();
        public delegate void PacketHandler(int _fromClient, Packet _packet);
        public static Dictionary<int, PacketHandler> packetHandlers;

        private static TcpListener tcpListener;
        
        public static void Start(int _maxPlayers, int _port)
        {
            MaxPlayers = _maxPlayers;
            Port = _port;

            InitializeServerData();


            tcpListener = new TcpListener(IPAddress.Any, Port);
            tcpListener.Start();
            tcpListener.BeginAcceptTcpClient(TCPConnectCallback, null);

            Debug.Heading($"Server started on {_port}");
        }


        private static void TCPConnectCallback(IAsyncResult _result)
        {
            TcpClient _client = tcpListener.EndAcceptTcpClient(_result);
            tcpListener.BeginAcceptTcpClient(TCPConnectCallback, null);

            Debug.Log($"Client {_client.Client.RemoteEndPoint} is connecting...");


            for (int i = 1; i <= MaxPlayers; i++)
            {
                if(clients[i].tcp.socket == null)
                {
                    clients[i].tcp.Connect(_client);
                    return;
                }
            }

            Debug.ErrorLog($"{_client.Client.RemoteEndPoint} failed to connect. Reason [Server is full]");
        }

        private static void InitializeServerData()
        {
            for (int y = 1; y <= MaxPlayers;  y++)
            {
                clients.Add(y,new Client(y));
            }

            packetHandlers = new Dictionary<int, PacketHandler>()
            {
                {(int)ClientPackets.welcomeReceived, ServerHandle.WelcomeReceived }
            };

            Debug.Log("Initialize packets");
        }
    }
}
