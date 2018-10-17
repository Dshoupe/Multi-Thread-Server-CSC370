using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestServer;
using TestServer.Models;

namespace ServerTestHarness
{
    [TestClass]
    public class ServerTests
    {
        [TestMethod]
        public void ServerTestWith100PeopleWith1SecondTimeLimit()
        {
            Program.waitTimeMin = 100;
            Program.waitTimeMax = 1000;
            Program.writeTimeMin = 100;
            Program.writeTimeMax = 1000;

            Program.TakeNumberSemaphore = new Semaphore(Program.TotalNumOfCustomer, Program.TotalNumOfCustomer);

            try
            {
                // Set the TcpListener on port 13000.
                Int32 port = 8000;
                IPAddress localAddr = IPAddress.Parse("127.0.0.1");

                // TcpListener server = new TcpListener(port);
                Program.server = new TcpListener(localAddr, port);

                // Start listening for client requests.
                Program.server.Start();

                // Buffer for reading data
                Byte[] bytes = new Byte[256];
                String data = null;
                TcpClient Client = Program.server.AcceptTcpClient();
                // Enter the listening loop.
                for (int i = 0; i < Program.TotalNumOfCustomer; i++)
                {
                    Console.Write("Waiting for a connection... ");

                    ThreadPool.QueueUserWorkItem(Program.CreatePerson, null);
                }
                while (Program.TotalCustomer < 100) { }
                Client.Close();
                Assert.AreEqual(Program.TotalCustomer, 100);
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }
            finally
            {
                // Stop listening for new clients.
                Program.server.Stop();
            }
        }
    }
}