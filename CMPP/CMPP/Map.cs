using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TCPCS;
using System.Net;
using System.Net.Sockets;

namespace CMPP
{
    class Map
    {
        Server TcpServer;
        Dictionary<IPEndPoint, string> MapListDict = new Dictionary<IPEndPoint, string>();
        
        public delegate bool SelectUserAndPasswordDelegate(string User, string Password);
        public SelectUserAndPasswordDelegate SelectUserAndPassword;
        #region 构造函数    
        public Map(IPAddress Communication, int Port)
        {
            TcpServer = new Server(new IPAddress[] { Communication }, Port, 4096, true);
            TcpServer.StartListen();
            TcpServer.ClientConnent += NewConnet;
            TcpServer.ClientMessages += ClientMessage;
            TcpServer.ConnentStop += ConnetStop;
        }
        #endregion
        #region 通信管道处理代码
        public void NewConnet(IPEndPoint Listen, long SocketID)
        {

        }
        public void ClientMessage(IPEndPoint Listen, long SocketID, byte[] Context, int Legth)
        {

        }
        public void ConnetStop(IPEndPoint Listen, long SocketID)
        {

        }
        #endregion

        public void AddMap(IPEndPoint ListenEndPoint, string User, MapNetMode Mode)
        {
            if (Mode == MapNetMode.TCP)
            {
                MapListDict.Add(ListenEndPoint, User);
            }
        }
    }
    enum MapNetMode
    {
        TCP, UDP, TCPUDP
    }
}
