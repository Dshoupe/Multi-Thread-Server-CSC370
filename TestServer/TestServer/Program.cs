using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TestServer.Models;

namespace TestServer
{
    public static class Program
    {
        public static int waitTimeMin = 500;
        public static int waitTimeMax = 1000;
        public static int writeTimeMin = 500;
        public static int writeTimeMax = 1000;
        public static int TotalCustomer = 1;
        public static int TotalNumOfCustomer = 50;
        public static List<string> CustomerNames = new List<string>();
        public static Semaphore TakeNumberSemaphore;
        public static Random randy = new Random();
        public static int requestCount = 0;

        public static void Main(string[] args)
        {
            TakeNumberSemaphore = new Semaphore(TotalNumOfCustomer, TotalNumOfCustomer);

            IPHostEntry ipHostInfo = Dns.Resolve(Dns.GetHostName());
            IPAddress ipAddress = ipHostInfo.AddressList[0];
            IPAddress ipAddressLocal = new IPAddress(new byte[] { 127, 0, 0, 1 });
            IPEndPoint localEndPoint = new IPEndPoint(ipAddressLocal, 8000);
            //Console.WriteLine(ipAddress);
            Console.WriteLine(ipAddressLocal);

            Socket listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            listener.Bind(localEndPoint);
            listener.Listen(100);

            Console.WriteLine("Waiting for a connection...");
            Socket handler = listener.Accept();
            String data = "";
            byte[] bytes = new byte[1024];
            while (!data.Contains("exit"))
            {
                bytes = new byte[1024];
                int bytesRec = handler.Receive(bytes);
                data += Encoding.ASCII.GetString(bytes, 0, bytesRec);

                Console.WriteLine("Text received : {0}", data);
            }
            byte[] msg = Encoding.ASCII.GetBytes(data);
            handler.Send(msg);
            handler.Shutdown(SocketShutdown.Both);
            handler.Close();
        }

        public static void GrabNumber(Person p)
        {
            TakeNumberSemaphore.WaitOne();
            p.Number = TotalCustomer++;
            Thread.Sleep(randy.Next(500, 1000));
            Console.WriteLine($"{p.Name} has taken number {p.Number}");
            TakeNumberSemaphore.Release();
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void WriteName(Person p)
        {
            CustomerNames.Add(p.Name);
            Thread.Sleep(randy.Next(500, 1000));
            Console.WriteLine($"{p.Name} has added their name to the list");
        }
    }
}
