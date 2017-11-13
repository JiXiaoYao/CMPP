using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using TCPCS;

namespace CMPP
{
    public class Control
    {
        TCPCS.Server TcpServer;
        Dictionary<string, string> HaveLogin = new Dictionary<string, string>();                   // kyes:User     Value:IP:Port&SocketID
        Dictionary<IPEndPoint, long> ConnetListDict = new Dictionary<IPEndPoint, long>();
        string MessageBufferZone;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="ListenIP"></param>
        /// <param name="ListenPort"></param>
        public Control(IPAddress ListenIP, int ListenPort)
        {
            TcpServer = new TCPCS.Server(new IPAddress[] { ListenIP }, ListenPort, 4096, true);
            TcpServer.StartListen();
            TcpServer.ClientConnent += NewConnet;
            TcpServer.ClientMessages += ClientMessage;
            TcpServer.ConnentStop += ConnetStop;
        }
        #region 通信管道处理代码
        public void NewConnet(IPEndPoint ListenEndPoint, long SocketID)
        {
            Thread.Sleep(1000 * 60);
            if (HaveLogin.ContainsValue(ListenEndPoint.ToString() + "&" + SocketID))
            {

            }
            else
            {
                if (TcpServer.SocketIPEndPointDict[ListenEndPoint].ContainsKey(SocketID))
                {
                    TcpServer.Send(ListenEndPoint, (int)SocketID, Encoding.UTF8.GetBytes(Packet.Encapsulation(Encoding.UTF8.GetBytes("Time Out"))));
                    TcpServer.ConnentStop(ListenEndPoint, SocketID);
                }
            }
        }
        public void ClientMessage(IPEndPoint Listen, long SocketID, byte[] Context, int Legth)
        {
            string ContextMessage = Encoding.UTF8.GetString(Context,0,Legth);
            MessageBufferZone = MessageBufferZone + Context;
            if (MessageBufferZone.Contains("<DataStart>") && MessageBufferZone.Contains("<DataEnd>"))
            {

            }
        }
        public void ConnetStop(IPEndPoint Listen, long SocketID)
        {

        }
        #endregion
    }
}
