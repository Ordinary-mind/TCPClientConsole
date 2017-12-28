using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//添加新的命名空间
using System.Net;
using System.Net.Sockets;
using System.IO;              //流StreamReader
using System.Threading;

namespace TCPClientConsole
{
    public class StateObject
    {
        public static ManualResetEvent connectDone = new ManualResetEvent(false);
        public static ManualResetEvent sendDone = new ManualResetEvent(false);
        public static ManualResetEvent receiveDone = new ManualResetEvent(false);

        public Socket workSocket = null;
        public const int BufferSize = 256;
        public byte[] buffer = new byte[BufferSize];
        public StringBuilder sb = new StringBuilder();

        static void Main(String[] args)
        {
            IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
            IPEndPoint remoteEP = new IPEndPoint(ipAddress, 20170);   
            Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            Connect(remoteEP, client);
            Receive(client);
            Send(client, "");   
            

            Console.ReadLine();
        }

        public static void Connect(EndPoint remoteEP,Socket client)
        {
            client.BeginConnect(remoteEP, new AsyncCallback(ConnectCallback), client);
            //connectDone.WaitOne();
        }

        private static void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                Socket client = (Socket)ar.AsyncState;
                client.EndConnect(ar);
                Console.WriteLine("Socket connect to {0}" + client.RemoteEndPoint.ToString());
                //connectDone.Set();
            }catch(Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private static void Send(Socket client,String data)
        {
            while (true)
            {
                data = Console.ReadLine();
                byte[] byteData = Encoding.ASCII.GetBytes(data);
                client.BeginSend(byteData, 0, byteData.Length, SocketFlags.None, new AsyncCallback(SendCallback), client);
            }
            //sendDone.WaitOne();
        }

        private static void SendCallback(IAsyncResult ar)
        {
            try
            {
                Socket client = (Socket)ar.AsyncState;
                int byteSent = client.EndSend(ar);
                Thread.Sleep(100);
                Send(client, "");
                //sendDone.Set();
            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private static void Receive(Socket client)
        {
            try
            {
                StateObject state = new StateObject();
                state.workSocket = client;
                client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReceiveCallback), state);
               // receiveDone.WaitOne();
            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private static void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                StateObject state = (StateObject)ar.AsyncState;
                Socket client = state.workSocket;
                int byteRead = client.EndReceive(ar);
                if (byteRead > 0)
                {
                    String str = Encoding.ASCII.GetString(state.buffer, 0, byteRead);
                    Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")+" Response:"+ str);
                    client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReceiveCallback), state);
                }
            }catch(Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}