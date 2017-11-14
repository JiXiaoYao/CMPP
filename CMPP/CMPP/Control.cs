using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using TCPCS;

namespace CMPP
{
    /// <summary>
    /// 
    /// </summary>
    public class Control
    {
        Server TcpServer;
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
            TcpServer = new Server(new IPAddress[] { ListenIP }, ListenPort, 4096, true);
            TcpServer.ClientConnent += NewConnet;
            TcpServer.ClientMessages += ClientMessage;
            TcpServer.ConnentStop += ConnetStop;
            TcpServer.StartListen();
        }
        #region 通信管道处理代码
        /// <summary>
        /// 收到一个新连接的处理方法
        /// </summary>
        /// <param name="ListenEndPoint"></param>
        /// <param name="SocketID"></param>
        public void NewConnet(IPEndPoint ListenEndPoint, long SocketID)
        {
            Thread.Sleep(1000 * 60);                                                               // 卡死线程一分钟
            if (HaveLogin.ContainsValue(ListenEndPoint.ToString() + "&" + SocketID))               // 如果登录
            {

            }
            else
            {
                if (TcpServer.SocketIPEndPointDict[ListenEndPoint].ContainsKey(SocketID))          // 如果没登录
                {
                    TcpServer.Send(ListenEndPoint, (int)SocketID, Encoding.UTF8.GetBytes(Packet.Encapsulation(Encoding.UTF8.GetBytes("Time Out"))));// 登录超时
                    TcpServer.ConnentStop(ListenEndPoint, SocketID);
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ListenEndPoint"></param>
        /// <param name="SocketID"></param>
        /// <param name="Context"></param>
        /// <param name="Legth"></param>
        public void ClientMessage(IPEndPoint ListenEndPoint, long SocketID, byte[] Context, int Legth)
        {
            string ContextMessage = Encoding.UTF8.GetString(Context, 0, Legth);                    // 转为字符串
            MessageBufferZone = MessageBufferZone + Context;                                       // 添加到缓冲区
            if (MessageBufferZone.Contains("<DataStart>") && MessageBufferZone.Contains("<DataEnd>"))// 确认缓冲区至少有一组完整的包体
            {
                string[] MessagePacket = MessageBufferZone.Split("<DataEnd>", StringSplitOptions.None);
                for (int i = 0; i < MessagePacket.Length - 1; i = i + 1)
                {
                    string PacketContext = MessagePacket[i] + "<DataEnd>";                         // 生成完整的一个数据包
                    byte[] MessageInfo = Packet.Unpack(PacketContext);                             // 拆包
                    string MessageContext = Encoding.UTF8.GetString(MessageInfo);                  // 恢复数据包 
                    if (MessageContext.Contains("},{"))
                    {
                        #region 映射通信
                        string[] MCtt = MessageContext.Split(new string[] { "},{" }, StringSplitOptions.None);
                        MCtt[0] = MCtt[0].Substring(1);
                        MCtt[4] = MCtt[4].Substring(0, MCtt[4].Length - 2);
                        // {监听的IP:Port},{SocketID},{运算符},{信息前缀修饰符},{信息}
                        if (MCtt[3] == "Info")
                        {
                            if (MCtt[2] == ">")
                            {

                            }
                            if (MCtt[2] == "<")
                            {

                            }
                        }
                        if (MCtt[2] == "*")
                        {
                            if (MCtt[4] == "Start")
                            {

                            }
                            else
                            {

                            }
                        }
                        #endregion
                    }
                    else
                    {
                        #region C/S通信
                        // : Longin User=Admin&Psw=Admin   返回：1:Longin 200 OK Null      2:Longin 400 Error Info=密码错误
                        // : Select Map TCP                     返回：Admin Map List IPEndPoint={x.x.x.x:xxx},{y.y.y.y:yyy}
                        // : Map TCP Null IPEndPoint={x.x.x.x:xxx},{y.y.y.y:yyy},{z.z.z.z:zzz}     返回：Map Return Null Info=“x.x.x.x:xxx”成功 “y.y.y.y:yyy”成功 “z.z.z.z:zzz”失败：原因，无权限
                        string[] MCtt = MessageContext.Split(' ');
                        if (MCtt[0] == "Login")
                        {
                            if (MCtt[1].Split("=")[0] == "User" && MCtt[1].Split("=")[1].Split("&")[1] == "Psw")
                            {
                                string User = MCtt[1].Split("&")[0].Split("=")[1];
                                string Psw = MCtt[1].Split("&")[1].Split("=")[1];
                                if (User == Psw)
                                {
                                    TcpServer.Send(ListenEndPoint, (int)SocketID, Encoding.UTF8.GetBytes(Packet.Encapsulation(Encoding.UTF8.GetBytes("Longin 200 OK"))));
                                }
                                else
                                {
                                    TcpServer.Send(ListenEndPoint, (int)SocketID, Encoding.UTF8.GetBytes(Packet.Encapsulation(Encoding.UTF8.GetBytes("Longin 400 Error Info=密码错误"))));
                                    TcpServer.Close(ListenEndPoint, (int)SocketID);
                                }
                            }
                        }
                        if (MCtt[0] == "Select")
                        {

                        }
                        if (MCtt[0] == "Map")
                        {

                        }
                        #endregion
                    }
                }
                MessageBufferZone = MessagePacket[MessagePacket.Length - 1];
            }
        }
        public void ConnetStop(IPEndPoint Listen, long SocketID)
        {

        }
        #endregion
    }
}
