using System;
using System.Collections.Generic;
using System.Text;

namespace CyberWareServer
{
    class ServerHandle
    {

        public static void WelcomeReceived(int _fromClient, Packet _packet)
        {
            int _clientIdCheck = _packet.ReadInt();
            string _username = _packet.ReadString();

            Debug.AlertLog($" \"{_username}\"  - [ID: {_fromClient}] [IP:{Server.clients[_fromClient].tcp.socket.Client.RemoteEndPoint}] has connected succesfully");
            if(_fromClient != _clientIdCheck)
            {
                Debug.ErrorLog($"Player \"{_username}\" [ID: { _fromClient} ] has assumed the wrong client ID ({_clientIdCheck})");
            }


            //napojit hráče DO EPICKÉ hry(ale kam? co je to hra? PANIKAA aaÁáÁA)
        }

        public static void UDPTestReceived(int _fromClient, Packet _packet)
        {
            string _msg = _packet.ReadString();
            Debug.AlertLog($"[UDP]Player{_fromClient} : {_msg}");
        }
    }
}
