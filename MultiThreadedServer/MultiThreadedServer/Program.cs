using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MultiThreadedServer
{
    public class Program
    {
        public const int Port = 9999;

        public static void Main(string[] args)
        {
            Run();
        }

        public static void Run()
        {
            TcpListener server = new TcpListener(IPAddress.Any, Port);
            server.Start();

            while (true)
            {
                TcpClient client = server.AcceptTcpClient();

                NetworkStream ns = client.GetStream();

                byte[] hello = new byte[100];
                hello = Encoding.Default.GetBytes("Hello World");

                ns.Write(hello, 0, hello.Length);

                while (client.Connected)
                {
                    byte[] msg = new byte[1024];
                    ns.Read(msg, 0, msg.Length);
                    Console.WriteLine(Encoding.Default.GetString(msg));
                }
            }
        }
    }
}