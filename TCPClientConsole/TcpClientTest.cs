using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace TCPClientConsole
{
    class TcpClientTest
    {
        private Thread sendThread;
        private Thread recvThread;
        private NetworkStream stream;
        private StreamReader sr;
        private StreamWriter sw;

        public void ConnectServer()
        {
            IPAddress myIP = IPAddress.Parse("127.0.0.1");
            //构造一个TcpClient类对象,TCP客户端
            TcpClient client = new TcpClient();
            //与TCP服务器连接
            client.Connect(myIP, 20170);
            Console.WriteLine("服务器已经连接...请输入对话内容...");
            sendThread = new Thread(new ThreadStart(sendMsg));
            sendThread.Start();
            recvThread = new Thread(new ThreadStart(recvMsg));
            recvThread.Start();
            stream = client.GetStream();
            sr = new StreamReader(stream);
            sw = new StreamWriter(stream);
        }

        private void recvMsg()
        {
            while (true)
            {
                String readString = sr.ReadLine();
                if (readString != null)
                {
                    Console.WriteLine("服务器：" + readString);
                    sw.Flush();
                }
            }
        }

        private void sendMsg()
        {
            while (true)
            {
                String str = Console.ReadLine();
                if (!String.IsNullOrEmpty(str))
                {
                    //Console.WriteLine("客户端：" + str);
                    sw.WriteLine(str);
                    sw.Flush();
                }
            }
        }
    }
}
