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
        private static UdpClient udpListener;

        public static void Start(int _maxPlayers, int _port)
        {
            MaxPlayers = _maxPlayers;
            Port = _port;

            InitializeServerData();


            tcpListener = new TcpListener(IPAddress.Any, Port);
            tcpListener.Start();
            tcpListener.BeginAcceptTcpClient(TCPConnectCallback, null);

            udpListener = new UdpClient(Port);
            udpListener.BeginReceive(UDPReceiveCallback, null);

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


        private static void UDPReceiveCallback(IAsyncResult _result)
        {
            try
            {
                IPEndPoint _clientEndPoint = new IPEndPoint(IPAddress.Any, 0);
                byte[] _data = udpListener.EndReceive(_result, ref _clientEndPoint);
                udpListener.BeginReceive(UDPReceiveCallback, null);

                if(_data.Length < 4)
                {
                    return;
                }

                using(Packet _packet = new Packet(_data))
                {
                    int _clientId = _packet.ReadInt();

                    if(_clientId == 0)
                    {
                        return;
                    }

                    if(clients[_clientId].udp.endPoint == null)
                    {
                        clients[_clientId].udp.Connect(_clientEndPoint);
                        return;
                    }

                    if(clients[_clientId].udp.endPoint.ToString() == _clientEndPoint.ToString())
                    {
                        clients[_clientId].udp.HandleData(_packet);
                    }
                }

            }
            catch (Exception _exception)
            {
                Debug.ErrorLog($"[UDP] Error receiving data: {_exception}");
            }


        }

        public static void SendUDPData(IPEndPoint _clientEndPoint, Packet _packet)
        {
            try
            {
                if(_clientEndPoint != null)
                {
                    udpListener.BeginSend(_packet.ToArray(), _packet.Length(), _clientEndPoint, null, null);
                }

            } catch(Exception _exception)
            {
                Debug.ErrorLog($"[UDP] Error sending data to {_clientEndPoint} : {_exception}");
            }
        }

        private static void InitializeServerData()
        {
            for (int y = 1; y <= MaxPlayers;  y++)
            {
                clients.Add(y,new Client(y));
            }

            packetHandlers = new Dictionary<int, PacketHandler>()
            {
                {(int)ClientPackets.welcomeReceived, ServerHandle.WelcomeReceived },
                {(int)ClientPackets.udpTestReceived, ServerHandle.UDPTestReceived }
            };

            Debug.Log("Initialize packets");
        }
    }
}
