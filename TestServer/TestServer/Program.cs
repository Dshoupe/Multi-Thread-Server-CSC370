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
        public const int port = 8000;
        public static int TotalCustomer = 1;
        public static int TotalNumOfCustomer = 50;
        public static List<string> CustomerNames = new List<string>();
        public static Semaphore TakeNumberSemaphore;
        public static Random randy = new Random();
        public static int requestCount = 0;
        public static TcpListener server = null;

        public static void Main(string[] args)
        {
            TakeNumberSemaphore = new Semaphore(TotalNumOfCustomer, TotalNumOfCustomer);
            try
            {
                // Set the TcpListener on port 13000.
                Int32 port = 8000;
                IPAddress localAddr = IPAddress.Parse("127.0.0.1");

                // TcpListener server = new TcpListener(port);
                server = new TcpListener(localAddr, port);

                // Start listening for client requests.
                server.Start();

                // Buffer for reading data
                Byte[] bytes = new Byte[256];
                String data = null;
                //TcpClient Client = server.AcceptTcpClient();
                // Enter the listening loop.
                for (int i = 0; i < 100; i++)
                {
                    Console.Write("Waiting for a connection... ");

                    ThreadPool.QueueUserWorkItem(CreatePerson, null);
                }
                while (true)
                {

                }
                //Client.Close();
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }
            finally
            {
                // Stop listening for new clients.
                server.Stop();
            }


            Console.WriteLine("\nHit enter to continue...");
            Console.Read();


            //IPHostEntry ipHostInfo = Dns.Resolve(Dns.GetHostName());
            //IPAddress ipAddress = ipHostInfo.AddressList[0];
            //IPAddress ipAddressLocal = new IPAddress(new byte[] { 127, 0, 0, 1 });
            //IPEndPoint localEndPoint = new IPEndPoint(ipAddressLocal, port);
            ////Console.WriteLine(ipAddress);
            //Console.WriteLine(ipAddressLocal + ":" + port);

            //Socket listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            //listener.Bind(localEndPoint);
            //listener.Listen(100);

            //Console.WriteLine("Waiting for a connection...");
            //Socket handler = listener.Accept();
            //String data = "";
            //byte[] bytes = new byte[1024];
            //while (!data.Contains("exit"))
            //{
            //    bytes = new byte[1024];
            //    int bytesRec = handler.Receive(bytes);
            //    data += Encoding.ASCII.GetString(bytes, 0, bytesRec);

            //    Console.WriteLine("Text received : {0}", data);
            //}
            //byte[] msg = Encoding.ASCII.GetBytes(data);
            //handler.Send(msg);
            //handler.Shutdown(SocketShutdown.Both);
            //handler.Close();
        }

        public static void CreatePerson(Object o)
        {
            Person p = new Person();
            p.GetInLine();
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