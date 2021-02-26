using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using static System.Console;

namespace CyberWareServer
{
    class Client
    {

        public static int dataBufferSize = 4096;

        public int id;
        public TCP tcp;

        public Client(int _clientId)
        {
            id = _clientId;
            tcp = new TCP(id);
        }
        public class TCP
        {
            public TcpClient socket;

            private readonly int id;
            private NetworkStream stream;
            private byte[] receiveBuffer;
            private Packet receivedData;

            public TCP(int _id)
            {
                id = _id;
            }

            public void Connect(TcpClient _socket)
            {
                socket = _socket;
                socket.ReceiveBufferSize = dataBufferSize;
                socket.SendBufferSize = dataBufferSize;

                stream = socket.GetStream();
                receiveBuffer = new byte[dataBufferSize];
                receivedData = new Packet();

                stream.BeginRead(receiveBuffer,0,dataBufferSize, ReceiveCallback, null);

                ServerSend.Welcome(id, "Tvoje mama kekW");
            }

            public void SendData(Packet _packet)
            {
                try
                {
                  if(socket != null)
                    {
                        stream.BeginWrite(_packet.ToArray(), 0, _packet.Length(), null, null);
                    }
                }
                catch (Exception _exception)
                {
                    Debug.ErrorLog($"Error sending data to player {id}: {_exception}");
                }
            }

            private void ReceiveCallback(IAsyncResult _result)
            {
                try
                {
                    int _byteLength = stream.EndRead(_result);
                    if(_byteLength <= 0)
                    {
                        // tady bude disconegr teda co
                        return;
                    }

                    byte[] _data = new byte[_byteLength];
                    Array.Copy(receiveBuffer, _data, _byteLength);

                    receivedData.Reset(HandleData(_data));
                    stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
                }
                catch(Exception _exception)
                {
                    Debug.ErrorLog($"Error receiving data: {_exception}");

                    // tady taky ten jeden negr.. teda disconnect
                }
            }

            private bool HandleData(byte[] _data)
            {
                int _packetLenght = 0;

                receivedData.SetBytes(_data);

                if (receivedData.UnreadLength() >= 4)
                {
                    _packetLenght = receivedData.ReadInt();
                    if (_packetLenght <= 0)
                    {
                        return true;
                    }
                }

                while (_packetLenght > 0 && _packetLenght <= receivedData.UnreadLength())
                {
                    byte[] _packetBytes = receivedData.ReadBytes(_packetLenght);
                    ThreadManager.ExecuteOnMainThread(() =>
                    {
                        using (Packet _packet = new Packet(_packetBytes))
                        {
                            int _packetId = _packet.ReadInt();
                            Server.packetHandlers[_packetId](id,_packet);
                        }
                    });

                    _packetLenght = 0;
                    if (receivedData.UnreadLength() >= 4)
                    {
                        _packetLenght = receivedData.ReadInt();
                        if (_packetLenght <= 0)
                        {
                            return true;
                        }
                    }
                }

                if (_packetLenght <= 1)
                {
                    return true;
                }

                return false;
            }
        }
    }
}
