using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using SG.GeneralLib;

namespace CommService
{
    static class CommServiceTCPIP
    {
        private static void DecodeRequest(string ip, byte[] data)
        {
            string sLogEntry = String.Format("[TCPIP]: RX: IP={0}; Len={1}; PKT={2}",
                ip, data.Length, BitConverter.ToString(data).Replace("-", " "));

            Global.WriteLogFile(sLogEntry);
        }

        /// <summary>
        /// If using the TCP/IP server with UDP make sure a maximum MTU size of approx 1450 bytes is used
        /// because at this point packet fragmentation can start occur then things get really messy, this
        /// could start to happen if large JSON packets are thrown down the pipe via UDP.
        /// </summary>
        public static void TCPIPThreadFunction()
        {
            string remoteAddress = string.Empty;
            IPAddress tcpAddress = IPAddress.Parse("127.0.0.1");
            UdpClient udpClient = null;
            TcpListener tcpClient = null;
            Socket tcpSocket = null;

            try
            {
                if (Global.svc.cfg.tcpipSvrProto == "udp")
                {
                    udpClient = new UdpClient(Global.svc.cfg.tcpipSvrPort);
                }
                else
                {
                    tcpClient = new TcpListener(tcpAddress, Global.svc.cfg.tcpipSvrPort);
                    tcpClient.Start();
                }

                while (Global.tcpipActive)
                {
                    if (Global.svc.cfg.tcpipSvrProto == "udp")
                    {
                        IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
                        byte[] receiveBytes = udpClient.Receive(ref RemoteIpEndPoint);
                        remoteAddress = "udp:" + RemoteIpEndPoint.ToString();
                        DecodeRequest(remoteAddress, receiveBytes);
                    }
                    else if (Global.svc.cfg.tcpipSvrProto == "tcp")
                    {
                        byte[] receiveBytes = new byte[2048];
                        tcpSocket = tcpClient.AcceptSocket();
                        int size = tcpSocket.Receive(receiveBytes);
                        Array.Resize(ref receiveBytes, size);
                        remoteAddress = "tcp:" + tcpSocket.RemoteEndPoint.ToString();
                        tcpSocket.Close();
                        DecodeRequest(remoteAddress, receiveBytes);
                    }
                    else
                    {
                        string error = "[TCPIP]: Invalid procotol specified in config.";
                        Global.tcpipActive = false;

                        Global.WriteEventLog(error);
                        Global.WriteLogFile(error);
                    }
                }
            }
            catch (Exception ex)
            {
                string error = string.Format("[TCPIP]: Exception: {0}", ex.Message);
                Global.tcpipActive = false;

                Global.WriteEventLog(error);
                Global.WriteLogFile(error);
            }
        }
    }
}
